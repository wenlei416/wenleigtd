using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IProjectrepository:IRepository<Project>
    {
        Project GetById(int projectId);
    }
}
