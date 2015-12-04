using Imagine.Models;
using Microsoft.AspNet.Identity;
using System;
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
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}