using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class ProjectServices:IProjectServices
    {
        private readonly IProjectrepository _projectrepository = new ProjectRepository();


        public Project GetProjectById(int id)
        {
            return _projectrepository.GetById(id);
        }
    }
}