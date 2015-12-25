using System;
using System.Linq;
using GTD.DAL.Abstract;
using GTD.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GTD.Services;
using GTD.Services.Abstract;
using Moq;


namespace GTD.UT.Services
{
    [TestClass()]
    public class TaskServicesTests
    {

        [TestMethod()]
        public void GetTaskByIdTest()
        {
            //准备
            Mock<ITaskRepository> mock = new Mock<ITaskRepository>();
            mock.Setup(m => m.GetTaskById(1)).Returns(new Task
            {
                TaskId = 1,
                Headline = "第一个任务"
            });
            mock.Setup(m => m.GetTaskById(2)).Returns(new Task
            {
                TaskId = 2,
                Headline = "第2个任务"
            });
               
            ITaskServices taskServices=new TaskServices(mock.Object);

            //动作

            Task result = taskServices.GetTaskById(2);

            //判断
            Assert.AreEqual(result.Headline, "第一个任务");
        }
    }
}


