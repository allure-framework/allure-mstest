using System;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// Allure adapter.
    /// </summary>
    public class AllureAdapter : AllureAdapterBase
    {
        protected override void HandleTestResult(MSTestResult testResult)
        {
            switch (testResult.Outcome)
            {
                case TestOutcome.Failed:
                    TestFailed(testResult);
                    break;
            }
        }
    }
}

