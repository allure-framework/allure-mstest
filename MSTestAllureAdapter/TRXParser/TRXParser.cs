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
        private static readonly XNamespace mTrxNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /// <summary>
        /// The category given to tests without any category.
        /// </summary>
        public static readonly string DEFAULT_CATEGORY = "NO_CATEGORY";

        public IEnumerable<MSTestResult> GetTestResults(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = mTrxNamespace;

            string testRunName = doc.Document.Root.Attribute("name").Value;
            string runUser = doc.Document.Root.Attribute("runUser").Value;

            IEnumerable<XElement> unitTests = doc.Descendants(ns + "UnitTest").ToList();           

            IEnumerable<XElement> unitTestResults = doc.Descendants(ns + "UnitTestResult").ToList();

            Func<XElement, string> outerKeySelector = _ => _.Element(ns + "Execution").Attribute("id").Value;
            Func<XElement, string> innerKeySelector = _ => _.Attribute("executionId").Value;
            Func<XElement, XElement, MSTestResult> resultSelector = CreateMSTestResult;

            IEnumerable<MSTestResult> result = unitTests.Join<XElement, XElement, string, MSTestResult>(unitTestResults, outerKeySelector, innerKeySelector, resultSelector);

            return result;
        }


        private ErrorInfo ParseErrorInfo(XElement errorInfoXmlElement)
        {
            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("prefix", mTrxNamespace.NamespaceName);

            ErrorInfo errorInfo = new ErrorInfo();

            XElement messageElement = errorInfoXmlElement.XPathSelectElement("prefix:ErrorInfo/prefix:Message", xmlNamespaceManager);
            if (messageElement != null)
                {
                    errorInfo.Message = messageElement.Value;
                }

            XElement stackTraceElement = errorInfoXmlElement.XPathSelectElement("prefix:ErrorInfo/prefix:StackTrace", xmlNamespaceManager);
            if (stackTraceElement != null)
                {
                    errorInfo.StackTrace = stackTraceElement.Value;
                }

            XElement stdOutElement = errorInfoXmlElement.XPathSelectElement("prefix:StdOut", xmlNamespaceManager);
            if (stdOutElement != null)
                {
                    errorInfo.StdOut = stdOutElement.Value;
                }

            return errorInfo;
        }

        private MSTestResult CreateMSTestResult(XElement unitTest, XElement unitTestResult)
        {
            XNamespace ns = mTrxNamespace;
            string testName = unitTest.GetSafeAttributeValue(ns + "TestMethod", "name");
            TestOutcome outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), unitTestResult.Attribute("outcome").Value);
            DateTime start = DateTime.Parse(unitTestResult.Attribute("startTime").Value);
            DateTime end = DateTime.Parse(unitTestResult.Attribute("endTime").Value);
            string[] categories = (from testCategory in unitTest.Descendants(ns + "TestCategoryItem")
                                            select testCategory.GetSafeAttributeValue("TestCategory")).ToArray<string>();
            if (categories.Length == 0)
                categories = new string[]{ "NO_CATEGORY" };

            MSTestResult testResult = new MSTestResult(testName, outcome, start, end, categories);

            if (outcome == TestOutcome.Error || outcome == TestOutcome.Failed)
            {
                    testResult.ErrorInfo = ParseErrorInfo(unitTestResult.Element(ns + "Output"));
            }
            return testResult;
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

        public static string GetSafeAttributeValue(this XElement element, XName name, XName attributeName)
        {
            string result = String.Empty;

            element = element.Element(name);

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

        public static TimeSpan ParseDuration(this XElement element, string attributeName)
        {
            TimeSpan result = new TimeSpan(0);

            XAttribute attribute = element.Attribute(attributeName);

            if (attribute != null)
            {
                result = TimeSpan.Parse(attribute.Value);
            }

            return result;
        }
    }

    /// <summary>
    /// Represents an ErrorInfo element in the trx file.
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
        /// <value>The stack trace.</value>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets or sets the StdOut.
        /// </summary>
        /// <value>The StdOut.</value>
        public string StdOut { get; set; }

        public override string ToString()
        {
            return string.Format("[ErrorInfo: Message={0}, StackTrace={1}, StdOut={2}]", Message, StackTrace, StdOut);
        }

    }
}

