using System.Collections.Generic;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class GoalServices : IGoalServices

    {
        private readonly IGoalRepository _goalRepository;

        public GoalServices(IGoalRepository goalRepository)
        {
            this._goalRepository = goalRepository;
        }

        public Goal GetGoalById(int id)
        {
            return _goalRepository.GetGoalById(id);
        }

        public void CreateGoal(Goal goal)
        {
            _goalRepository.Create(goal);
        }

        public IEnumerable<Goal> GetAllGoals()
        {
            return _goalRepository.GetAll();
        }

        public void UpdateGoal(Goal goal)
        {
            _goalRepository.Update(goal);
        }

        public void DeleteGoal(Goal goal)
        {
            _goalRepository.Delete(goal);
        }

    }
}