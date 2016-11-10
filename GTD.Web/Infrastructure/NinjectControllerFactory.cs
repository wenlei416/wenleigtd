﻿using System;
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
    public class NinjectControllerFactory:DefaultControllerFactory
    {
        private readonly IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            _ninjectKernel =new StandardKernel();
            AddBindings();
            //AddBindingsFromMock();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) _ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            _ninjectKernel.Bind<IPomodoroRepository>().To<PomodoroRepository>();
            _ninjectKernel.Bind<IProjectrepository>().To<ProjectRepository>();

            _ninjectKernel.Bind<ITaskServices>().To<TaskServices>();
            _ninjectKernel.Bind<IProjectServices>().To<ProjectServices>();
        }

        private void AddBindingsFromMock()
        {
            Mock<IPomodoroRepository> pomodoroRepositoryMock=new Mock<IPomodoroRepository>();
            pomodoroRepositoryMock.Setup(p => p.GetAll()).Returns(new List<Pomodoro>
                {
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7}

                }.AsQueryable()
            );
            _ninjectKernel.Bind<IPomodoroRepository>().ToConstant(pomodoroRepositoryMock.Object);

        }


    }
}