using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using GTD.Models;

namespace GTD.Filters.Tests
{
    /*
     * 测试场景
     * group过后，
     * group中，有新任务
     * group中，没有新任务
     * exist任务中，有以前的任务
     */
    [TestClass()]
    public class RepeatTaskFilterAttributeTests
    {
        //测试场景
        // 1. 生成的任务都在list中，不需要新建
        // 2. 生成的任务在list中缺少部分，需要新建
        // 3. list中有过期任务，没有额外影响
        public void GetValueTest()
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