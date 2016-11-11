using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class GoalRepository : GenericRepository<Goal>, IGoalRepository
    {
        public Goal GetGoalById(int? goalId)
        {
            return this.Get(t => t.GoalId == goalId);
        }
    }
}