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

        public void CreateProject(Project project)
        {
            _projectrepository.Create(project);
        }

        public IEnumerable<Project> GetAllProjects()
        {
           return _projectrepository.GetAll();
        }

        public void UpdateProject(Project project)
        {
            _projectrepository.Update(project);
        }

        public void DeleteProjectByLogic(Project project)
        {
            project.IsDeleted = true;
            _projectrepository.Update(project);
        }
    }
}