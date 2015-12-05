using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imagine.Models
{
    public class ScheduledTaskViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}