using System;

namespace MSTestAllureAdapter
{
	public enum TestOutcome { Failure, Success }

	public class MSTestResult
	{
		public MSTestResult (string testName, string suiteName, TestOutcome outcome)
		{
			Name = testName;
			Suite = suiteName;
			Outcome = outcome;
		}

		public string Name { get; private set; }

		public string Suite { get; private set; }

		public TestOutcome Outcome { get; private set; }
	}
}

