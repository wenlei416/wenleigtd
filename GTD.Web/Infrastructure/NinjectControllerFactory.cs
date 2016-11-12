using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using Ninject;
using Moq;

namespace GTD.Infrastructure
{
    //这个是pro asp.net mvc4上的注入做法，已经老了。新的做法不需要在这里注册ninjectcontrollerfactory，
    //也不需要ninjectcontrollerfacotry
    //而是在ninjectwebcommon里面注册各种bind，包括过滤器的
    //注意，引入了ninject对mvc的支持
    //public class NinjectControllerFactory:DefaultControllerFactory
    //{
    //    private readonly IKernel _ninjectKernel;

    //    public NinjectControllerFactory()
    //    {
    //        _ninjectKernel =new StandardKernel();
    //        AddBindings();
    //        //AddBindingsFromMock();
    //    }

    //    protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
    //    {
    //        return controllerType == null ? null : (IController) _ninjectKernel.Get(controllerType);
    //    }

    //    private void AddBindings()
    //    {
    //        _ninjectKernel.Bind<IPomodoroRepository>().To<PomodoroRepository>();
    //        _ninjectKernel.Bind<IProjectrepository>().To<ProjectRepository>();
    //        _ninjectKernel.Bind<ITaskRepository>().To<TaskRepository>();

    //        _ninjectKernel.Bind<ITaskServices>().To<TaskServices>();
    //        _ninjectKernel.Bind<IProjectServices>().To<ProjectServices>();
    //    }

    //    private void AddBindingsFromMock()
    //    {
    //        Mock<IPomodoroRepository> pomodoroRepositoryMock=new Mock<IPomodoroRepository>();
    //        pomodoroRepositoryMock.Setup(p => p.GetAll()).Returns(new List<Pomodoro>
    //            {
    //                new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
    //                new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
    //                new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
    //                new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
    //                new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7}

    //            }.AsQueryable()
    //        );
    //        _ninjectKernel.Bind<IPomodoroRepository>().ToConstant(pomodoroRepositoryMock.Object);

    //    }


    //}
}