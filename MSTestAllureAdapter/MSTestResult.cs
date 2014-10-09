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
        private string[] mSuits;

        private MSTestResult(MSTestResult other)
            : this(other.Name, other.Outcome, other.Start, other.End, other.mSuits, other.InnerTests) 
        { 
            Owner = other.Owner;
            ErrorInfo = other.ErrorInfo; // ErrorInfo is immutable
        }

        public MSTestResult(string name, TestOutcome outcome, DateTime start, DateTime end, string[] suits, IEnumerable<MSTestResult> innerResults) 
		{ 
            Name = name;

            if (suits == null)
            {
                mSuits = new string[0];
            }
            else
            {
                // strings are immutable to no deep copy is needed.
                string[] suiteNamesCopy = new string[suits.Length];
                Array.Copy(suits, suiteNamesCopy, suits.Length);

                mSuits = suiteNamesCopy;
            }

            Outcome = outcome;
            Start = start;
            End = end;

            List<MSTestResult> results = new List<MSTestResult>();
            if (innerResults != null)
            {
                foreach (MSTestResult result in innerResults)
                {
                    results.Add(result.Clone());
                }
            }

            //InnerTests = innerTests.Count != 0 ? innerTests : Enumerable.Empty<MSTestResult>();
            InnerTests = results;
		}

        public MSTestResult(string name, TestOutcome outcome, DateTime start, DateTime end, string[] suits) 
            : this(name, outcome, start, end, suits, Enumerable.Empty<MSTestResult>()) { }


        public MSTestResult (string testName, TestOutcome outcome, DateTime start, DateTime end, string suite)
            : this(testName, outcome, start, end, new string[] { suite }) { }

        public MSTestResult(string testName, TestOutcome outcome, string[] suits, IEnumerable<MSTestResult> innerResults)
            : this(testName, outcome, default(DateTime), default(DateTime), suits, innerResults) { }


        public MSTestResult(string testName, TestOutcome outcome, string suit, IEnumerable<MSTestResult> innerResults)
            : this(testName, outcome, default(DateTime), default(DateTime), new string[] { suit }, innerResults) { }

        public MSTestResult (string testName, TestOutcome outcome, params string[] suits)
            : this(testName, outcome, default(DateTime), default(DateTime), suits) { }

        public MSTestResult (string testName, TestOutcome outcome, string suite)
            : this(testName, outcome, default(DateTime), default(DateTime), suite) { }

        public MSTestResult (string testName, TestOutcome outcome)
            : this(testName, outcome, default(DateTime), default(DateTime), (string[])null) { }

		public string Name { get; private set; }

        public IEnumerable<string> Suites
        {
            get { return mSuits; }
        }

		public TestOutcome Outcome { get; private set; }

        public DateTime Start { get; private set; }

        public DateTime End { get; private set; }

        public ErrorInfo ErrorInfo { get; set; }

        /// <summary>
        /// Gets or sets the test owner.
        /// According to the vstst.xsd currently only one owner is supported.
        /// </summary>
        /// <value>The test owner.</value>
        public string Owner { get; set; }

        public IEnumerable<MSTestResult> InnerTests { get; private set; }

        public MSTestResult Clone()
        {
            return new MSTestResult(this);
        }

        public override string ToString()
        {
            return string.Format("[MSTestResult: Name={0}, Suites=[{1}], Outcome={2}, Start={3}, End={4}, ErrorInfo={5}]", Name, String.Join(",", Suites), Outcome, Start, End, ErrorInfo);
        }
	}
}

