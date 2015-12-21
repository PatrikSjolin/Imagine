using Imagine.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Imagine.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly Random random;

        public HomeController() : base()
        {
            context = new ApplicationDbContext();
            
            int seed = (int)DateTime.Now.Ticks & 0x0000FFFF;
            random = new Random(seed);
        }

        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            TasksViewModel vm = new TasksViewModel();

            vm.Tasks = new List<TaskViewModel>();

            List<ScheduledTask> scheduled = context.ScheduledTasks.Include("Task").Where(x => x.Task.User.Id == userId).ToList();

            vm.ScheduledTasks = GetScheduledTasks(scheduled);

            foreach (var task in context.Tasks.Where(x => x.User.Id == userId).OrderBy(x => x.Created))
            {
                TaskType type = TaskType.Pending;
                if (task.Period != null)
                {
                    type = TaskType.Recurring;
                }
                else if (task.DueDate != null)
                {
                    type = TaskType.Event;
                }
                vm.Tasks.Add(new TaskViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    Type = task.TaskType.ToString(),
                    Hours = task.Duration,
                    Scheduled = scheduled.Any(x => x.Task.Id == task.Id)
                });
            }

            return View(vm);
        }

        private List<ScheduledTaskViewModel> GetScheduledTasks(List<ScheduledTask> scheduled)
        {
            List<ScheduledTaskViewModel> model = new List<ScheduledTaskViewModel>();

            if (scheduled.Count > 0)
            {
                DateTime min = scheduled.Min(x => x.Date);
                DateTime max = scheduled.Max(x => x.Date);

                for (int i = 0; i <= max.Subtract(min).Days; i++)
                {
                    model.Add(new ScheduledTaskViewModel
                    {
                        Date = min.Date.AddDays(i),
                        Tasks = new List<TaskViewModel>()
                    });
                }

                foreach (var schedule in scheduled.OrderBy(x => x.Date))
                {
                    var scheduledTask = model.FirstOrDefault(x => x.Date.ToShortDateString() == schedule.Date.ToShortDateString());
                    if (scheduledTask != null)
                    {
                        scheduledTask.Tasks.Add(new TaskViewModel
                        {
                            Id = schedule.Id,
                            Name = schedule.Task.Name,
                            Hours = schedule.Task.Duration,
                            TaskStatus = schedule.TaskStatus,
                            Locked = schedule.Locked
                        });
                    }
                }
            }

            return model;
        }

        [HttpPost]
        public ActionResult Add(string id, string name, DateTime? date, DateTime? time, int? duration)
        {
            TaskType taskType = TaskType.Pending;
            string userId = User.Identity.GetUserId();

            bool full = context.Tasks.Count(x => x.User.Id == userId) > 20;

            DateTime? dueDate = null;

            if (date.HasValue && time.HasValue)
            {
                dueDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Value.Hour, time.Value.Minute, time.Value.Second);
                taskType = TaskType.Event;
            }
            else if (date.HasValue)
            {
                dueDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                taskType = TaskType.Event;
            }
            else if (time.HasValue)
            {
                dueDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Value.Hour, time.Value.Minute, time.Value.Second);
                taskType = TaskType.Event;
            }

            if (!full)
            {
                context.Tasks.Add(
                    new TaskEntity
                    {
                        Id = new Guid(id),
                        Name = name,
                        Created = DateTime.Now,
                        User = context.Users.First(x => x.Id == userId),
                        Modified = DateTime.Now,
                        DueDate = dueDate,
                        Duration = duration.HasValue ? duration.Value : 0,
                        TaskType = taskType
                    });
                context.SaveChanges();
            }

            return Json(new { Code = 200 });
        }

        [HttpPost]
        public ActionResult AddOnTheGo(string id, string name)
        {
            string userId = User.Identity.GetUserId();

            TaskEntity entity = new TaskEntity
            {
                Created = DateTime.Now,
                Id = new Guid(id),
                Name = name,
                User = context.Users.First(x => x.Id == userId),
                TaskType = TaskType.OnTheGo
            };

            context.Tasks.Add(entity);
            context.SaveChanges();

            SchedulePending(new List<TaskEntity> { entity }, new TimeSpan(3, 0, 0, 0), DateTime.Today);
            context.SaveChanges();

            List<ScheduledTask> scheduled = context.ScheduledTasks.Include("Task").Where(x => x.Task.User.Id == userId).ToList();
            var scheduledTasks = GetScheduledTasks(scheduled);
            return Json(new { Code = 200, Data = scheduledTasks });
        }

        [HttpPost]
        public ActionResult AddRecurringTask(string id, string name, string period, int? frequency, DateTime? date, int? duration)
        {
            string userId = User.Identity.GetUserId();

            TaskEntity entity = new TaskEntity
            {
                Created = DateTime.Now,
                Id = new Guid(id),
                Name = name,
                User = context.Users.First(x => x.Id == userId),
                Frequency = frequency,
                Period = (Period)Enum.Parse(typeof(Period), period),
                DueDate = date,
                Duration = duration != null ? duration.Value : 0,
                TaskType = TaskType.Recurring
            };
            context.Tasks.Add(entity);
            context.SaveChanges();

            return Json(new { Code = 200 });
        }

        [HttpPost]
        public ActionResult Edit(string id, string name, string duration)
        {
            TaskEntity task = context.Tasks.First(x => x.Id == new Guid(id));
            task.Name = name;
            task.Duration = int.Parse(duration);
            context.SaveChanges();

            return Json(new { Code = 200 });
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            context.Tasks.Remove(context.Tasks.First(x => x.Id == new Guid(id)));
            context.ScheduledTasks.RemoveRange(context.ScheduledTasks.Where(x => x.Task.Id == new Guid(id)));
            context.SaveChanges();

            return Json(new { Code = 200 });
        }

        [HttpPost]
        public ActionResult AddSchedule(DateTime? from, DateTime? to)
        {
            if (from == null)
            {
                from = DateTime.Today;
            }
            if (to == null)
            {
                to = from.Value.AddDays(14);
            }
            string userId = User.Identity.GetUserId();

            context.ScheduledTasks.RemoveRange(context.ScheduledTasks.Where(x => !x.Locked));
            context.SaveChanges();

            var lockedTasks = context.ScheduledTasks.Include("Task").Where(x => x.Locked).ToList();

            List<TaskEntity> userTasks = context.Tasks.Where(x => x.User.Id == userId).ToList();

            List<TaskEntity> tasksWithDueDates = userTasks.Where(x => x.DueDate.HasValue && !x.Period.HasValue && !lockedTasks.Any(y => y.Task.Id == x.Id)).ToList();
            List<TaskEntity> recurringTasks = userTasks.Where(x => x.Period.HasValue).ToList();
            List<TaskEntity> pendingTasks = userTasks.Where(x => !x.Frequency.HasValue && !x.Period.HasValue && !x.DueDate.HasValue && !lockedTasks.Any(y => y.Task.Id == x.Id)).ToList();

            foreach (var taskWithDueDate in tasksWithDueDates)
            {
                context.ScheduledTasks.Add(new ScheduledTask
                {
                    Date = taskWithDueDate.DueDate.Value,
                    Id = Guid.NewGuid(),
                    Hours = taskWithDueDate.Duration,
                    Task = taskWithDueDate
                });
            }

            TimeSpan period = to.Value.Subtract(from.Value);

            foreach (var recurring in recurringTasks)
            {
                if (recurring.Period == Period.WorkDay)
                {
                    for (int i = 0; i < period.Days; i++)
                    {
                        if (from.Value.AddDays(i).DayOfWeek != DayOfWeek.Saturday && from.Value.AddDays(i).DayOfWeek != DayOfWeek.Sunday)
                        {
                            context.ScheduledTasks.Add(new ScheduledTask
                            {
                                Date = from.Value.AddDays(i),
                                Hours = recurring.Duration,
                                Id = Guid.NewGuid(),
                                Task = recurring
                            });
                        }
                    }
                    continue;
                }
                int days = 1;
                if (recurring.Period == Period.Week)
                {
                    days = 7;
                }
                else if (recurring.Period == Period.Month)
                {
                    days = 30;
                }

                double frequency = 1;
                if (recurring.Frequency.HasValue)
                {
                    frequency = recurring.Frequency.Value;
                }
                double daysBetweenActivities = days / frequency;

                DateTime startDate = from.Value;
                if (recurring.DueDate.HasValue)
                {
                    startDate = recurring.DueDate.Value;
                }
                else
                {
                    startDate = startDate.AddDays(random.Next((int)daysBetweenActivities));
                }
                for (int i = 0; i * daysBetweenActivities < period.Days; i++)
                {
                    DateTime newTime;
                    if (recurring.DueDate.HasValue)
                    {
                        if (days == 30)
                        {
                            newTime = startDate.AddMonths(i);
                        }
                        else if (days == 7)
                        {
                            newTime = startDate.AddDays(i * 7);
                        }
                        else
                        {
                            newTime = startDate.AddDays(i);
                        }
                        if (newTime < DateTime.Now)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        newTime = startDate.AddDays(i * daysBetweenActivities);
                    }
                    context.ScheduledTasks.Add(new ScheduledTask
                    {
                        Date = newTime,
                        Hours = recurring.Duration,
                        Id = Guid.NewGuid(),
                        Task = recurring
                    });
                }
            }

            SchedulePending(pendingTasks, period, from.Value);

            context.SaveChanges();

            List<ScheduledTask> scheduled = context.ScheduledTasks.Include("Task").Where(x => x.Task.User.Id == userId).ToList();
            var scheduledTasks = GetScheduledTasks(scheduled);

            return Json(new { Code = 200, Data = scheduledTasks });
        }

        private void SchedulePending(List<TaskEntity> pendingTasks, TimeSpan period, DateTime from)
        {
            foreach (TaskEntity pendingTask in pendingTasks)
            {
                context.ScheduledTasks.Add(new ScheduledTask
                {
                    Date = from.Add(new TimeSpan(random.Next(period.Days), random.Next(period.Hours), random.Next(period.Minutes), 0)),
                    Hours = pendingTask.Duration,
                    Id = Guid.NewGuid(),
                    Task = pendingTask
                });
            }
        }

        [HttpPost]
        public ActionResult CompleteTask(string id)
        {
            var task = context.ScheduledTasks.First(x => x.Id == new Guid(id));
            if(task.TaskStatus == TaskStatus.Completed)
            {
                task.TaskStatus = TaskStatus.NotCompleted;
                task.Locked = false;
            }
            else
            {
                task.TaskStatus = TaskStatus.Completed;
                task.Locked = true;
            }
            
            context.SaveChanges();
            return Json(new { Code = 200 });
        }

        [HttpPost]
        public ActionResult LockTask(string id)
        {
            var task = context.ScheduledTasks.First(x => x.Id == new Guid(id));
            task.Locked = !task.Locked;
            context.SaveChanges();
            return Json(new { Code = 200 });
        }
    }
}