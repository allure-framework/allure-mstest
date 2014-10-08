using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SampleTestingProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [TestCategory("Catehory1")]
        [Owner("Owner1")]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Catehory2")]
        [Owner("Owner1")]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Catehory1")]
        [TestCategory("Catehory2")]
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

        [TestCategory("Catehory3")]
        [Owner("Owner2")]
        [TestMethod]
        public void SimpleFailingTest()
        {
            Assert.AreEqual<int>(1, 2, "The calculation failed.");
        }

        [TestCategory("Catehory3")]
        [Owner("Owner2")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ExpectedException()
        {
            throw new ArgumentOutOfRangeException("value", "This value cannot be set to the specified range.");
        }

        [TestCategory("Catehory3")]
        [Owner("Owner2")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "Expected ArgumentOutOfRangeException was not fired.")]
        public void ExpectedExceptionWithNoExceptionMessage()
        {
            throw new ArgumentOutOfRangeException("value", "This value cannot be set to the specified range.");
        }

        [TestCategory("Catehory3")]
        [Owner("Owner2")]
        [TestMethod]
        public void UnexpectedException()
        {
            throw new Exception();
        }
    }
}
