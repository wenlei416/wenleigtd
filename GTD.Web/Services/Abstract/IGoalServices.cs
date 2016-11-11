using System.Collections.Generic;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface IGoalServices
    {
        Goal GetGoalById(int id);
        void CreateGoal(Goal goal);
        IEnumerable<Goal> GetAllGoals();
        void UpdateGoal(Goal goal);
        void DeleteGoal(Goal goal);
    }
}
