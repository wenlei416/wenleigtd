using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL;
using GTD.Models;

namespace GTD.Services
{
    public class ContextServices
    {
        readonly GTDContext _gtdContext = new GTDContext();
        public IEnumerable<Context> GetContext()
        {


            return _gtdContext.Contexts;
        }
    }
}