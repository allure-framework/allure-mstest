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

            if (suits == null)
            {
                suits = new string[0];
            }

            // strings are immutable to no deep copy is needed.
            string[] suiteNamesCopy = new string[suits.Length];
            Array.Copy(suits, suiteNamesCopy, suits.Length);

            Suites = suiteNamesCopy;

			Outcome = outcome;
            Start = start;
            End = end;
		}


        public MSTestResult (string testName, TestOutcome outcome, DateTime start, DateTime end, string suite)
            : this(testName, outcome, start, end, new string[] { suite }) { }

        public MSTestResult (string testName, TestOutcome outcome, params string[] suits)
            : this(testName, outcome, default(DateTime), default(DateTime), suits) { }

        public MSTestResult (string testName, TestOutcome outcome, string suite)
            : this(testName, outcome, default(DateTime), default(DateTime), suite) { }

        public MSTestResult (string testName, TestOutcome outcome)
            : this(testName, outcome, default(DateTime), default(DateTime), (string[])null) { }

		public string Name { get; private set; }

		public IEnumerable<string> Suites { get; private set; }

		public TestOutcome Outcome { get; private set; }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public ErrorInfo ErrorInfo { get; set; }

        public override string ToString()
        {
            return string.Format("[MSTestResult: Name={0}, Suites=[{1}], Outcome={2}, Start={3}, End={4}, ErrorInfo={5}]", Name, String.Join(",", Suites), Outcome, Start, End, ErrorInfo);
        }
	}
}

