using System;

namespace Imagine.Models
{
    public enum TaskStatus
    {
        NotCompleted,
        Completed
    }
    public class ScheduledTask
    {
        public Guid Id { get; set; }
        public virtual TaskEntity Task { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public bool Locked { get; set; }
    }
}