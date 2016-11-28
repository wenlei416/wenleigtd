using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using GTD.Models;

namespace GTD.Filters.Tests
{

    [TestClass()]
    public class RepeatTaskFilterAttributeTests
    {
        /*
         * 最早传进来的是所有有json字符串的任务
         * 测试场景
         * 1.原list，比Cyc任务少，GetToBeCreadedTasks有新任务
         * 2.原list，和Cyc任务一样，GetToBeCreadedTasks无新任务
         * 3.原list中，有完成/删除的任务，需要注意
         * 4.原list中，生成不出来新的Cyc任务
         * 5.原list中，有过期任务，不影响GetToBeCreadedTasks的新任务（结合场景1和2）
         */
        public void GetToBeCreadedTasksTest()
        {
            //准备
            IEnumerable<Task> tasks = new List<Task>()
            {
                new Task()
                { TaskId = 1,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,27).Date,
                    RepeatJson = @"{'id':'1','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 2,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,28).Date,
                    RepeatJson = @"{'id':'1','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},
                new Task()
                { TaskId = 3,Headline = "Test1",Description = "TestD1",ProjectID = 1,ContextID = 1,
                    Priority = Priority.高,IsComplete = true,StartDateTime = new DateTime(2016,11,26).Date,
                    RepeatJson = @"{'id':'1','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"},

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

            IEnumerable<IGrouping<string, Task>> groupByRepeatJson = tasks.Where(t => t.RepeatJson.IsNullOrEmpty())
                .GroupBy(t => t.RepeatJson);

            //动作
            var z = RepeatTaskFilterAttribute.GetToBeCreadedTasks(groupByRepeatJson);

            var g101 = new Task()
            {
                Headline = "Test1",
                Description = "TestD1",
                ProjectID = 1,
                ContextID = 1,
                Priority = Priority.高,
                IsComplete = true,
                StartDateTime = new DateTime(2016, 11, 29).Date,
                RepeatJson =
                    @"{'id':'1','cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-11-25','endday':'2016-12-1','cyc':'1'}"
            };

            //判断
            Assert.AreEqual(z.Count,1);
            Assert.AreEqual(z[0].StartDateTime, g101.StartDateTime);
        }
    }
}