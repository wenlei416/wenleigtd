using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IContextRepository:IRepository<Context>
    {
        Context GetContextById(int? contextId);
    }
}
