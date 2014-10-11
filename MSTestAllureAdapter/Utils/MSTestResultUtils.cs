using System;
using System.Linq;
using System.Collections.Generic;

namespace MSTestAllureAdapter
{
    public static class MSTestResultUtils
    {
        public static IEnumerable<MSTestResult> EnumerateTestResults(this IEnumerable<MSTestResult> results)
        {
            foreach (MSTestResult testReslt in results)
            {
                if (testReslt.InnerTests != null && testReslt.InnerTests.Any())
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
