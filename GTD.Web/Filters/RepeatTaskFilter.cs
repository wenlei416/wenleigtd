using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GTD.Filters
{
    public class RepeatTaskFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var requestCookie = HttpContext.Current.Request.Cookies["lastCreateRepeatTaskDate"];
            if (requestCookie != null)
            {
                //获取cookie中的最后创建日期
                var lastCreateRepeatTaskDate = Convert.ToDateTime(requestCookie.Value,
                    new DateTimeFormatInfo() {ShortDatePattern = "yyyyMMdd"});
                if (lastCreateRepeatTaskDate >= DateTime.Now.Date)
                {
                    //检查是否真的创建了
                    //如果真创建了
                    //啥也不做，直接return
                    return;
                }
                //检查是否真的没创建了
                //如果真没创建
                //创建循环任务
                //更新cooike
                requestCookie.Value = DateTime.Now.Date.ToString("yyyyMMdd");
            }
            else
            {
                //检查是否真的没创建了
                //如果真没创建
                //创建循环任务
                //创建cookie
                HttpCookie cookie = new HttpCookie("lastCreateRepeatTaskDate") {Value = DateTime.Now.Date.ToString("yyyyMMdd")};
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}