using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace MSTestAllureAdapter.Tests
{
    public abstract class MSTestAllureAdapterTestBase
    {
        public MSTestAllureAdapterTestBase()
        {
            ExpectedTestsResultsMap = new Dictionary<string, MSTestResult>
            { 
                { "TestMethod1", new MSTestResult("", TestOutcome.Passed, "Category1") },
                { "TestMethod2", new MSTestResult("", TestOutcome.Passed, "Category1", "Category2") },
                { "TestMethod3", new MSTestResult("", TestOutcome.Passed, "Category2") },
                { "Test_Without_Category", new MSTestResult("", TestOutcome.Passed) },
                { "SimpleFailingTest", new MSTestResult("", TestOutcome.Failed) },
                { "ExpectedException", new MSTestResult("", TestOutcome.Passed) },
                { "ExpectedExceptionWithNoExceptionMessage", new MSTestResult("", TestOutcome.Passed) },
                { "UnexpectedException", new MSTestResult("", TestOutcome.Failed) }
            };

            ExpectedTestsResultsMap["TestMethod1"].Owner = "Owner1";
            ExpectedTestsResultsMap["TestMethod2"].Owner = "Owner2";
            ExpectedTestsResultsMap["TestMethod3"].Owner = "Owner1";
            ExpectedTestsResultsMap["SimpleFailingTest"].Owner = "OwnerOfFailingTest";
        }

        protected IDictionary<string, MSTestResult> ExpectedTestsResultsMap { get; set; }

        [SetUp]
        public virtual void SetUp()
        {

        }

        [TearDown]
        public virtual void TearDown()
        {

        }

        protected string GetRandomDir()
        {
            string randomDir = Guid.NewGuid().ToString();

            // not chance of that happening...
            while (Directory.Exists(randomDir))
                randomDir = Guid.NewGuid().ToString();

            return randomDir;
        }

        protected string CreateRandomDir()
        {
            string randomDir = GetRandomDir();
            Directory.CreateDirectory(randomDir);
            return randomDir;
        }
    }
}

