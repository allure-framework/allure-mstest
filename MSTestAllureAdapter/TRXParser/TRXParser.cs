using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// MSTest TRX parser.
    /// </summary>
    public class TRXParser
    {
        // this namespace is required whenever using linq2xml on the trx.
        // for aesthetic reasons the naming convention was violated.
        private static readonly XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /// <summary>
        /// Parses the test results from the supplied trx file.
        /// </summary>
        /// <returns>The parsed test results.</returns>
        /// <param name="filePath">File path to the trx file.</param>
        public IEnumerable<MSTestResult> GetTestResults(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);

            IEnumerable<XElement> unitTests = doc.Descendants(ns + "UnitTest");

            IEnumerable<XElement> unitTestResults = doc.Descendants(ns + "UnitTestResult");

            Func<XElement, string> outerKeySelector = _ => _.Element(ns + "Execution").Attribute("id").Value;
            Func<XElement, string> innerKeySelector = _ => _.Attribute("executionId").Value;
            Func<XElement, XElement, MSTestResult> resultSelector = CreateMSTestResult;

            IEnumerable<MSTestResult> result = unitTests.Join<XElement, XElement, string, MSTestResult>(unitTestResults, outerKeySelector, innerKeySelector, resultSelector);
             
            // this will return the flat list of the tests with the inner tests.
            // here a test 'parent' that holds other tests will be discarded (such as the data driven tests).
            result = result.EnumerateTestResults();

            return result;
        }

        private ErrorInfo ParseErrorInfo(XElement errorInfoXmlElement)
        {
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("prefix", ns.NamespaceName);

            string message = null;
            XElement messageElement = errorInfoXmlElement.XPathSelectElement("prefix:ErrorInfo/prefix:Message", xmlNamespaceManager);
            if (messageElement != null)
            {
                message = messageElement.Value;
            }

            string stackTrace = null;
            XElement stackTraceElement = errorInfoXmlElement.XPathSelectElement("prefix:ErrorInfo/prefix:StackTrace", xmlNamespaceManager);
            if (stackTraceElement != null)
            {
                stackTrace = stackTraceElement.Value;
            }

            string stdOut = null;
            XElement stdOutElement = errorInfoXmlElement.XPathSelectElement("prefix:StdOut", xmlNamespaceManager);
            if (stdOutElement != null)
            {
                stdOut = stdOutElement.Value;
            }

            return new ErrorInfo(message, stackTrace, stdOut);
        }

        private MSTestResult CreateMSTestResult(XElement unitTest, XElement unitTestResult)
        {
            string testName = unitTest.GetSafeAttributeValue(ns + "TestMethod", "name");

            string dataRowInfo = unitTestResult.GetSafeAttributeValue("dataRowInfo");

            // in data driven tests this appends the input row number to the test name
            testName += dataRowInfo;

            TestOutcome outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), unitTestResult.Attribute("outcome").Value);

            DateTime start = DateTime.Parse(unitTestResult.Attribute("startTime").Value).ToUniversalTime();

            DateTime end = DateTime.Parse(unitTestResult.Attribute("endTime").Value).ToUniversalTime();

            string[] categories = (from testCategory in unitTest.Descendants(ns + "TestCategoryItem")
                                            select testCategory.GetSafeAttributeValue("TestCategory")).ToArray<string>();
                
            /*
            if (categories.Length == 0)
                categories = new string[]{ DEFAULT_CATEGORY };
            */

            IEnumerable<MSTestResult> innerTestResults = ParseInnerTestResults(unitTest, unitTestResult);

            MSTestResult testResult = new MSTestResult(testName, outcome, start, end, categories, innerTestResults);

            bool containsInnerTestResults = unitTestResult.Element(ns + "InnerResults") == null;
            if ((outcome == TestOutcome.Error || outcome == TestOutcome.Failed) && containsInnerTestResults)
            {
                testResult.ErrorInfo = ParseErrorInfo(unitTestResult.Element(ns + "Output"));
            }

            testResult.Owner = GetOwner(unitTest);

            return testResult;
        }

        private IEnumerable<MSTestResult> ParseInnerTestResults(XElement unitTest, XElement unitTestResult)
        {
            IEnumerable<XElement> innerResultsElements = unitTestResult.Descendants(ns + "InnerResults");

            if (!innerResultsElements.Any())
                return null;

            // the schema for the trx states there can be multiple 'InnerResults' elements but
            // until we see it we take the first.
            // In the future if it will be required to handle multiple 'InnerResults' elements 
            // one can wrap the comming loop in another loop that loops over them.
            XElement innerResultsElement = innerResultsElements.FirstOrDefault<XElement>();

            IList<MSTestResult> result = new List<MSTestResult>();

            foreach (XElement innerUnitTestResult in innerResultsElement.Descendants(ns + "UnitTestResult"))
            {
                result.Add(CreateMSTestResult(unitTest, innerUnitTestResult));
            }

            return result;
        }

        private string GetOwner(XElement unitTestElement)
        {
            string owner = null;

            XElement ownerElement = unitTestElement.Descendants(ns + "Owner").FirstOrDefault();

            if (ownerElement != null)
            {
                XAttribute ownerAttribute = ownerElement.Attribute("name");
                owner = ownerAttribute.Value;
            }

            return owner;
        }
    }
}

