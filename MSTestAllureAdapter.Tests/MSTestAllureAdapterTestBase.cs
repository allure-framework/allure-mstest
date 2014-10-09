using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace MSTestAllureAdapter.Tests
{
    public abstract class MSTestAllureAdapterTestBase
    {
        public MSTestAllureAdapterTestBase()
        {
            MSTestResult[] testResults = new [] {
                new MSTestResult("TestMethod1", TestOutcome.Passed, "Category1"),
                new MSTestResult("TestMethod2", TestOutcome.Passed, "Category1", "Category2"),
                new MSTestResult("TestMethod3", TestOutcome.Passed, "Category2"),
                new MSTestResult("Test_Without_Category", TestOutcome.Passed),
                new MSTestResult("SimpleFailingTest", TestOutcome.Failed),
                new MSTestResult("ExpectedException", TestOutcome.Passed),
                new MSTestResult("ExpectedExceptionWithNoExceptionMessage", TestOutcome.Passed),
                new MSTestResult("UnexpectedException", TestOutcome.Failed),
                new MSTestResult("CSVdataDrivenTest0", TestOutcome.Passed, "Category3"),
                new MSTestResult("CSVdataDrivenTest1", TestOutcome.Passed, "Category3"),
                new MSTestResult("CSVdataDrivenTest2", TestOutcome.Failed, "Category3")
            };

            ExpectedTestsResultsMap = CreateMap(testResults);

            ExpectedTestsResultsMap["TestMethod1"].Owner = "Owner1";
            ExpectedTestsResultsMap["TestMethod2"].Owner = "Owner1";
            ExpectedTestsResultsMap["TestMethod3"].Owner = "Owner1";
            ExpectedTestsResultsMap["UnexpectedException"].Owner = "Owner2";
            ExpectedTestsResultsMap["ExpectedExceptionWithNoExceptionMessage"].Owner = "Owner2";
            ExpectedTestsResultsMap["CSVdataDrivenTest0"].Owner = "Owner3";
            ExpectedTestsResultsMap["CSVdataDrivenTest1"].Owner = "Owner3";
            ExpectedTestsResultsMap["CSVdataDrivenTest2"].Owner = "Owner3";
        }

        private IDictionary<string, MSTestResult> CreateMap(IEnumerable<MSTestResult> testResults)
        {
            return testResults.ToDictionary<MSTestResult, string>(x => x.Name);
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

