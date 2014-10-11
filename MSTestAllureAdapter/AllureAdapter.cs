using System;
using AllureCSharpCommons;
using AllureCSharpCommons.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AllureCSharpCommons.AllureModel;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// The base class for the Allure adapter.
    /// </summary>
    public class AllureAdapter
    {
        static AllureAdapter()
        {
            AllureConfig.AllowEmptySuites = true;
        }

        private TRXParser mTrxParser = new TRXParser();

        private static readonly string EMPTY_SUITE_CATEGORY_NAME = "NO_CATEGORY";

        protected virtual void HandleTestResult(MSTestResult testResult)
        {
            switch (testResult.Outcome)
            {
                case TestOutcome.Failed:
                    TestFailed(testResult);
                    break;
            }
        }

        /// <summary>
        /// Generates the test results from the supplied TRX file to be used by the allure framework.
        /// </summary>
        /// <param name="trxFile">Trx file.</param>
        /// <param name="resultsPath">Results path where the files shuold be saved.</param>
        public void GenerateTestResults(string trxFile, string resultsPath)
        {
            string originalResultsPath = AllureConfig.ResultsPath;

            if (!resultsPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
            {
                resultsPath += System.IO.Path.DirectorySeparatorChar;
            }

            AllureConfig.ResultsPath = resultsPath;

            try
            {
                IEnumerable<MSTestResult> testResults = mTrxParser.GetTestResults(trxFile);

                IDictionary<string, ICollection<MSTestResult>> testsMap = CreateSuitToTestsMap(testResults);

                foreach (KeyValuePair<string, ICollection<MSTestResult>> testResultBySuit in testsMap)
                {
                    string suitUid = Guid.NewGuid().ToString();
                    string suitName = testResultBySuit.Key;
                    ICollection<MSTestResult> tests = testResultBySuit.Value;
                    
                    MSTestResult first = tests.Aggregate((a, b) => a.Start < b.Start ? a : b);
                    MSTestResult last = tests.Aggregate((a, b) => a.End > b.End ? a : b);

                    if (suitName == EMPTY_SUITE_CATEGORY_NAME)
                    {
                        suitName = null;
                    }

                    TestSuitStarted(suitUid, suitName, first.Start);

                    foreach (MSTestResult testResult in testResultBySuit.Value)
                    {
                        TestStarted(suitUid, testResult);
                        
                        HandleTestResult(testResult);

                        TestFinished(testResult);
                    }

                    TestSuitFinished(suitUid, last.End);
                }
            }
            finally
            {
                AllureConfig.ResultsPath = originalResultsPath;
            }
        }

        private IDictionary<string, ICollection<MSTestResult>> CreateSuitToTestsMap(IEnumerable<MSTestResult> testResults )
        {
            // use MultiValueDictionary from the Microsoft Experimental Collections when it becomes available.
            // https://www.nuget.org/packages/Microsoft.Experimental.Collections/
            IDictionary<string, ICollection<MSTestResult>> testsMap = new Dictionary<string, ICollection<MSTestResult>>();

            foreach (MSTestResult testResult in testResults)
            {
                IEnumerable<string> suits = testResult.Suites;

                if (!suits.Any())
                {
                    suits = new string[]{ EMPTY_SUITE_CATEGORY_NAME };
                }

                foreach (string suit in suits)
                {
                    ICollection<MSTestResult> tests = null;

                    if (!testsMap.TryGetValue(suit, out tests))
                    {
                        tests = new List<MSTestResult>();
                        testsMap[suit] = tests;
                    }

                    tests.Add(testResult);
                }
            }

            return testsMap;
        }

        protected virtual void TestStarted(string suitId, MSTestResult testResult)
        {
            TestCaseStartedWithTimeEvent testCase = new TestCaseStartedWithTimeEvent(suitId, testResult.Name, testResult.Start);

            if (testResult.Owner != null)
            {
                label ownerLabel = new label();
                ownerLabel.name = "Owner";
                ownerLabel.value = testResult.Owner;

                testCase.Labels = new label[]{ ownerLabel };

                // allure doesnt support custom labels so until issue #394 is solved
                // the test description is used.
                //
                // https://github.com/allure-framework/allure-core/issues/394

                testCase.Description = new description { type = descriptiontype.text, Value = "Test Owner: " + testResult.Owner };
            }

            Allure.Lifecycle.Fire(testCase);
        }

        protected virtual void TestFinished(MSTestResult testResult)
        {
            Allure.Lifecycle.Fire(new TestCaseFinishedWithTimeEvent(testResult.End));
        }

        protected virtual void TestFailed(MSTestResult testResult)
        {
            Allure.Lifecycle.Fire(new TestCaseFailureWithErrorInfoEvent(testResult.ErrorInfo));
        }

        protected virtual void TestSuitStarted(string uid, string name, DateTime start)
        {
            Allure.Lifecycle.Fire(new TestSuiteStartedWithTimeEvent(uid, name, start));
        }

        protected virtual void TestSuitFinished(string uid, DateTime finished)
        {
            Allure.Lifecycle.Fire(new TestSuiteFinishedWithTimeEvent(uid, finished));
        }
    }
}
