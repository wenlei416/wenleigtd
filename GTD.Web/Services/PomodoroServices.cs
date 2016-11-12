using System.Collections.Generic;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class PomodoroServices:IPomodoroServices
    {
        private readonly IPomodoroRepository _pomodoroRepository;

        public PomodoroServices(IPomodoroRepository pomodoroRepository)
        {
            this._pomodoroRepository = pomodoroRepository;
        }

        public Pomodoro GetPomodoroById(int id)
        {
            return _pomodoroRepository.GetPomodoroById(id);
        }

        public void CreatePomodoro(Pomodoro pomodoro)
        {
            _pomodoroRepository.Create(pomodoro);
        }

        public IEnumerable<Pomodoro> GetAllPomodoroes()
        {
            return _pomodoroRepository.GetAll();
        }

        public void UpdatePomodoro(Pomodoro pomodoro)
        {
            _pomodoroRepository.Update(pomodoro);
        }

        public void DeletePomodoro(Pomodoro pomodoro)
        {
            _pomodoroRepository.Delete(pomodoro);
        }
    }
}