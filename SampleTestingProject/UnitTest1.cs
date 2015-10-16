using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace SampleTestingProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [TestCategory("Category1")]
        [Owner("Owner1")]
        [Description("Description of the TestMethod1 UnitTest.")] 
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
        public void TestMethod_With_One_Missing_And_One_present_Result_File()
        {
            File.WriteAllText("file.xxx", "file.xxx contents.");
            TestContext.AddResultFile("file.xxx");

            TestContext.AddResultFile("missing.zzz");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod_With_Only_Missing_Result_File()
        {
            TestContext.AddResultFile("missing.zzz");
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod_With_Multiple_Result_Files()
        {
            File.WriteAllText("file.xxx", "file.xxx contents.");
            TestContext.AddResultFile("file.xxx");

            File.WriteAllText("file.yyy", "file.yyy contents.");
            TestContext.AddResultFile("file.yyy");

            TestContext.AddResultFile("missing.zzz");
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

        public TestContext TestContext
        {
            get; 
            set;
        }

        //[DataSource("Microsoft.Visualstudio.TestTools.DataSource.CSV", "|DataDirectory|\\UserData.csv", "UserData#csv", Microsoft.VisualStudio.TestTools.UnitTesting.DataAccessMethod.Sequential)]
        [DataSource("System.Data.OleDb", "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='|DataDirectory|';Extended Properties=\"text;HDR=Yes;FMT=Delimited\"", "UserData#csv", Microsoft.VisualStudio.TestTools.UnitTesting.DataAccessMethod.Sequential)]
        [DeploymentItem("UserData.csv")]
        [TestCategory("Category3")]
        [Owner("Owner3")]
        [TestMethod]
        [Description("Description of the CSVDataDrivenTest data driven test.")]
        public void CSVDataDrivenTest()
        {
            int a = Convert.ToInt32(TestContext.DataRow["Operator 1"]);
            int b = Convert.ToInt32(TestContext.DataRow["Operator 2"]);
            int expected = Convert.ToInt32(TestContext.DataRow["Outcome"]);

            int actual = a / b;
            Assert.AreEqual(expected, actual);
        }
    }
}
