using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GTDTest.Models
{
    public class Goal
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }

        public virtual ICollection<Project > Projects { get; set; }
    }
}