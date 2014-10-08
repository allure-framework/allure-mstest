using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTestingProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [TestCategory("Category1")]
        [Owner("Owner1")]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Category1")]
        [TestCategory("Category2")]
        [Owner("Owner1")]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Owner("Owner1")]
        public void TestMethod3()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Test_Without_Category()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void SimpleFailingTest()
        {
            Assert.AreEqual<int>(1, 2, "The calculation failed.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExpectedException()
        {
            throw new ArgumentOutOfRangeException("value", "This value cannot be set to the specified range.");
        }

        [Owner("Owner2")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Expected ArgumentOutOfRangeException was not fired.")]
        public void ExpectedExceptionWithNoExceptionMessage()
        {
            throw new ArgumentOutOfRangeException("value", "This value cannot be set to the specified range.");
        }

        [Owner("Owner2")]
        [TestMethod]
        public void UnexpectedException()
        {
            throw new Exception();
        }

        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        //[DataSource("Microsoft.Visualstudio.TestTools.DataSource.CSV", "|DataDirectory|\\UserData.csv", "UserData#csv", Microsoft.VisualStudio.TestTools.UnitTesting.DataAccessMethod.Sequential)]
        [DataSource("System.Data.OleDb", "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='|DataDirectory|';Extended Properties=\"text;HDR=Yes;FMT=Delimited\"", "UserData#csv", Microsoft.VisualStudio.TestTools.UnitTesting.DataAccessMethod.Sequential)]
        [DeploymentItem("UserData.csv")]
        [TestCategory("Category3")]
        [Owner("Owner3")]
        [TestMethod]
        public void CSVdataDrivenTest()
        {
            int a = Convert.ToInt32(TestContext.DataRow["Operator 1"]);
            int b = Convert.ToInt32(TestContext.DataRow["Operator 2"]);
            int expected = Convert.ToInt32(TestContext.DataRow["Outcome"]);

            int actual = a / b;
            Assert.AreEqual(expected, actual);
        }
    }
}
