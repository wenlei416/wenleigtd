using System;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.Models;

namespace GTD.Util
{
    public static class DropDownListHelp
    {
        public static SelectList PopulateProjectsDropDownList(GTDContext db,int? selectProject = null)
        {
            var projectsQuery = from p in db.Projects orderby p.ProjectName select p;
            return  new SelectList(projectsQuery, "ProjectID", "ProjectName", selectProject);
        }

        public static SelectList PopulateContextsDropDownList(GTDContext db,int? selectContext = null)
        {
            var contextQuery = from c in db.Contexts orderby c.ContextId select c;
            return new SelectList(contextQuery, "ContextId", "ContextName", selectContext);
        }

        public static SelectList PopulatePrioritysDropDownList(string selectPriority = "中")
        {
            var priorityQuery = from Priority s in Enum.GetValues(typeof(Priority)) select new { ID = s, Name = s.ToString() };
            return new SelectList(priorityQuery, "ID", "Name", selectPriority);
        }

        public static SelectList PopulateDateAttributeDropDownList(string selectDateAttribute = null)
        {
            var dateAttributeQuery = from DateAttribute s in Enum.GetValues(typeof(DateAttribute))
                select new {ID = s, Name = s.ToString()};
            return new SelectList(dateAttributeQuery, "ID", "Name", selectDateAttribute);
        }

        public static SelectList PopulateTaskDropDownList(GTDContext db, int? selectTask = null)
        {
            var taskQuery = from t in db.Tasks where t.IsComplete==false && t.IsDeleted==false orderby t.TaskId select t;
            return new SelectList(taskQuery, "TaskID", "Headline", selectTask);
        }

    }
}