using System.Collections.Generic;
using GTD.Models;

namespace GTD.Services.Abstract
{
    public interface ISubTaskServices
    {
        SubTask GetSubTaskById(int id);
        void CreateSubTask(SubTask subTask);
        IEnumerable<SubTask> GetAllSubTasks();
        void UpdateSubTask(SubTask subTask);
        void DeleteSubTask(SubTask subTask);
    }
}
