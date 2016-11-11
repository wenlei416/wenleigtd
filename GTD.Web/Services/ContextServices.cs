using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class ContextServices:IContextServices
    {
        readonly GTDContext _gtdContext = new GTDContext();
        public IEnumerable<Context> GetContext()
        {
            return _gtdContext.Contexts;
        }
    }
}