using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Filters;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using Moq;
using Ninject.Web.Mvc.FilterBindingSyntax;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(GTD.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(GTD.App_Start.NinjectWebCommon), "Stop")]

namespace GTD.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.BindFilter<TaskCount>(FilterScope.Global, 1).InRequestScope();
            kernel.Bind<IPomodoroRepository>().To<PomodoroRepository>();
            kernel.Bind<IProjectrepository>().To<ProjectRepository>();
            kernel.Bind<ITaskRepository>().To<TaskRepository>();

            kernel.Bind<ITaskServices>().To<TaskServices>();
            kernel.Bind<IProjectServices>().To<ProjectServices>();
            kernel.Bind<IPomodoroServices>().To<PomodoroServices>();

        }

        //需要重新设计mock的代码
        private static void AddBindingsFromMock(IKernel kernel)
        {
            Mock<IPomodoroRepository> pomodoroRepositoryMock = new Mock<IPomodoroRepository>();
            pomodoroRepositoryMock.Setup(p => p.GetAll()).Returns(new List<Pomodoro>
                {
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7}

                }.AsQueryable()
            );
            kernel.Bind<IPomodoroRepository>().ToConstant(pomodoroRepositoryMock.Object);

        }

    }
}
