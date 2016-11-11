
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface IPomodoroServices
    {
        Pomodoro GetPomodoroById(int id);
        void CreatePomodoro(Pomodoro pomodoro);
        IEnumerable<Pomodoro> GetAllPomodoroes();
        void UpdatePomodoro(Pomodoro pomodoro);
        void DeletePomodoro(Pomodoro pomodoro);
    }
}
