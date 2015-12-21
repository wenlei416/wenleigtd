using System.Linq;
using GTD.Services.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GTD.Services.Tests
{
    [TestClass()]
    public class TaskServicesTests
    {
        [TestMethod()]
        public void GetCompletedTasksTest()
        {
            //GTDContext db = new GTDContext();
            ITaskServices taskServices = new TaskServices();
            var tasks=taskServices.GetInProgressTasks();
            //var project=db.Projects.Find(3);
            //System.Console.WriteLine(project.ProjectName);
            Assert.AreEqual(tasks.Count(), 2);
        }


    }
}
