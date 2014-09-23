using System;

namespace MSTestAllureAdapter
{
    public class AllureAdapter : AllureAdapterBase
    {
        protected override void HandleTestResult(MSTestResult testResult)
        {
            switch (testResult.Outcome)
            {
                case TestOutcome.Failed:
                    TestFailed(testResult.ErrorInfo);
                    break;
            }
        }
    }
}

