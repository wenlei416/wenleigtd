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
         IEnumerable<Context> GetContext();
    }
}
