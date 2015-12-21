using System.Web;
using System.Web.Mvc;
using GTDTest.Filters;

namespace GTDTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //这个全局过滤器用来统计每个dateattribe下的任务数量，显示在导航菜单中
            filters.Add(new TaskCount());
        }
    }
}