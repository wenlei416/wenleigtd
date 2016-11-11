using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IContextRepository:IRepository<Context>
    {
        Context GetContextById(int? contextId);
    }
}
