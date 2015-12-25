using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface IProjectServices
    {
        Project GetProjectById(int id);
        void CreateProject(Project project);
        IEnumerable<Project> GetAllProjects();
        void UpdateProject(Project project);
        void DeleteProjectByLogic(Project project);


    }


}
