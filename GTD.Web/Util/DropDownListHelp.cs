using GTD.Models;
using GTD.Services.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GTD.Util
{
    public static class DropDownListHelp
    {
        public static SelectList PopulateProjectsDropDownList(IProjectServices projectServices, int? selectProject = null)
        {
            return new SelectList(projectServices.GetAllInprogressProjects(), "ProjectID", "ProjectName", selectProject);
        }

        public static SelectList PopulateContextsDropDownList(IContextServices contextServices, int? selectContext = null)
        {
            return new SelectList(contextServices.GetAllContexts(), "ContextId", "ContextName", selectContext);
        }

        public static SelectList PopulatePrioritysDropDownList(string selectPriority = "中")
        {
            var priorityQuery = from Priority s in Enum.GetValues(typeof(Priority)) select new { ID = s, Name = s.ToString() };
            return new SelectList(priorityQuery, "ID", "Name", selectPriority);
        }

        public static SelectList PopulateDateAttributeDropDownList(string selectDateAttribute = null)
        {
            var dateAttributeQuery = from DateAttribute s in Enum.GetValues(typeof(DateAttribute))
                                     select new { ID = s, Name = s.ToString() };
            return new SelectList(dateAttributeQuery, "ID", "Name", selectDateAttribute);
        }

        public static SelectList PopulateTaskDropDownList(ITaskServices taskServices, int? selectTask = null)
        {
            var task = taskServices.GetInProgressTasks();//from t in db.Tasks where t.IsComplete==false && t.IsDeleted==false orderby t.TaskId select t;
            return new SelectList(task, "TaskID", "Headline", selectTask);
        }
    }
}