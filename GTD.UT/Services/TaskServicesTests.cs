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

            Task result = taskServices.GetTaskById(1);

            //判断
            Assert.AreEqual(result.Headline, "第一个任务");
        }


        [TestMethod()]
        public void GetTaskNameFromTextTest()
        {
            //准备
            string text1 = "#测试项目 测试任务1";//项目在开头，任务在中间
            string text2 = "测试任务1 #测试项目";//项目在中间，任务在开头
            string text3 = "测试任务1";         //没有项目，只有任务
            string text4 = "";                  //空字符串
            string text5 = "#测试项目";         //只有项目，没有任务


            TaskServices taskServices = new TaskServices();
            //动作
            var taskname1 = taskServices.GetTaskNameFromText(text1);
            var taskname2 = taskServices.GetTaskNameFromText(text2);
            var taskname3 = taskServices.GetTaskNameFromText(text3);
            var taskname4 = taskServices.GetTaskNameFromText(text4);
            var taskname5 = taskServices.GetTaskNameFromText(text5);

            //判断
            Assert.AreEqual(taskname1, "测试任务1");
            Assert.AreEqual(taskname2, "测试任务1");
            Assert.AreEqual(taskname3, "测试任务1");
            Assert.AreEqual(taskname4, null);
            Assert.AreEqual(taskname5, null);

        }

        [TestMethod()]
        public void GetProjectNameFromTextTest()
        {
            //准备
            string text1 = "#测试项目 测试任务1";//项目在开头，任务在中间
            string text2 = "测试任务1 #测试项目";//项目在中间，任务在开头
            string text3 = "测试任务1";         //没有项目，只有任务
            string text4 = "";                  //空字符串
            string text5 = "#测试项目";         //只有项目，没有任务


            TaskServices taskServices = new TaskServices();

            //动作
            var projectname1 = taskServices.GetProjectNameFromText(text1);
            var projectname2 = taskServices.GetProjectNameFromText(text2);
            var projectname3 = taskServices.GetProjectNameFromText(text3);
            var projectname4 = taskServices.GetProjectNameFromText(text4);
            var projectname5 = taskServices.GetProjectNameFromText(text5);

            //判断
            Assert.AreEqual(projectname1, "测试项目");
            Assert.AreEqual(projectname2, "测试项目");
            Assert.AreEqual(projectname3, null);
            Assert.AreEqual(projectname4, null);
            Assert.AreEqual(projectname5, null); //项目名称后没有空格
        }
    }
}


