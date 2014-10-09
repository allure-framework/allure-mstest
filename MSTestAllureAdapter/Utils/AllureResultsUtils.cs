using System;
using System.Linq;
using System.Collections.Generic;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// Utility method 
    /// Allure results utils.
    /// </summary>
    public static class AllureResultsUtils
    {
        public static long ToUnixEpochTime(this DateTime time)
        {
            return (long) (time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }

    public static class MSTestResultUtils
    {
        public static IEnumerable<MSTestResult> EnumerateTestResults(this IEnumerable<MSTestResult> results)
        {
            foreach (MSTestResult testReslt in results)
            {
                if (testReslt.InnerTests.Any())
                {
                    foreach (MSTestResult innerTestResult in testReslt.InnerTests)
                    {
                        yield return innerTestResult;
                    }
                }
                else
                {
                    yield return testReslt;
                }
            }
        }
    }
}

