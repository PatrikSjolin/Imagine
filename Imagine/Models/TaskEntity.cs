using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Imagine.Models
{
    public enum Period
    {   
        Day,
        WorkDay,
        Week,
        Month
    }

    public class TaskEntity
    {
        public Guid Id { get; set; }
        public virtual IdentityUser User { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public DateTime Created { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Completed { get; set; }
        public Period? Period { get; set; }
        public int? Frequency { get; set; }
    }
}