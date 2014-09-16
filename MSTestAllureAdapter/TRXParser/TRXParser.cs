using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MSTestAllureAdapter
{
    /// <summary>
    /// TRX parser.
    /// Based on the trx2html parser code: http://trx2html.codeplex.com/ 
    /// </summary>
	public class TRXParser
    {
        private readonly XNamespace mTrxNamespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        /*
        ErrorInfo ParseErrorInfo(XElement r)
        {
            ErrorInfo err = new ErrorInfo();
            if (r.Element(ns + "Output") != null && 
                r.Element(ns + "Output").Element(ns + "ErrorInfo") != null &&
                r.Element(ns + "Output").Element(ns + "ErrorInfo").Element(ns + "Message") != null )
                {
                    err.Message = r.Element(ns + "Output").Element(ns + "ErrorInfo").Element(ns + "Message").Value;

                }

            if (r.Descendants(ns + "StackTrace").Count()> 0 )
                {
                    err.StackTrace = r.Descendants(ns + "StackTrace").FirstOrDefault().Value;
                }

            if (r.Descendants(ns + "DebugTrace").Count() > 0)
                {
                    err.StdOut = r.Descendants(ns + "DebugTrace").FirstOrDefault().Value.Replace("\r\n", "<br />");
                }

            return err;
        }
        */
       


        public IEnumerable<MSTestResult> Parse(string filePath)
        {
            XDocument doc = XDocument.Load(filePath);
            XNamespace ns = mTrxNamespace;

            string testRunName = doc.Document.Root.Attribute("name").Value;
            string runUser = doc.Document.Root.Attribute("runUser").Value;

            IEnumerable<XElement> unitTests = doc.Descendants(ns + "UnitTest");           

            IEnumerable<XElement> unitTestResults = doc.Descendants(ns + "UnitTestResult");

            IEnumerable<MSTestResult> result = from unitTest in unitTests
                                  let id = unitTest.Element(ns + "Execution").Attribute("id").Value
                                  let description = unitTest.GetSafeValue(ns + "Description")
                                  let testClass = unitTest.GetSafeAttributeValue(ns + "TestMethod", "className")
                                  let testName = unitTest.GetSafeAttributeValue(ns + "TestMethod", "name")
                                  let categories = from testCategory in unitTest.Descendants(ns + "TestCategoryItem") select testCategory.GetSafeAttributeValue("TestCategory")
                                  join unitTestResult in unitTestResults
                on id equals unitTestResult.Attribute("executionId").Value
                                  let outcome = unitTestResult.Attribute("outcome").Value
                select new MSTestResult(testName, (TestOutcome)Enum.Parse(typeof(TestOutcome), outcome), categories.ToArray<string>());

            return result;
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
}

