using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services.Abstract;

namespace GTD.Services
{
    public class SubTaskServices : ISubTaskServices

    {
        private readonly ISubTaskRepository _subTaskRepository;

        public SubTaskServices(ISubTaskRepository subTaskRepository)
        {
            this._subTaskRepository = subTaskRepository;
        }

        public SubTask GetSubTaskById(int id)
        {
            return _subTaskRepository.GetSubTaskById(id);
        }

        public void CreateSubTask(SubTask subTask)
        {
            _subTaskRepository.Create(subTask);
        }

        public IEnumerable<SubTask> GetAllSubTasks()
        {
            return _subTaskRepository.GetAll();
        }

        public void UpdateSubTask(SubTask subTask)
        {
            _subTaskRepository.Update(subTask);
        }

        public void DeleteSubTask(SubTask subTask)
        {
            _subTaskRepository.Delete(subTask);
        }
    }
}