using System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestMockToday
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int fixedYear = 2000;

            using (ShimsContext.Create())
            {
                // Arrange:  
                // Detour DateTime.Now to return a fixed date:  
                System.Fakes.ShimDateTime.NowGet =
                () => new DateTime(fixedYear, 1, 1);

                var d = DateTime.Now;
                // This will always be true if the component is working:  
                Assert.AreEqual(fixedYear, d.Year);
            }
            var z = DateTime.Now;
            Assert.AreEqual(2016,z.Year);

        }
    }
}
