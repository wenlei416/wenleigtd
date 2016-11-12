using System;
using System.Collections.Generic;
using System.Linq;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GTD.UT.Services.Tests
{
    [TestClass()]
    public class PomodoroServicesTests
    {

        public Mock<IPomodoroRepository> MockPomodoroRepository()
        {
            Mock<IPomodoroRepository> mock = new Mock<IPomodoroRepository>();
            mock.Setup(p => p.GetAll()).Returns(new List<Pomodoro>
                {
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7},
                    new Pomodoro{PomodoroId=7,IsCompletedPomodoro=true,StarDateTime=Convert.ToDateTime("2016/11/6 18:25:55"),EnDateTime=Convert.ToDateTime("2016/11/6 18:26:20"),IsWorkingTime=true,TaskId=7}

                }.AsQueryable()
            );
            mock.Setup(p => p.GetPomodoroById(7))
                .Returns(new Pomodoro
                {
                    PomodoroId = 7,
                    IsCompletedPomodoro = true,
                    StarDateTime = Convert.ToDateTime("2016/11/6 18:25:55"),
                    EnDateTime = Convert.ToDateTime("2016/11/6 18:26:20"),
                    IsWorkingTime = true,
                    TaskId = 7
                });
            return mock;
        }

        [TestMethod()]
        public void GetPomodoroByIdTest()
        {
            //准备
            var mockPomodoroRepository = MockPomodoroRepository();
            IPomodoroServices pomodoroServices = new PomodoroServices(mockPomodoroRepository.Object);

            //动作
            var pomodoro = pomodoroServices.GetPomodoroById(7);

            //断言
            Assert.IsTrue(pomodoro.IsCompletedPomodoro);
            Assert.IsTrue(pomodoro.IsWorkingTime);
            Assert.AreEqual(pomodoro.TaskId, 7);
            Assert.AreEqual(pomodoro.StarDateTime, Convert.ToDateTime("2016/11/6 18:25:55"));
            Assert.AreEqual(pomodoro.EnDateTime, Convert.ToDateTime("2016/11/6 18:26:20"));
        }

        [TestMethod()]
        public void GetAllPomodoroesTest()
        {
            //准备
            var mockPomodoroRepository = MockPomodoroRepository();
            IPomodoroServices pomodoroServices = new PomodoroServices(mockPomodoroRepository.Object);

            //动作
            var pomodoros = pomodoroServices.GetAllPomodoroes();

            //断言
            Assert.AreEqual(pomodoros.Count(), 5);


        }
    }
}
