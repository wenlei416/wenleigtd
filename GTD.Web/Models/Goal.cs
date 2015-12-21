using System.Collections.Generic;

namespace GTD.Models
{
    public class Goal
    {
        public int GoalId { get; set; }
        public string GoalName { get; set; }

        public virtual ICollection<Project > Projects { get; set; }
    }
}