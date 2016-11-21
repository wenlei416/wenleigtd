using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GTD.Models;
using GTD.Util;

namespace GTD.Filters
{
    public class LogActionAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //base.OnActionExecuting(filterContext);
            //var model = (Task) filterContext.Controller.ViewData.Model;
            //var viewBag = filterContext.Controller.ViewBag;

            //var model = filterContext.ActionParameters["model"] as Task;
            //if (model == null)
            //    return;

            //LogHelper.WriteLog("start: "+model.Headline);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //base.OnActionExecuted(filterContext);
            //var model = (Task)filterContext.Controller.ViewData.Model;
            //LogHelper.WriteLog(model.Headline);
            //var model = filterContext.ActionDescriptor["model"] as Task;
            //if (model == null)
            //    return;

            //LogHelper.WriteLog("end: " + model.Headline);
        }

        //public void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    //var model = (Task)filterContext.Controller.ViewData.Model;
        //    //LogHelper.WriteLog("start " + model.Headline);
        //}

        //public void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    var model = (Task)filterContext.Controller.ViewData.Model;
        //    LogHelper.WriteLog("end " + model.Headline);
        //}
    }
}