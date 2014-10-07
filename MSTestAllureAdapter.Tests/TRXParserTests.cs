using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSTestAllureAdapter.Tests
{
    [TestFixture]
    public class TRXParserTests : MSTestAllureAdapterTestBase
    {
        private IEnumerable<MSTestResult> mTestResults;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            TRXParser parser = new TRXParser();

            mTestResults = parser.GetTestResults("sample.trx");
        }

        [Test]
        public void ExpectedNumberOfTestsWereFound()
        {
            Assert.AreEqual(ExpectedTestsResultsMap.Keys.Count, mTestResults.Count());
        }

        [Test]
        public void AllTestsWereFound()
        {
            IEnumerable<string> expected = ExpectedTestsResultsMap.Keys;
            IEnumerable<string> found = mTestResults.Select(testResult => testResult.Name);

            EnumerableDiffResult result = EnumerableDiff(expected, found);

            Assert.AreEqual(0, result.TotalOff, result.Message);
        }

        [Test]
        public void TestCategoriesWereFound()
        {
            foreach (MSTestResult testResult in mTestResults)
            {
                IEnumerable<string> expected = ExpectedTestsResultsMap[testResult.Name].Suites;
                IEnumerable<string> found = testResult.Suites;
                
                EnumerableDiffResult result = EnumerableDiff(expected, found);

                if (result.TotalOff > 0)
                {
                    Assert.AreEqual(0, result.TotalOff, "Test '" + testResult.Name + "' : "+ Environment.NewLine + result.Message);
                }
            }
        }

        [Test]
        public void TestOutcomesMatch()
        {
            foreach (MSTestResult testResult in mTestResults)
            {
                TestOutcome expected = ExpectedTestsResultsMap[testResult.Name].Outcome;
                TestOutcome actual = testResult.Outcome;

                if (expected != actual)
                {
                    Assert.Fail("Test '" + testResult.Name + "' was found to have an outcome of " + actual + " instead of the expected " + expected);
                }
            }
        }

        [Test]
        public void TestOwnersWereFound()
        {
            foreach (MSTestResult testResult in mTestResults)
            {
                string expected = ExpectedTestsResultsMap[testResult.Name].Owner;
                string actual = testResult.Owner;

                if (String.Compare(expected, actual, true) != 0)
                {
                    Assert.Fail("Test '" + testResult.Name + "' owner was found to be '" + actual + "' instead of the expected '" + expected + "'");
                }
            }
        }


        private EnumerableDiffResult EnumerableDiff(IEnumerable<string> expected, IEnumerable<string> found)
        {
            if (expected == null)
                expected = Enumerable.Empty<string>();

            if (found == null)
                found = Enumerable.Empty<string>();

            HashSet<string> missing = new HashSet<string>(expected);
            missing.ExceptWith(found);


            HashSet<string> notExpected = new HashSet<string>(found);
            notExpected.ExceptWith(expected);

            string message = String.Empty;

            if (missing.Count > 0)
            {
                message += "The following items were not found: ";
                message += String.Join(", ", missing.Select( _ => "'" + _ + "'"));
                message += Environment.NewLine;
            }

            if (notExpected.Count > 0)
            {
                message += "The following items were not expected: ";
                message += String.Join(", ", notExpected.Select( _ => "'" + _ + "'"));
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

