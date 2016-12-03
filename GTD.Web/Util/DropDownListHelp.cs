using GTD.Models;
using GTD.Services.Abstract;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GTD.Util
{
    public static class DropDownListHelp
    {

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
    }
}