using System;
using AllureCSharpCommons;
using AllureCSharpCommons.Events;
using System.Collections.Generic;
using System.Linq;

namespace MSTestAllureAdapter
{
    public class AllureAdapter
    {
        private TRXParser mTrxParser = new TRXParser();

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
                            
                        switch (testResult.Outcome)
                        {
                            case TestOutcome.Completed:
                            case TestOutcome.Passed:
                                        TestFinished(testResult.End);
                                break;

                            case TestOutcome.Failed:
                                        TestFailed(testResult.End);
                                break;

                            default:
                                throw new Exception("Test result '" + testResult.Outcome.ToString() + "' is not handled.");
                        }
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

        protected virtual void TestFailed(DateTime finished)
        {
            Allure.Lifecycle.Fire(new TestCaseFailureWithTimeEvent(finished));
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

    class SuitWithTest
    {
        public SuitWithTest(string suitName, MSTestResult test)
        {
            SuitName = suitName;
            Test = test;
        }

        public string SuitName { get; private set; }

        public MSTestResult Test { get; private set; }
    }
}


