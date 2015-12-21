using System;
using System.Collections.Generic;

namespace Imagine.Models
{
    public class ScheduledTaskViewModel
    {
        public DateTime Date { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }
}