using System;
using System.Collections.Generic;

namespace Imagine.Models
{
    public enum TaskType
    {
        Pending,
        Recurring,
        Event,
        OnTheGo
    }

    public class TaskViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Hours { get; set; }
        public bool Scheduled { get; set; }
    }
    public class TasksViewModel
    {
        public List<TaskViewModel> Tasks { get; set; }
        public List<ScheduledTaskViewModel> ScheduledTasks { get; set; }
    }
}