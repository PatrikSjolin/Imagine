using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Imagine.Models
{
    public class ScheduledTaskViewModel
    {
        public DateTime Date { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }
}