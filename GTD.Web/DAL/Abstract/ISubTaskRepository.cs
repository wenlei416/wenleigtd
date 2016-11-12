using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface ISubTaskRepository : IRepository<SubTask>
    {
        SubTask GetSubTaskById(int? subTaskId);
    }
}
