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
    /// MSTest TRX output parser.
    /// </summary>
    public class TRXParser
    {
        internal static readonly XNamespace TrxNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /// <summary>
        /// The category given to tests without any category.
        /// </summary>
        public static readonly string DEFAULT_CATEGORY = "NO_CATEGORY";

        public IEnumerable<MSTestResult> GetTestResults(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = TrxNamespace;

            string testRunName = doc.Document.Root.Attribute("name").Value;
            string runUser = doc.Document.Root.Attribute("runUser").Value;

            IEnumerable<XElement> unitTests = doc.Descendants(ns + "UnitTest").ToList();           

            IEnumerable<XElement> unitTestResults = doc.Descendants(ns + "UnitTestResult").ToList();

            Func<XElement, string> outerKeySelector = _ => _.Element(ns + "Execution").Attribute("id").Value;
            Func<XElement, string> innerKeySelector = _ => _.Attribute("executionId").Value;
            Func<XElement, XElement, MSTestResult> resultSelector = CreateMSTestResult;

            IEnumerable<MSTestResult> result = unitTests.Join<XElement, XElement, string, MSTestResult>(unitTestResults, outerKeySelector, innerKeySelector, resultSelector);
             
            // this will return the flat list of the tests with the inner tests.
            // here a test 'parent' that holds other tests will be discarded.
            result = result.EnumerateTestResults();

            return result;
        }


        private ErrorInfo ParseErrorInfo(XElement errorInfoXmlElement)
        {
            if (errorInfoXmlElement == null)
                return new ErrorInfo("Error occured in parsing errorInfo");

            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("prefix", TrxNamespace.NamespaceName);

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
            XNamespace ns = TrxNamespace;

            string testName = unitTest.GetSafeAttributeValue(ns + "TestMethod", "name");

            string dataRowInfo = unitTestResult.GetSafeAttributeValue("dataRowInfo");

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

            IEnumerable<MSTestResult> innerTestResults = ReadInnerTestResults(unitTest, unitTestResult);

            MSTestResult testResult = new MSTestResult(testName, outcome, start, end, categories, innerTestResults);

            if (outcome == TestOutcome.Error || outcome == TestOutcome.Failed)
            {
                testResult.ErrorInfo = ParseErrorInfo(unitTestResult.Element(ns + "Output"));
            }

            testResult.Owner = GetOwner(unitTest);

            return testResult;
        }

        private IEnumerable<MSTestResult> ReadInnerTestResults(XElement unitTest, XElement unitTestResult)
        {
            XNamespace ns = TrxNamespace;
            IEnumerable<XElement> innerResultsElements = unitTestResult.Descendants(ns + "InnerResults");

            if (!innerResultsElements.Any())
                return null;

            // there can be only one.
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
            XNamespace ns = TrxNamespace;

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

    internal static class XElementExtensions
    {
        public static string GetSafeValue(this XElement element, XName name)
        {
            string result = String.Empty;

            element = element.Element(name);

            if (element != null)
            {
                result = element.Value;
            }

            return result;
        }

        public static string GetSafeAttributeValue(this XElement element, XName descendantElement, XName attributeName)
        {
            string result = String.Empty;

            element = element.Element(descendantElement);

            if (element != null && element.Attribute(attributeName) != null)
            {
                result = element.Attribute(attributeName).Value;
            }

            return result;
        }

        public static string GetSafeAttributeValue(this XElement element, XName attributeName)
        {
            string result = String.Empty;

            if (element != null && element.Attribute(attributeName) != null)
            {
                result = element.Attribute(attributeName).Value;
            }

            return result;
        }
    }

    /// <summary>
    /// Represents an ErrorInfo element in the trx file.
    /// </summary>
    public class ErrorInfo
    {

        public ErrorInfo(string message)
            : this(message, null) { }

        public ErrorInfo(string message, string stackTrace)
            : this(message, stackTrace, null) { }

        public ErrorInfo(string message, string stackTrace, string stdOut)
        {
            Message = message;
            StackTrace = stackTrace;
            StdOut = stdOut;
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; private set; }

        /// <summary>
        /// Gets or sets the StdOut.
        /// </summary>
        /// <value>The StdOut.</value>
        public string StdOut { get; private set; }

        public override string ToString()
        {
            return string.Format("[ErrorInfo: Message={0}, StackTrace={1}, StdOut={2}]", Message, StackTrace, StdOut);
        }

    }
}

