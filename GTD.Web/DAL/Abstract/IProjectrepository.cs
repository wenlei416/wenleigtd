using GTD.Models;

namespace GTD.DAL.Abstract
{
    public interface IProjectrepository:IRepository<Project>
    {
        Project GetById(int projectId);
        int CreateWithId(Project instance);

    }
}
