using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class SubTaskRepository : GenericRepository<SubTask>, ISubTaskRepository
    {
        public SubTask GetSubTaskById(int? subTaskId)
        {
            return this.Get(t => t.SubTaskId == subTaskId);
        }
    }
}