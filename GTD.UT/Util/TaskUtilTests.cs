using Microsoft.VisualStudio.TestTools.UnitTesting;
using GTD.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}