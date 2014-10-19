using System;
using System.Collections.Generic;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// An abstraction on the way test results are provided.
    /// </summary>
    public interface ITestResultProvider
    {
        /// <summary>
        /// Gets the test results form the supplied file.
        /// </summary>
        /// <returns>The test results.</returns>
        /// <param name="filePath">The file containing the test results.</param>
        IEnumerable<MSTestResult> GetTestResults(string filePath);
    }
}

