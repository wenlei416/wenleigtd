using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using GTD.Models;

namespace GTD.Util.Tests
{
    [TestClass()]
    public class TaskUtilTests
    {
        //需要批量更新的修改：标题，描述，项目，场景，优先级。其他的都需要批量更新，允许不一致
        [TestMethod()]
        public void ModifiedPropertiesInListTest()
        {
            Task oldTask = new Task { Headline = "a" };
            Task newTask = new Task { Headline = "b" };
            var b = TaskUtil.ModifiedPropertiesInList(oldTask, newTask);
            Assert.AreEqual(b, true);
        }

        [TestMethod()]
        public void ModifiedPropertiesInListTest2()
        {
            Task oldTask = new Task { IsComplete = true };
            Task newTask = new Task { IsComplete = false };
            var b = TaskUtil.ModifiedPropertiesInList(oldTask, newTask);
            Assert.AreEqual(b, false);
        }

        //[TestMethod()]
        public void UpdateRepeatTasksPropertiesTest()
        {
            //准备
            //需要批量更新的修改：标题，描述，项目，场景，优先级。其他的都需要批量更新，允许不一致
            IQueryable<Task> repeatTasks = new List<Task>
            {
                new Task(){Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,Priority = Priority.高,IsComplete = true},
                new Task(){Headline = "Test0",Description = "TestD0",ProjectID = 0,ContextID = 1,Priority = Priority.低,IsComplete = false},
                new Task(){Headline = "Test0",Description = "TestD0",ProjectID = 0,ContextID = 1,Priority = Priority.低,IsComplete = true},
            }.AsQueryable();
            Task templateTask = new Task()
            {
                Headline = "Test1",
                Description = "TestD1",
                ProjectID = 1,
                ContextID = 1,
                Priority = Priority.高,
                IsComplete = false
            };

            //动作
            var resultTasks = TaskUtil.UpdateRepeatTasksProperties(repeatTasks, templateTask);

            //断言
            Assert.AreEqual(resultTasks.Count(), repeatTasks.Count());
            for (int i = 0; i < resultTasks.Count(); i++)
            {
                Assert.AreEqual(resultTasks.ToList()[i].Headline, templateTask.Headline);
                Assert.AreEqual(resultTasks.ToList()[i].Description, templateTask.Description);
                Assert.AreEqual(resultTasks.ToList()[i].ProjectID, templateTask.ProjectID);
                Assert.AreEqual(resultTasks.ToList()[i].ContextID, templateTask.ContextID);
                Assert.AreEqual(resultTasks.ToList()[i].Priority, templateTask.Priority);
                Assert.AreEqual(resultTasks.ToList()[i].IsComplete, repeatTasks.ToList()[i].IsComplete);
            }
        }

        //测试场景
        // 1. 有或没有closeTime
        // 2. 是否有符合今天、明天、日程的任务（今天，明天，日程；今天，日程；明天，日程；日程）
        //这里需要设定今天的日期，测试日期是1127，需要继续找mock日期的方法 
        [TestMethod()]
        public void CreateCycTasksTest1()
        {
            var task = new Task()
            {
                TaskId = 100,
                Headline = "测试CreateCycTasksTest",
                StartDateTime = new DateTime(2016, 11, 27),
                RepeatJson = @"{'id':'10','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-11-30','cyc':'1'}"
            };
            var ts = TaskUtil.CreateCycTasks(task);
            Assert.AreEqual(ts.Count, 3);
            Assert.AreEqual(ts[0].StartDateTime, new DateTime(2016, 11, 27));
            Assert.AreEqual(ts[1].StartDateTime, new DateTime(2016, 11, 28));
            Assert.AreEqual(ts[2].StartDateTime, new DateTime(2016, 11, 29));
            Assert.AreEqual(ts[0].CloseDateTime, null);
        }

        [TestMethod]
        public void CreateCycTasksTest2()
        {
            var task = new Task()
            {
                TaskId = 100,
                Headline = "测试CreateCycTasksTest",
                StartDateTime = new DateTime(2016, 11, 27),
                CloseDateTime = new DateTime(2016, 11, 28),
                RepeatJson = @"{'id':'10','cyear':'0','cmonth':'0','cweek':'0','cday':'2','startday':'2016-11-25','endday':'2016-11-30','cyc':'2'}"
            };
            var ts = TaskUtil.CreateCycTasks(task);
            Assert.AreEqual(ts.Count, 2);
            Assert.AreEqual(ts[0].StartDateTime, new DateTime(2016, 11, 27));
            Assert.AreEqual(ts[1].StartDateTime, new DateTime(2016, 11, 29));
            Assert.AreEqual(ts[0].CloseDateTime, new DateTime(2016, 11, 28));
            Assert.AreEqual(ts[1].CloseDateTime, new DateTime(2016, 11, 30));
        }

        [TestMethod]
        public void CreateCycTasksTest3()
        {
            var task = new Task()
            {
                TaskId = 100,
                Headline = "测试CreateCycTasksTest",
                StartDateTime = new DateTime(2016, 11, 27),
                CloseDateTime = new DateTime(2016, 11, 28),
                RepeatJson = @"{'id':'10','cyear':'0','cmonth':'0','cweek':'0','cday':'2','startday':'2016-11-26','endday':'2016-11-30','cyc':'2'}"
            };
            var ts = TaskUtil.CreateCycTasks(task);
            Assert.AreEqual(ts.Count, 2);
            Assert.AreEqual(ts[0].StartDateTime, new DateTime(2016, 11, 28));
            Assert.AreEqual(ts[1].StartDateTime, new DateTime(2016, 11, 30));
            Assert.AreEqual(ts[0].CloseDateTime, new DateTime(2016, 11, 29));
            Assert.AreEqual(ts[1].CloseDateTime, new DateTime(2016, 12, 1));
        }

        [TestMethod]
        public void CreateCycTasksTest4()
        {
            var task = new Task()
            {
                TaskId = 100,
                Headline = "测试CreateCycTasksTest",
                StartDateTime = new DateTime(2016, 11, 27),
                RepeatJson = @"{'id':'10','cyear':'0','cmonth':'0','cweek':'0','cday':'4','startday':'2016-11-26','endday':'2016-11-30','cyc':'4'}"
            };
            var ts = TaskUtil.CreateCycTasks(task);
            Assert.AreEqual(ts.Count, 1);
            Assert.AreEqual(ts[0].StartDateTime, new DateTime(2016, 11, 30));
            Assert.AreEqual(ts[0].CloseDateTime, null);
        }

    }
}