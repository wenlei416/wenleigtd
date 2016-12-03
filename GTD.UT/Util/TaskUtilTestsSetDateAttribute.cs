using System;
using GTD.Models;
using GTD.Util;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTD.UT.Util
{
    [TestClass]
    public class TaskUtilTestsSetDateAttribute
    {
        /* 测试场景
         * 开始时间：今天           属性：任意         项目：任意   今日待办
         * 开始时间：明天           属性：任意         项目：任意   明日待办
         * 开始时间：今天/明天以外  属性：任意         项目：任意   日程
         * 开始时间：无             属性：除将来/等待  项目：有     下一步行动
         * 开始时间：无             属性：将来也许     项目：任意   将来也许
         * 开始时间：无             属性：等待         项目：任意   等待
         * 开始时间：无             属性：除将来/等待/下一步  项目：无     收集箱
         * DateTime? star, DateAttribute? att, int? projectid
         */
        private DateTime? Today { get; set; }
        private DateTime? Tommorow { get; set; }
        private DateTime? Feature { get; set; }
        private DateAttribute? Att { get; set; }
        private int? Projectid { get; set; }
        [TestInitialize]
        public void Init()
        {
            Today = new DateTime(2016, 11, 12).Date;
            Tommorow = new DateTime(2016, 11, 13).Date;
            Feature = new DateTime(2016, 11, 15).Date;
            Projectid = 1;
        }

        // 开始时间：今天 属性：任意 项目：任意   今日待办
        [TestMethod]
        public void Scense1()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(Today, DateAttribute.明日待办, null);
                var result2 = TaskUtil.SetDateAttribute(Today, DateAttribute.明日待办, Projectid);
                var result3 = TaskUtil.SetDateAttribute(Today, null, null);
                var result4 = TaskUtil.SetDateAttribute(Today, null, Projectid);

                // Assert:
                Assert.AreEqual(result1, DateAttribute.今日待办);
                Assert.AreEqual(result2, DateAttribute.今日待办);
                Assert.AreEqual(result3, DateAttribute.今日待办);
                Assert.AreEqual(result4, DateAttribute.今日待办);
            }
        }

        // 开始时间：明天 属性：任意 项目：任意 明日待办
        [TestMethod]
        public void Scense2()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(Tommorow, DateAttribute.今日待办, null);
                var result2 = TaskUtil.SetDateAttribute(Tommorow, DateAttribute.将来也许, Projectid);
                var result3 = TaskUtil.SetDateAttribute(Tommorow, null, null);
                var result4 = TaskUtil.SetDateAttribute(Tommorow, null, Projectid);

                // Assert:
                Assert.AreEqual(result1, DateAttribute.明日待办);
                Assert.AreEqual(result2, DateAttribute.明日待办);
                Assert.AreEqual(result3, DateAttribute.明日待办);
                Assert.AreEqual(result4, DateAttribute.明日待办);
            }
        }

        // 开始时间：今天/明天以外 属性：任意 项目：任意 日程
        [TestMethod]
        public void Scense3()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(Feature, DateAttribute.今日待办, null);
                var result2 = TaskUtil.SetDateAttribute(Feature, DateAttribute.收集箱, Projectid);
                var result3 = TaskUtil.SetDateAttribute(Feature, null, null);
                var result4 = TaskUtil.SetDateAttribute(Feature, null, Projectid);

                // Assert:
                Assert.AreEqual(result1, DateAttribute.日程);
                Assert.AreEqual(result2, DateAttribute.日程);
                Assert.AreEqual(result3, DateAttribute.日程);
                Assert.AreEqual(result4, DateAttribute.日程);
            }
        }

        // 开始时间：无 属性：除将来/等待 项目：有 下一步行动
        [TestMethod]
        public void Scense4()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(null, DateAttribute.今日待办, Projectid);
                var result2 = TaskUtil.SetDateAttribute(null, DateAttribute.收集箱, Projectid);
                var result3 = TaskUtil.SetDateAttribute(null, DateAttribute.下一步行动, null);
                var result4 = TaskUtil.SetDateAttribute(null, null, Projectid);


                // Assert:
                Assert.AreEqual(result1, DateAttribute.下一步行动);
                Assert.AreEqual(result2, DateAttribute.下一步行动);
                Assert.AreEqual(result3, DateAttribute.下一步行动);
                Assert.AreEqual(result4, DateAttribute.下一步行动);
            }
        }

        // 开始时间：无 属性：将来也许 项目：任意 将来也许
        [TestMethod]
        public void Scense5()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(null, DateAttribute.将来也许, Projectid);
                var result2 = TaskUtil.SetDateAttribute(null, DateAttribute.将来也许, null);

                // Assert:
                Assert.AreEqual(result1, DateAttribute.将来也许);
                Assert.AreEqual(result2, DateAttribute.将来也许);
            }
        }

        // 开始时间：无 属性：等待 项目：任意 等待
        [TestMethod]
        public void Scense6()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet = () => new DateTime(2016, 11, 12);

                // Do:
                var result1 = TaskUtil.SetDateAttribute(null, DateAttribute.等待, Projectid);
                var result2 = TaskUtil.SetDateAttribute(null, DateAttribute.等待, null);

                // Assert:
                Assert.AreEqual(DateAttribute.等待, result1);
                Assert.AreEqual(DateAttribute.等待, result2);
            }
        }

    }
}
