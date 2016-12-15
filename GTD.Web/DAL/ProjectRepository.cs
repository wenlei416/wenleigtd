using System;
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

        //创建task，返回taskid
        public int CreateWithId(Project instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                base.Context.Set<Project>().Add(instance);
                SaveChanges();
            }
            return instance.ProjectId;
        }

    }
}