using System;
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

        //t + p + c + d + p + c + d 存在重复的@#
        [TestMethod()]
        public void GetTaskInfoFromText_Scense02()
        {
            //准备
            string input = _taskName +
                           " " + "#" + _projectName + " " + "@" + _contextName + " " + _taskDescripiton +
                           " " + "#" + _projectName + " " + "@" + _contextName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton + " " + _taskDescripiton, result["taskDescription"]);
        }

        //@#前面没空格
        [TestMethod()]
        public void GetTaskInfoFromText_Scense01()
        {
            //准备
            string input = _taskName +
                           "#" + _taskDescripiton + " " + "@" + _contextName + " " + _taskDescripiton +
                           " " + "#" + _projectName + "@" + _taskDescripiton + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName + "@" + _taskDescripiton, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName + "#" + _taskDescripiton, result["taskName"]);
            Assert.AreEqual(_taskDescripiton + " " + _taskDescripiton, result["taskDescription"]);
        }

        #region project打头
        //p + c + t
        [TestMethod()]
        public void GetTaskInfoFromText_Scense03()
        {
            //准备
            string input = "#" + _projectName + " " + "@" + _contextName + " " + _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescripiton"));
        }

        //p + t + c + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense04()
        {
            //准备
            string input = "#" + _projectName + " " + _taskName + " " + "@" + _contextName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }

        //p + c     不存在
        [TestMethod()]
        public void GetTaskInfoFromText_Scense05()
        {
            //准备
            string input = "#" + _projectName + " " + "@" + _contextName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(false, result.ContainsKey("taskName"));
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }

        //p + t 
        [TestMethod()]
        public void GetTaskInfoFromText_Scense06()
        {
            //准备
            string input = "#" + _projectName + " " + _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(false, result.ContainsKey("contextName"));
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }


        //p 不存在
        [TestMethod()]
        public void GetTaskInfoFromText_Scense07()
        {
            //准备
            string input = "#" + _projectName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(false, result.ContainsKey("contextName"));
            Assert.AreEqual(false, result.ContainsKey("taskName"));
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }
        #endregion


        #region taskname打头
        //t + p + c + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense08()
        {
            //准备
            string input = _taskName + " " + "#" + _projectName + " " + "@" + _contextName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }

        //t + p + d + c + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense09()
        {
            //准备
            string input = _taskName + " " + "#" + _projectName + " " + _taskDescripiton + " " + "@" + _contextName +
                           " " + _t1;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton + " " + _t1, result["taskDescription"]);
        }

        //t + c + p + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense10()
        {
            //准备
            string input = _taskName + " " + "@" + _contextName + " " + "#" + _projectName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }

        //t + c + d + p + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense11()
        {
            //准备
            string input = _taskName + " " + "@" + _contextName + " " + _taskDescripiton + " " + "#" + _projectName +
                           " " + _t1;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton + " " + _t1, result["taskDescription"]);
        }

        //t + c 
        [TestMethod()]
        public void GetTaskInfoFromText_Scense12()
        {
            //准备
            string input = _taskName + " " + "@" + _contextName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(false, result.ContainsKey("projectName"));
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }

        //t + c + d 
        [TestMethod()]
        public void GetTaskInfoFromText_Scense13()
        {
            //准备
            string input = _taskName + " " + "@" + _contextName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(false, result.ContainsKey("projectName"));
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);

        }

        //t + p 
        [TestMethod()]
        public void GetTaskInfoFromText_Scense14()
        {
            //准备
            string input = _taskName + " " + "#" + _projectName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(false, result.ContainsKey("contextName"));
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));

        }
        //t + p + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense15()
        {
            //准备
            string input = _taskName + " " + "#" + _projectName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(false, result.ContainsKey("contextName"));
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);

        }

        //t
        [TestMethod()]
        public void GetTaskInfoFromText_Scense16()
        {
            //准备
            string input = _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(false, result.ContainsKey("projectName"));
            Assert.AreEqual(false, result.ContainsKey("contextName"));
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));

        }
        #endregion

        #region context打头
        //c + p + t
        [TestMethod()]
        public void GetTaskInfoFromText_Scense17()
        {
            //准备
            string input = "@" + _contextName + " " + "#" + _projectName + " " + _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }

        //c + t + p + d
        [TestMethod()]
        public void GetTaskInfoFromText_Scense18()
        {
            //准备
            string input = "@" + _contextName + " " + _taskName + " " + "#" + _projectName + " " + _taskDescripiton;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }

        //c + t 
        [TestMethod()]
        public void GetTaskInfoFromText_Scense19()
        {
            //准备
            string input = "@" + _contextName + " " + _taskName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(false, result.ContainsKey("projectName"));
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }

        //c + p  不存在
        [TestMethod()]
        public void GetTaskInfoFromText_Scense20()
        {
            //准备
            string input = "@" + _contextName + " " + "#" + _projectName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(false, result.ContainsKey("taskName"));
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }

        //c 不存在
        [TestMethod()]
        public void GetTaskInfoFromText_Scense21()
        {
            //准备
            string input = "@" + _contextName ;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(false, result.ContainsKey("projectName"));
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(false, result.ContainsKey("taskName"));
            Assert.AreEqual(false, result.ContainsKey("taskDescription"));
        }
        #endregion

        //t + p +  d + p + c  存在重复的@#
        [TestMethod()]
        public void GetTaskInfoFromText_Scense22()
        {
            //准备
            string input = _taskName +
                           " " + "#" + _projectName + " " + _taskDescripiton +
                           " " + "#" + _t1 + " " + "@" + _contextName;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }
        //t + p +  d + c + p  存在重复的@#
        [TestMethod()]
        public void GetTaskInfoFromText_Scense23()
        {
            //准备
            string input = _taskName +
                           " " + "#" + _projectName + " " + _taskDescripiton +
                           " " + "@" + _contextName+ " " + "#" + _t1;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }

        //t + p + d + c + c  存在重复的@#
        [TestMethod()]
        public void GetTaskInfoFromText_Scense24()
        {
            //准备
            string input = _taskName +
                           " " + "#" + _projectName + " " + _taskDescripiton +
                           " " + "@" + _contextName + " " + "@" + _t1;
            //动作
            var result = TaskUtil.GetTaskInfoFromText(input);
            //断言
            Assert.AreEqual(_projectName, result["projectName"]);
            Assert.AreEqual(_contextName, result["contextName"]);
            Assert.AreEqual(_taskName, result["taskName"]);
            Assert.AreEqual(_taskDescripiton, result["taskDescription"]);
        }


    }
}
