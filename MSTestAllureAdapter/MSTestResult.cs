using System;
using System.Collections.Generic;
using System.Linq;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// Test outcome.
    /// From the TestOutcome type in the vstst.xsd
    /// </summary>
    public enum TestOutcome { 
        Error,
        Failed,
        Timeout,
        Aborted,
        Inconclusive,
        PassedButRunAborted,
        NotRunnable,
        NotExecuted,
        Disconnected,
        Warning,
        Passed,
        Completed,
        InProgress,
        Pending
    }

	public class MSTestResult
	{
        public MSTestResult(string name, TestOutcome outcome, string[] suits) 
		{ 
            Name = name;

			// strings are immutable to no deep copy is needed.
            string[] suiteNamesCopy = new string[suits.Length];
            Array.Copy(suits, suiteNamesCopy, suits.Length);

			Suites = suiteNamesCopy;

			Outcome = outcome;
		}


		public MSTestResult (string testName, TestOutcome outcome, string suiteName)
			: this(testName, outcome, new string[] { suiteName }) { }

		public string Name { get; private set; }

		public IEnumerable<string> Suites { get; private set; }

		public TestOutcome Outcome { get; private set; }
	}
}

