using GTD.Filters;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Castle.Core.Internal;
using GTD.DAL;
using GTD.DAL.Abstract;
using GTD.Models;
using GTD.Services;
using GTD.Services.Abstract;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Shims;
using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;

namespace GTD.Filters.Tests
{

    [TestClass()]
    public class RepeatTaskFilterAttributeTests
    {
        private readonly MoqMockingKernel _kernel;

        public RepeatTaskFilterAttributeTests()
        {
            _kernel = new MoqMockingKernel();
            _kernel.Bind<ITaskServices>().To<TaskServices>();
        }

        [TestCleanup]
        public void SetUp()
        {
            _kernel.Reset();
        }
        /*
         * 整体和今天无关
         * 最早传进来的是所有有json字符串的任务
         * 测试场景
         * 1.原list，比Cyc任务少，GetToBeCreadedTasks有新任务
         * 2.原list，和Cyc任务多，GetToBeCreadedTasks无新任务
         * 3.原list中，有完成/删除的任务，需要注意
         * 4.原list中，生成不出来新的Cyc任务
         * 5.原list中，有过期任务，不影响GetToBeCreadedTasks的新任务（结合场景1和2）
         */
        [TestMethod]
        public void GetToBeCreadedTasksTest_Scene_1()
        {
            //准备
            IEnumerable<Task> exitsTasks = new List<Task>()
            {
                new Task()
                { TaskId = 4,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,26).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
               new Task()
                { TaskId = 4,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,27).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 4,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,28).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 4,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,29).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"}
            };

            IEnumerable<IGrouping<string, Task>> groupByRepeatJson = exitsTasks.Where(t => !t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);

            var result01 = new Task()
            {
                TaskId = 4,
                Headline = "Test1",
                Description = "TestD1",
                ProjectID = 1,
                ContextID = 1,
                Priority = Priority.高,
                IsComplete = false,
                IsDeleted = false,
                StartDateTime = new DateTime(2016, 11, 30).Date,
                RepeatJson =
                    @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
            };
            var result00 = new Task()
            {
                TaskId = 4,
                Headline = "Test1",
                Description = "TestD1",
                ProjectID = 1,
                ContextID = 1,
                Priority = Priority.高,
                IsComplete = false,
                IsDeleted = false,
                StartDateTime = new DateTime(2016, 11, 25).Date,
                RepeatJson =
        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
            };

            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 28);
                //动作
                var reslut = RepeatTaskFilterAttribute.GetToBeCreadedTasks(groupByRepeatJson);

                //判断
                Assert.AreEqual(reslut.Count, 2);
                Assert.AreEqual(reslut[0].StartDateTime, result00.StartDateTime);
                Assert.AreEqual(reslut[1].StartDateTime, result01.StartDateTime);
                Assert.AreEqual(reslut[1].IsComplete, result01.IsComplete);
                Assert.AreEqual(reslut[1].IsDeleted, result01.IsDeleted);
            }
        }

        [TestMethod]
        public void GetToBeCreadedTasksTest_Scene_2()
        {
            //准备
            #region 准备existTasks
            IEnumerable<Task> exitsTasks = new List<Task>()
            {
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = true,
                    StartDateTime = new DateTime(2016, 11, 26).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                },
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = true,
                    StartDateTime = new DateTime(2016, 11, 27).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                },
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = true,
                    StartDateTime = new DateTime(2016, 11, 28).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                },
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = true,
                    StartDateTime = new DateTime(2016, 11, 29).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                },
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = false,
                    IsDeleted = false,
                    StartDateTime = new DateTime(2016, 11, 30).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                },
                new Task()
                {
                    TaskId = 4,
                    Headline = "Test1",
                    Description = "TestD1",
                    ProjectID = 1,
                    ContextID = 1,
                    Priority = Priority.高,
                    IsComplete = false,
                    IsDeleted = false,
                    StartDateTime = new DateTime(2016, 11, 25).Date,
                    RepeatJson =
                        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
                }
            };
            #endregion

            IEnumerable<IGrouping<string, Task>> groupByRepeatJson = exitsTasks.Where(t => !t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);

            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 28);
                //动作
                var reslut = RepeatTaskFilterAttribute.GetToBeCreadedTasks(groupByRepeatJson);

                //判断
                Assert.AreEqual(reslut.Count, 0);
            }
        }

        [TestMethod()]
        public void OnActionExecutingTest()
        {
            IEnumerable<Task> exitsTasks = new List<Task>()
            {
                new Task()
                { TaskId = 1,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,26).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
               new Task()
                { TaskId = 2,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,27).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 3,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,28).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 4,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,29).Date,
                    RepeatJson = @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"}
            };
            var taskRepository = _kernel.GetMock<ITaskRepository>();
            //taskRepository.Setup(r => r.GetTaskById(1)).Returns(new Task()
            //{
            //    TaskId = 1,
            //    Headline = "Test1",
            //    Description = "TestD1",
            //    ProjectID = 1,
            //    ContextID = 1,
            //    Priority = Priority.高,
            //    IsComplete = true,
            //    StartDateTime = new DateTime(2016, 11, 26).Date,
            //    RepeatJson =
            //        @"{'id':'4','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
            //});
            //var taskServices = _kernel.Get<ITaskServices>();
            //Assert.AreEqual(taskServices.GetTaskById(1).Headline, "Test1");
            taskRepository.Setup(t => t.GetAll()).Returns(exitsTasks.AsQueryable());
            taskRepository.Setup(t => t.Create(new Task()));

            using (ShimsContext.Create())
            {
                // Mock今天是2016.11.29
                System.Fakes.ShimDateTime.NowGet =
                () => new DateTime(2016, 11, 29);

                var request = new Mock<HttpRequestBase>();
                var httpContext = new Mock<HttpContextBase>();
                httpContext.SetupGet(c => c.Request).Returns(request.Object);

                request.Setup(r => r.Cookies).Returns(new HttpCookieCollection() { new HttpCookie("lastCreateRepeatTaskDate", "2016-11-12") });


                var filter = new RepeatTaskFilterAttribute();
                var actionExecutedContext = new Mock<ActionExecutingContext>();
                actionExecutedContext.SetupGet(c => c.HttpContext).Returns(httpContext.Object);

                filter.OnActionExecuting(actionExecutedContext.Object);

                Assert.AreEqual(actionExecutedContext.Object.HttpContext.Request.Cookies["lastCreateRepeatTaskDate"].Value, "2016-11-12");
            }
        }
    }
}