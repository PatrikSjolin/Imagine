using System;

namespace Imagine.Models
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public virtual TaskEntity Task { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
    }
}