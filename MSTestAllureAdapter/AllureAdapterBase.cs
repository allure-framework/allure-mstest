using System;
using AllureCSharpCommons;
using AllureCSharpCommons.Events;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSTestAllureAdapter
{
    public abstract class AllureAdapterBase
    {
        private TRXParser mTrxParser = new TRXParser();

        protected abstract void HandleTestResult(MSTestResult testResult);

        public void Run(string trxFile, string resultsPath)
        {
            string originalResultsPath = AllureConfig.ResultsPath;

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

                    TestSuitStarted(suitUid, suitName, first.Start);

                    foreach (MSTestResult testResult in testResultBySuit.Value)
                    {
                        TestStarted(suitUid, testResult.Name, testResult.Start);
                        
                        HandleTestResult(testResult);

                        TestFinished(testResult.End);
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
            IDictionary<string, ICollection<MSTestResult>> testsMap = new Dictionary<string, ICollection<MSTestResult>>();

            foreach (MSTestResult testResult in testResults)
            {
                foreach (string suit in testResult.Suites)
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

        protected virtual void TestStarted(string suitId, string name, DateTime started)
        {
            Allure.Lifecycle.Fire(new TestCaseStartedWithTimeEvent(suitId, name, started));
        }

        protected virtual void TestFinished(DateTime finished)
        {
            Allure.Lifecycle.Fire(new TestCaseFinishedWithTimeEvent(finished));
        }

        protected virtual void TestFailed(ErrorInfo errorInfo)
        {
            Allure.Lifecycle.Fire(new TestCaseFailureWithErrorInfoEvent(errorInfo));
        }

        protected virtual void TestSuitStarted(string uid, string name, DateTime started)
        {
            Allure.Lifecycle.Fire(new TestSuiteStartedWithTimeEvent(uid, name, started));
        }

        protected virtual void TestSuitFinished(string uid, DateTime finished)
        {
            Allure.Lifecycle.Fire(new TestSuiteFinishedWithTimeEvent(uid, finished));
        }
    }
}
