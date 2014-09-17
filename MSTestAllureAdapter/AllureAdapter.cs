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

        public void Run(string trxFile)
		{
            IEnumerable<MSTestResult> testResults = mTrxParser.GetTestResults(trxFile);

            IEnumerable<string> suits = testResults.SelectMany<MSTestResult, string>(testResult => testResult.Suites).Distinct();

            var testResultsWithSuit = 
                from suit in suits
                            from testResult in testResults
                            where testResult.Suites.Contains<string>(suit)
                            select new 
                                    {
                                        Suit = suit,
                                        TestResult = testResult
                };

            var testResultsBySuit = from testResultWithSuit in testResultsWithSuit
                                             group testResultsWithSuit by testResultWithSuit.Suit into g
                                             select new {Suit = g.Key, Tests = g};

            foreach (var testResultBySuit in testResultsBySuit)
            {
                string suitUid = Guid.NewGuid().ToString();

                TestSuitStarted(suitUid, testResultBySuit.Suit);

                foreach (MSTestResult testResult in testResultBySuit.Tests)
                {
                    TestStarted(suitUid, testResult.Name);
                            
                    switch (testResult.Outcome)
                    {
                        case TestOutcome.Failed:
                            TestFailed();
                            break;

                        default:
                            throw new Exception("Test result '" + testResult.Outcome.ToString() + "' not handled.");
                    }
                }

                TestSuitFinished(suitUid);
            }
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
            Allure.Lifecycle.Fire(new TestCaseStartedEvent(uid, name));
        }

        private void TestSuitFinished(string uid)
        {
            Allure.Lifecycle.Fire(new TestSuiteFinishedEvent(uid));
        }

    }
}

