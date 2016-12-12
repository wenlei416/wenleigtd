using GTD.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTD.UT.Util
{
    [TestClass]
    public class TaskUtilTestsGetTaskInfoFromText
    {
        private string _projectName;
        private string _contextName;
        private string _taskName;
        private string _taskDescripiton;
        private string _t1, _t2, _t3;

        [TestInitialize]
        public void Init()
        {
            _projectName = "项目名称";
            _contextName = "场景名称";
            _taskName = "任务名称1 任务名称2";
            _taskDescripiton = "任务描述";
            _t1 = "穿插文本1";
            _t2 = "穿插文本2";
            _t3 = "穿插文本3";

        }
        //p + c + t
        [TestMethod()]
        public void GetTaskInfoFromText_Scense1()
        {
            //准备
            string input = "#" + _projectName + " " + "@" + _contextName + " " + _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName,result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescripiton"));
        }
        //p + t + c + d
        //p + c 
        //p + t 
        //p + d 不存在
        //p + d + c + t 不存在
        //p 不存在

        //t + p + c + d
        //t + p + d + c + d
        //t + c + p + d
        //t + c + d + p + d
        //t + c 
        //t + c + d 
        //t + p 
        //t + p + d
        //t



        //c + p + t
        //c + t + p + d
        //c + p 
        //c + t 
        //c + d 不存在
        //c + d + p + t 不存在
        //c 不存在


        //t + p + c + d + p + c + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense2()
        {
            //准备
            string input = _taskName + " #" + _projectName + " " + "@" + _contextName + " " + _taskDescripiton +
                           " #" + _projectName + " " + "@" + _contextName + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescripiton"));
        }

        //@#前面没空格

    }
}
