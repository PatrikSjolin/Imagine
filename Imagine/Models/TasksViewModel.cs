using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imagine.Models
{
    public enum TaskType
    {
        Pending,
        Recurring,
        Event
    }

    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }       
    }
    public class TasksViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
        public List<ScheduledTaskViewModel> ScheduledTasks { get; set; }
    }
}