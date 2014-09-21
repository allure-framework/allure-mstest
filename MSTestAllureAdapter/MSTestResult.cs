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
        Unknown,
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
        public MSTestResult(string name, TestOutcome outcome, DateTime start, DateTime end, string[] suits) 
		{ 
            Name = name;

			// strings are immutable to no deep copy is needed.
            string[] suiteNamesCopy = new string[suits.Length];
            Array.Copy(suits, suiteNamesCopy, suits.Length);

			Suites = suiteNamesCopy;

			Outcome = outcome;
            Start = start;
            End = end;
		}


        public MSTestResult (string testName, TestOutcome outcome, DateTime start, DateTime end, string suiteName)
            : this(testName, outcome, start, end, new string[] { suiteName }) { }

		public string Name { get; private set; }

		public IEnumerable<string> Suites { get; private set; }

		public TestOutcome Outcome { get; private set; }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }
	}
}

