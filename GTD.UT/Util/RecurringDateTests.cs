using System;
using GTD.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTD.UT.Util.Tests
{
    [TestClass()]
    public class RecurringDateTests
    {

        #region 针对年的测试
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

            Assert.AreEqual(a.Count, 0);
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
        #endregion

        #region 针对日的测试

        //全部在范围内
        [TestMethod]
        public void RecurringJsonToDateDay1Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-10-1','endday':'2016-10-5','cyc':'100'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 10, 1), a[0]);
            Assert.AreEqual(new DateTime(2016, 10, 2), a[1]);
            Assert.AreEqual(new DateTime(2016, 10, 3), a[2]);
            Assert.AreEqual(new DateTime(2016, 10, 4), a[3]);
            Assert.AreEqual(new DateTime(2016, 10, 5), a[4]);
            Assert.AreEqual(a.Count, 5);
        }

        //第二次超出
        [TestMethod]
        public void RecurringJsonToDateDay2Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'0','cday':'5','startday':'2016-10-1','endday':'2016-10-6','cyc':'100'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 10, 1), a[0]);
            Assert.AreEqual(new DateTime(2016, 10, 6), a[1]);
            Assert.AreEqual(a.Count, 2);
        }


        #endregion

        #region 针对周的测试

        //全部在范围内
        [TestMethod]
        public void RecurringJsonToDateWeek1Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'1','cday':'0','startday':'2016-11-9','endday':'2016-11-25','cyc':'4'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 10), a[0]);
            Assert.AreEqual(new DateTime(2016, 11, 17), a[1]);
            Assert.AreEqual(new DateTime(2016, 11, 24), a[2]);
            Assert.AreEqual(a.Count, 3);
        }

        //头尾都超出
        [TestMethod]
        public void RecurringJsonToDateWeek2Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'1','cday':'0','startday':'2016-11-9','endday':'2016-11-25','cyc':'2'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 15), a[0]);
            Assert.AreEqual(new DateTime(2016, 11, 22), a[1]);
            Assert.AreEqual(a.Count, 2);
        }

        //周日
        [TestMethod]
        public void RecurringJsonToDateWeek3Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'1','cday':'0','startday':'2016-11-9','endday':'2016-11-25','cyc':'0'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 13), a[0]);
            Assert.AreEqual(new DateTime(2016, 11, 20), a[1]);
            Assert.AreEqual(a.Count, 2);
        }

        //全部在范围外
        [TestMethod]
        public void RecurringJsonToDateWeek4Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'0','cweek':'1','cday':'0','startday':'2016-11-9','endday':'2016-11-12','cyc':'0'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(a.Count, 0);
        }


        #endregion

        #region 针对月的测试

        //最正常的情况，全部在范围内，前后有超出，日期小于等于28
        [TestMethod]
        public void RecurringJsonToDateMonth1Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'2','cweek':'0','cday':'0','startday':'2016-2-1','endday':'2016-12-5','cyc':'28'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 2, 28), a[0]);
            Assert.AreEqual(new DateTime(2016, 4, 28), a[1]);
            Assert.AreEqual(new DateTime(2016, 6, 28), a[2]);
            Assert.AreEqual(new DateTime(2016, 8, 28), a[3]);
            Assert.AreEqual(new DateTime(2016, 10, 28), a[4]);
            Assert.AreEqual(a.Count, 5);
        }

        //31，最后一天，包括2月（闰年），包括了跨年的场景
        [TestMethod]
        public void RecurringJsonToDateMonth2Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2016-1-1','endday':'2017-4-6','cyc':'31'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 1, 31), a[0]);
            Assert.AreEqual(new DateTime(2016, 2, 29), a[1]);
            Assert.AreEqual(new DateTime(2016, 3, 31), a[2]);
            Assert.AreEqual(new DateTime(2016, 4, 30), a[3]);
            Assert.AreEqual(new DateTime(2016, 5, 31), a[4]);
            Assert.AreEqual(new DateTime(2016, 6, 30), a[5]);
            Assert.AreEqual(new DateTime(2016, 7, 31), a[6]);
            Assert.AreEqual(new DateTime(2016, 8, 31), a[7]);
            Assert.AreEqual(new DateTime(2016, 9, 30), a[8]);
            Assert.AreEqual(new DateTime(2016, 10, 31), a[9]);
            Assert.AreEqual(new DateTime(2016, 11, 30), a[10]);
            Assert.AreEqual(new DateTime(2016, 12, 31), a[11]);
            Assert.AreEqual(new DateTime(2017, 1, 31), a[12]);
            Assert.AreEqual(new DateTime(2017, 2, 28), a[13]);
            Assert.AreEqual(new DateTime(2017, 3, 31), a[14]);

            Assert.AreEqual(a.Count, 15);
        }

        //cyc是29,30
        [TestMethod]
        public void RecurringJsonToDateMonth3Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2015-10-1','endday':'2016-4-5','cyc':'30'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2015, 10, 30), a[0]);
            Assert.AreEqual(new DateTime(2015, 11, 30), a[1]);
            Assert.AreEqual(new DateTime(2015, 12, 30), a[2]);
            Assert.AreEqual(new DateTime(2016, 1, 30), a[3]);
            Assert.AreEqual(new DateTime(2016, 2, 29), a[4]);
            Assert.AreEqual(new DateTime(2016, 3, 30), a[5]);
            Assert.AreEqual(a.Count,6);
        }

        //502，最后一周的场景
        [TestMethod]
        public void RecurringJsonToDateMonth4Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2016-11-1','endday':'2017-4-30','cyc':'503'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 30), a[0]);
            Assert.AreEqual(new DateTime(2016, 12, 28), a[1]);
            Assert.AreEqual(new DateTime(2017, 1, 25), a[2]);
            Assert.AreEqual(new DateTime(2017, 2, 22), a[3]);
            Assert.AreEqual(new DateTime(2017, 3, 29), a[4]);
            Assert.AreEqual(new DateTime(2017, 4, 26), a[5]);

            Assert.AreEqual(a.Count, 6);
        }

        //102，第一周的场景
        [TestMethod]
        public void RecurringJsonToDateMonth5Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2016-11-1','endday':'2017-4-30','cyc':'102'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 1), a[0]);
            Assert.AreEqual(new DateTime(2016, 12, 6), a[1]);
            Assert.AreEqual(new DateTime(2017, 1, 3), a[2]);
            Assert.AreEqual(new DateTime(2017, 2, 7), a[3]);
            Assert.AreEqual(new DateTime(2017, 3, 7), a[4]);
            Assert.AreEqual(new DateTime(2017, 4, 4), a[5]);

            Assert.AreEqual(a.Count, 6);
        }

        //202，第2周的场景
        [TestMethod]
        public void RecurringJsonToDateMonth6Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2016-11-1','endday':'2017-4-30','cyc':'202'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 8), a[0]);
            Assert.AreEqual(new DateTime(2016, 12, 13), a[1]);
            Assert.AreEqual(new DateTime(2017, 1, 10), a[2]);
            Assert.AreEqual(new DateTime(2017, 2, 14), a[3]);
            Assert.AreEqual(new DateTime(2017, 3, 14), a[4]);
            Assert.AreEqual(new DateTime(2017, 4, 11), a[5]);

            Assert.AreEqual(a.Count, 6);
        }

        //402，第4周的场景
        [TestMethod]
        public void RecurringJsonToDateMonth7Test()
        {
            const string cyc = @"{'cyear':'0','cmonth':'1','cweek':'0','cday':'0','startday':'2016-11-1','endday':'2017-4-30','cyc':'402'}";
            var a = RecurringDate.RecurringJsonToDate(cyc);

            Assert.AreEqual(new DateTime(2016, 11, 22), a[0]);
            Assert.AreEqual(new DateTime(2016, 12, 27), a[1]);
            Assert.AreEqual(new DateTime(2017, 1, 24), a[2]);
            Assert.AreEqual(new DateTime(2017, 2, 28), a[3]);
            Assert.AreEqual(new DateTime(2017, 3, 28), a[4]);
            Assert.AreEqual(new DateTime(2017, 4, 25), a[5]);


            Assert.AreEqual(a.Count, 6);
        }

        #endregion

    }
}
