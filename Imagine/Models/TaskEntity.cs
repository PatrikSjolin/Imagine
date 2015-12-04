using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel;

namespace Imagine.Models
{
    public enum Period
    {   
        Day,
        Week,
        Month
    }

    public class TaskEntity
    {
        public Guid Id { get; set; }
        public virtual IdentityUser User { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Completed { get; set; }
        public Period? Period { get; set; }
        public int? Frequency { get; set; }
    }
}