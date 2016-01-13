using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class ProjectServices : IProjectServices
    {
        private readonly IProjectrepository _projectrepository;

        public ProjectServices()
        {
            _projectrepository= new ProjectRepository();
        }

        public ProjectServices(IProjectrepository projectrepository)
        {
            _projectrepository = projectrepository;
        }


        public Project GetProjectById(int id)
        {
            return _projectrepository.GetById(id);
        }

        public void CreateProject(Project project)
        {
            _projectrepository.Create(project);
        }

        /// <summary>
        /// 查找所有Projects，无论状态
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 所有正在做的需求（未完成，未删除）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Project> GetAllInprogressProjects()
        {
            return _projectrepository.GetAll().Where(p => p.IsComplete == false && p.IsDeleted == false);
        }

        public int? IsExistByName(string projectName)
        {
            var pro = _projectrepository.Get(p => p.ProjectName == projectName);
            return pro == null ? (int?) null : _projectrepository.Get(p => p.ProjectName == projectName).ProjectId;
        }
    }
}
