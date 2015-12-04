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
        public ActionResult Index()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            TasksViewModel vm = new TasksViewModel();

            vm.Tasks = new System.Collections.Generic.List<TaskViewModel>();
            string userId = User.Identity.GetUserId();
            foreach (var task in context.Tasks.Where(x => x.User.Id == userId).OrderBy(x => x.Created))
            {
                vm.Tasks.Add(new TaskViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                });
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Add(string name, string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            string userId = User.Identity.GetUserId();

            bool full = context.Tasks.Count(x => x.User.Id == userId) > 20;

            if (!full)
            {
                context.Tasks.Add(
                    new TaskEntity
                    {
                        Id = new Guid(id),
                        Name = name,
                        Created = DateTime.Now,
                        User = context.Users.First(x => x.Id == userId),
                        Modified = DateTime.Now
                    });
                context.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            context.Tasks.Remove(context.Tasks.First(x => x.Id == new Guid(id)));
            context.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult AddSchedule(DateTime from, DateTime to)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();

            var userTasks = context.Tasks.Where(x => x.User.Id == userId);

            var tasksWithDueDates = userTasks.Where(x => x.DueDate.HasValue);
            var recurringTasks = userTasks.Where(x => x.Period.HasValue);
            var pendingTasks = userTasks.Where(x => !x.Frequency.HasValue && !x.Period.HasValue && !x.DueDate.HasValue);

            List<Schedule> schedule = new List<Schedule>();

            foreach(var taskWithDueDate in tasksWithDueDates)
            {
                schedule.Add(new Schedule
                {
                    Date = taskWithDueDate.DueDate.Value,
                    Id = Guid.NewGuid(),
                    Duration = new TimeSpan(1, 0, 0),
                    Task = taskWithDueDate
                });
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddRecurringTask(string id, string name, string period, int frequency)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();

            TaskEntity entity = new TaskEntity
            {
                Created = DateTime.Now,
                Id = new Guid(id),
                Name = name,
                User = context.Users.First(x => x.Id == userId),
                Frequency = frequency,
                Period = (Period)Enum.Parse(typeof(Period), period)
            };
            context.Tasks.Add(entity);
            context.SaveChanges();
            return View();
        }
    }
}