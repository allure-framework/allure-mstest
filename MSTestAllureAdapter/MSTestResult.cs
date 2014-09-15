using System;
using System.Collections.Generic;

namespace MSTestAllureAdapter
{
	public enum TestOutcome { Failure, Success }

	public class MSTestResult
	{
		public MSTestResult(string testName, TestOutcome outcome, params string[] suiteNames) 
		{ 
			Name = testName;

			// strings are immutable to no deep copy is needed.
			string[] suiteNamesCopy = new string[suiteNames.Length];
			Array.Copy(suiteNames, suiteNamesCopy, suiteNames.Length);

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

