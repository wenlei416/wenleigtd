using System;
using System.Diagnostics;
using System.Web;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GTD.UT
{
    [TestClass]
    public class MockTech
    {
        private Mock<HttpContextBase> moqContext;
        private Mock<HttpRequestBase> moqRequest;
        [TestMethod]
        public void FakeToday()
        {
            using (ShimsContext.Create())
            {
                // Arrange:  
                System.Fakes.ShimDateTime.NowGet =
                () => new DateTime(2000, 1, 1);

                // Do:
                var d = DateTime.Now;

                // Assert:
                Assert.AreEqual(2000, d.Year);
            }
        }

        [TestMethod]
        public void MockCookie()
        {
            moqContext = new Mock<HttpContextBase>();
            moqRequest = new Mock<HttpRequestBase>();

            moqContext.Setup(x => x.Request).Returns(moqRequest.Object);
            moqRequest.Setup(r => r.Cookies)
                .Returns(new HttpCookieCollection() {new HttpCookie("lastCreateRepeatTaskDate", "2016-11-12")});

            var mookCooike = new HttpCookie("lastCreateRepeatTaskDate", "2016-11-12");

            var requestCookie = moqContext.Object.Request.Cookies["lastCreateRepeatTaskDate"];
            Assert.AreEqual(requestCookie.Value, mookCooike.Value);

        }
    }
}
