using GTD.Models;

namespace GTD.DAL.Abstract
{
    interface ISubTaskRepository : IRepository<SubTask>
    {
        SubTask GetSubTaskById(int? subTaskId);
    }
}
