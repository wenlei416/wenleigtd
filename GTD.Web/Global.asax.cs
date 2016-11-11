using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GTD.Infrastructure;

namespace GTD
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //这个是pro asp.net mvc4上的注入做法，已经老了。新的做法不需要在这里注册ninjectcontrollerfactory，
            //也不需要ninjectcontrollerfacotry
            //而是在ninjectwebcommon里面注册各种bind，包括过滤器的
            //注意，引入了ninject对mvc的支持
            //ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
            

        }
    }
}