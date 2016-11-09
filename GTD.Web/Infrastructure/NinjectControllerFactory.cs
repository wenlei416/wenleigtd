using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using Ninject;
using Moq;

namespace GTD.Infrastructure
{
    public class NinjectControllerFactory:DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel =new StandardKernel();
            //AddBindings();
            AddBindingsFromMock();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController) ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IPomodoroRepository>().To<PomodoroRepository>();
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
            ninjectKernel.Bind<IPomodoroRepository>().ToConstant(pomodoroRepositoryMock.Object);

        }


    }
}