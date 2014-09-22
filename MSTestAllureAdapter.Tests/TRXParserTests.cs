using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSTestAllureAdapter.Tests
{
    [TestFixture()]
    public class TRXParserTests
    {
        const int NUMBER_OF_EXPECTED_TESTS = 4;

        private IEnumerable<MSTestResult> mTestResults;

        private IDictionary<string, ICollection<string>> mExpectedTestsCategoriesMap = new Dictionary<string, ICollection<string>>
        { 
            { "TestMethod1", new HashSet<string>{"Category1"} },
            { "TestMethod2", new HashSet<string>{"Category1", "Category2"} },
            { "TestMethod3", new HashSet<string>{"Category2"} },
            { "TestMethod4", new HashSet<string>{TRXParser.DEFAULT_CATEGORY} }
        };

        [SetUp]
        public void SetUp()
        {
            TRXParser parser = new TRXParser();

            mTestResults = parser.GetTestResults("sample.trx");
        }


        [Test]
        public void ExpectedNumberOfTestsWereFound()
        {
            Assert.AreEqual(NUMBER_OF_EXPECTED_TESTS, mTestResults.Count());
        }

        [Test]
        public void AllTestsWereFound()
        {
            IEnumerable<string> expected = mExpectedTestsCategoriesMap.Keys;
            IEnumerable<string> found = mTestResults.Select(testResult => testResult.Name);

            EnumerableDiffResult result = EnumerableDiff(expected, found);

            Assert.AreEqual(0, result.TotalOff, result.Message);
        }

        [Test]
        public void TestCategoriesWereFound()
        {
            foreach (MSTestResult testResult in mTestResults)
            {
                IEnumerable<string> expected = mExpectedTestsCategoriesMap[testResult.Name];
                IEnumerable<string> found = testResult.Suites;
                
                EnumerableDiffResult result = EnumerableDiff(expected, found);

                if (result.TotalOff > 0)
                {
                    Assert.AreEqual(0, result.TotalOff, "Test '" + testResult.Name + "' : "+ Environment.NewLine + result.Message);
                }
            }

        }


        private EnumerableDiffResult EnumerableDiff(IEnumerable<string> expected, IEnumerable<string> found)
        {
            HashSet<string> missing = new HashSet<string>(expected);
            missing.ExceptWith(found);


            HashSet<string> notExpected = new HashSet<string>(found);
            notExpected.ExceptWith(expected);

            string message = String.Empty;

            if (missing.Count > 0)
                {
                    message += "The following items were not found: ";
                    message += String.Join(", ", missing);
                    message += Environment.NewLine;
                }

            if (notExpected.Count > 0)
                {
                    message += "The following items were not expected: ";
                    message += String.Join(", ", notExpected);
                    message += Environment.NewLine;
                }

            return new EnumerableDiffResult(missing.Count + notExpected.Count, message);
        }

        private class EnumerableDiffResult
        {
            public EnumerableDiffResult(int totalOff, string message)
            {
                TotalOff = totalOff;
                Message = message;
            }

            public int TotalOff { get; private set; }

            public string Message { get; private set; }
        }
    }


}

