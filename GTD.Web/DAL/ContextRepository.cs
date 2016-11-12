using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class ContextRepository : GenericRepository<Context>, IContextRepository
    {
        public Context GetContextById(int? contextId)
        {
            return this.Get(t => t.ContextId == contextId);

        }
    }
}