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
                    TestSuitStarted(suitUid, suitName);

                    foreach (MSTestResult testResult in testResultBySuit.Value)
                    {
                        TestStarted(suitUid, testResult.Name);
                            
                        switch (testResult.Outcome)
                        {
                            case TestOutcome.Completed:
                            case TestOutcome.Passed:
                                TestFinished();
                                break;

                            case TestOutcome.Failed:
                                TestFailed();
                                break;

                            default:
                                throw new Exception("Test result '" + testResult.Outcome.ToString() + "' is not handled.");
                        }
                    }

                    TestSuitFinished(suitUid);
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

        private void TestStarted(string suitId, string name)
        {
            Allure.Lifecycle.Fire(new TestCaseStartedEvent(suitId, name));
        }

        private void TestFinished()
        {
            Allure.Lifecycle.Fire(new TestCaseFinishedEvent());
        }

        private void TestFailed()
        {
            Allure.Lifecycle.Fire(new TestCaseFailureEvent());
        }

        private void TestSuitStarted(string uid, string name)
        {
            Allure.Lifecycle.Fire(new TestSuiteStartedEvent(uid, name));
        }

        private void TestSuitFinished(string uid)
        {
            Allure.Lifecycle.Fire(new TestSuiteFinishedEvent(uid));
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


