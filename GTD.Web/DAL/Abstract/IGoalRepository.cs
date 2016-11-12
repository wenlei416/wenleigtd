using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IGoalRepository : IRepository<Goal>
    {
        Goal GetGoalById(int? goalId);
    }
}
