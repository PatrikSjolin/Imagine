﻿using Imagine.Models;
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

            vm.Tasks = new List<TaskViewModel>();
            string userId = User.Identity.GetUserId();
            foreach (var task in context.Tasks.Where(x => x.User.Id == userId).OrderBy(x => x.Created))
            {
                TaskType type = TaskType.Pending;
                if(task.Frequency != null)
                {
                    type = TaskType.Recurring;
                }
                else if(task.DueDate != null)
                {
                    type = TaskType.Event;
                }
                vm.Tasks.Add(new TaskViewModel
                {
                    Id = task.Id,
                    Name = task.Name,
                    Type = type.ToString()
                });
            }
            List<ScheduledTask> scheduled = context.ScheduledTasks.Include("Task").Where(x => x.Task.User.Id == userId).ToList();

            vm.ScheduledTasks = new List<ScheduledTaskViewModel>();
            foreach (var schedule in context.ScheduledTasks.Include("Task").Where(x => x.Task.User.Id == userId).OrderBy(x => x.Date))
            {
                vm.ScheduledTasks.Add(new ScheduledTaskViewModel
                {
                    Date = schedule.Date,
                    Id = schedule.Id,
                    Name = schedule.Task.Name
                });
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Add(string id, string name, DateTime? date, DateTime? time)
        {
            ApplicationDbContext context = new ApplicationDbContext();

            string userId = User.Identity.GetUserId();

            bool full = context.Tasks.Count(x => x.User.Id == userId) > 20;

            DateTime? dueDate = null;

            if(date.HasValue && time.HasValue)
            {
                dueDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day, time.Value.Hour, time.Value.Minute, time.Value.Second);
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
                        DueDate = dueDate
                    });
                context.SaveChanges();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<TaskEntity> taskEntities = context.Tasks.ToList();
            context.Tasks.Remove(taskEntities.First(x => x.Id == new Guid(id)));
            context.SaveChanges();
            return View();
        }

        [HttpPost]
        public ActionResult AddSchedule(DateTime? from, DateTime? to)
        {
            if(from == null)
            {
                from = DateTime.Today;
            }
            if(to == null)
            {
                to = DateTime.Today.AddDays(14);
            }
            ApplicationDbContext context = new ApplicationDbContext();
            string userId = User.Identity.GetUserId();
            context.ScheduledTasks.RemoveRange(context.ScheduledTasks);
            context.SaveChanges();

            List<TaskEntity> userTasks = context.Tasks.Where(x => x.User.Id == userId).ToList();

            List<TaskEntity> tasksWithDueDates = userTasks.Where(x => x.DueDate.HasValue).ToList();
            List<TaskEntity> recurringTasks = userTasks.Where(x => x.Period.HasValue).ToList();
            List<TaskEntity> pendingTasks = userTasks.Where(x => !x.Frequency.HasValue && !x.Period.HasValue && !x.DueDate.HasValue).ToList();

            foreach (var taskWithDueDate in tasksWithDueDates)
            {
                context.ScheduledTasks.Add(new ScheduledTask
                {
                    Date = taskWithDueDate.DueDate.Value,
                    Id = Guid.NewGuid(),
                    Hours = 1,
                    Task = taskWithDueDate
                });
            }

            TimeSpan period = to.Value.Subtract(from.Value);

            foreach (var recurring in recurringTasks)
            {
                int days = 1;
                if (recurring.Period == Period.Week)
                {
                    days = 7;
                }
                else if (recurring.Period == Period.Month)
                {
                    days = 30;
                }

                double daysBetweenActivities = days / (double)recurring.Frequency;

                for (int i = 0; i * daysBetweenActivities < period.Days; i++)
                {
                    context.ScheduledTasks.Add(new ScheduledTask
                    {
                        Date = from.Value.AddDays(i * daysBetweenActivities),
                        Hours = 1,
                        Id = Guid.NewGuid(),
                        Task = recurring
                    });
                }
            }

            Random rand = new Random();

            foreach(var pendingTask in pendingTasks)
            {
                context.ScheduledTasks.Add(new ScheduledTask
                {
                    Date = from.Value.Add(new TimeSpan(rand.Next(period.Days), rand.Next(period.Hours), rand.Next(period.Minutes), 0)),
                    Hours = 1,
                    Id = Guid.NewGuid(),
                    Task = pendingTask
                });
            }
            context.SaveChanges();
            return Index();
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