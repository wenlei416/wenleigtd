using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IPomodoroRepository : IRepository<Pomodoro>
    {
        Pomodoro GetPomodoroById(int? pomodoroId);
    }
}