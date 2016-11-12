using GTD.DAL.Abstract;
using GTD.Models;

namespace GTD.DAL
{
    public class ProjectRepository:GenericRepository<Project>,IProjectrepository
    {
        public Project GetById(int projectId)
        {
            return Get(p => p.ProjectId == projectId);
        }
    }
}