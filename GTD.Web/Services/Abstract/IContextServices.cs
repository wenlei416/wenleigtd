using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface IContextServices
    {
         Context GetContextById(int id);
         void CreateContext(Context context);
         IEnumerable<Context> GetAllContexts();
         void UpdateContext(Context context);
         void DeleteContext(Context context);
        int? IsExistByName(string contextName);
    }
}
