using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imagine.Models
{
    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
    public class TasksViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
    }
}