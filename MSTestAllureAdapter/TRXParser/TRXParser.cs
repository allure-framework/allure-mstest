using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MSTestAllureAdapter.VSTest;

namespace MSTestAllureAdapter
{
    public class TRXParser
    {
        public List<MSTestResult> GetTestResults(string trxFile)
        {
            var xml = File.ReadAllText(trxFile);
            var testRunType = Serializer.XML.Deserialize<TestRunType>(xml);

            Func<UnitTestType, string> outerKeySelector = _ => _.id;
            Func<UnitTestResultType, string> innerKeySelector = _ => _.testId;
            Func<UnitTestType, UnitTestResultType, MSTestResult> resultSelector = CreateMSTestResult;

            var testDefinations = testRunType.Items.Where(o => o.GetType() == typeof(TestDefinitionType)).Select(o => (TestDefinitionType)o);
            var unitTests = new List<UnitTestType>();
            foreach (var testDefination in testDefinations)
            {
                unitTests.AddRange(testDefination.Items.Where(o => o.GetType() == typeof(UnitTestType)).Select(o => (UnitTestType)o));
            }

            var results = testRunType.Items.Where(o => o.GetType() == typeof(ResultsType)).Select(o => (ResultsType)o);
            var unitTestResults = new List<UnitTestResultType>();
            foreach (var result in results)
            {
                unitTestResults.AddRange(result.Items.Where(o => o.GetType() == typeof(UnitTestResultType)).Select(o => (UnitTestResultType)o));
            }
            var mstestResults = unitTests.Join(unitTestResults, outerKeySelector, innerKeySelector, resultSelector);
            return mstestResults.ToList();
        }

        private MSTestResult CreateMSTestResult(UnitTestType unitTest, UnitTestResultType unitTestResult)
        {
            string testName = unitTest.TestMethod.name;
            string dataRowInfo = unitTestResult.dataRowInfo;

            // in data driven tests this appends the input row number to the test name
            if(dataRowInfo != null)
            testName += dataRowInfo;

            var testCategories = unitTest.Items.Where(o => o.GetType() == typeof(BaseTestTypeTestCategory)).Select(o => (BaseTestTypeTestCategory)o).Select(category => category.TestCategoryItem);
            var categories = (from testCategoryTypeTestCategoryItemse in testCategories from testCategoryTypeTestCategoryItem in testCategoryTypeTestCategoryItemse select testCategoryTypeTestCategoryItem.TestCategory).ToArray();

            var outcome = (TestOutcome)Enum.Parse(typeof(TestOutcome), unitTestResult.outcome);
            var start = DateTime.Parse(unitTestResult.startTime).ToUniversalTime();
            var end = DateTime.Parse(unitTestResult.endTime).ToUniversalTime();

            var innerTestResults = ReadInnerTestResults(unitTest, unitTestResult);

            var testResult = new MSTestResult(testName, outcome, start, end, categories, innerTestResults);

            var containsInnerTestResults = unitTestResult.InnerResults == null;
            if ((outcome == TestOutcome.Error || outcome == TestOutcome.Failed) && containsInnerTestResults)
            {
                testResult.ErrorInfo = ParseErrorInfo(unitTestResult.Items.Where(o => o.GetType() == typeof(OutputType)).Select(o => (OutputType)o));
            }
            testResult.Owner = GetOwner(unitTest);
            return testResult;
        }

        private IEnumerable<MSTestResult> ReadInnerTestResults(UnitTestType unitTest, UnitTestResultType unitTestResult)
        {
            if (unitTestResult.InnerResults == null)
                return null;
            var innerResults = unitTestResult.InnerResults.Items.Where(o => o.GetType() == typeof(UnitTestResultType)).Select(o => (UnitTestResultType)o);
            return innerResults.Select(innerResult => CreateMSTestResult(unitTest, innerResult)).ToList();
        }

        private ErrorInfo ParseErrorInfo(IEnumerable<OutputType> outputs)
        {
            var errorInfos = outputs.Select(outputType => new ErrorInfo(GetValue(outputType.ErrorInfo.Message), GetValue(outputType.ErrorInfo.StackTrace), GetValue(outputType.StdOut)));
            return errorInfos.FirstOrDefault();
        }

        private string GetValue(object value)
        {
            if (value == null)
                return string.Empty;
            var node = ((System.Xml.XmlNode[])(value)).FirstOrDefault();
            return node != null ? node.InnerText : string.Empty;
        }

        private string GetOwner(UnitTestType unitTest)
        {
            return unitTest.Items.Where(o => o.GetType() == typeof(BaseTestTypeOwners)).Select(o => (BaseTestTypeOwners)o).Select(baseTestTypeOwnerse => baseTestTypeOwnerse.Owner != null ? baseTestTypeOwnerse.Owner.name : null).FirstOrDefault();
        }
    }
}

