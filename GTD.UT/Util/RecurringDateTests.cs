using System;
using GTD.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTD.UT.Util.Tests
{
    [TestClass()]
    public class RecurringDateTests
    {
        //全部循环日期都在范围内
        [TestMethod()]
        public void RecurringJsonToDateYear1Test()
        {
            const string cyc = @"{'cyear':'1','cmonth':'0','cweek':'0','cday':'0','startday':'2016-1-2','endday':'2018-4-5','cyc':'12'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 1, 12), a[0]);
            Assert.AreEqual(new DateTime(2017, 1, 12), a[1]);
            Assert.AreEqual(new DateTime(2018, 1, 12), a[2]);
        }

        //闰年
        [TestMethod]
        public void RecurringJsonToDateYear2Test()
        {
            const string cyc = @"{'cyear':'1','cmonth':'0','cweek':'0','cday':'0','startday':'2016-1-2','endday':'2024-4-5','cyc':'366'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 2, 29), a[0]);
            Assert.AreEqual(new DateTime(2017, 2, 28), a[1]);
            Assert.AreEqual(new DateTime(2018, 2, 28), a[2]);
            Assert.AreEqual(new DateTime(2019, 2, 28), a[3]);
            Assert.AreEqual(new DateTime(2020, 2, 28), a[4]);
            //Assert.AreEqual(new DateTime(2020, 2, 29), a[8]);
        }
        //所有循环日期都不在范围内
        [TestMethod]
        public void RecurringJsonToDateYear3Test()
        {
            const string cyc = @"{'cyear':'1','cmonth':'0','cweek':'0','cday':'0','startday':'2016-10-2','endday':'2016-12-15','cyc':'20'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(a.Count,0);
        }

        //头尾不在
        [TestMethod]
        public void RecurringJsonToDateYear4Test()
        {
            const string cyc = @"{'cyear':'1','cmonth':'0','cweek':'0','cday':'0','startday':'2016-10-1','endday':'2019-2-1','cyc':'100'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2017, 4, 10), a[0]);
            Assert.AreEqual(new DateTime(2018, 4, 10), a[1]);
        }
    }
}
