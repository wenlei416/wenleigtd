using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using GTD.Models;
using Microsoft.QualityTools.Testing.Fakes;
using System;

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

        //[TestMethod()]
        public void CreateCycTasksTest()
        {
            //var ts = TaskUtil.CreateCycTasks(new Task()
            //{
            //    Headline = "测试CreateCycTasksTest",
            //    StartDateTime = new DateTime()
            //});
            Assert.Fail();
        }


    }
}