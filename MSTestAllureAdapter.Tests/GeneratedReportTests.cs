using System;
using NUnit.Framework;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using XmlUnit.Xunit;

namespace MSTestAllureAdapter.Tests
{
    [TestFixture]
    public class GeneratedReportTests
    {
        string mTargetDir = "results";

        private void DeleteTargetDir()
        {
            if (Directory.Exists(mTargetDir))
                Directory.Delete(mTargetDir, true);
        }

        [SetUp]
        public void SetUp()
        {
            DeleteTargetDir();
            Directory.CreateDirectory(mTargetDir);
        }

        [Test]
        #if __MonoCS__
        [Ignore("Currently mono has a bug with xsd validation.")]
        #endif
        public void Generated_Files_Have_Correct_Schema()
        {
            AllureAdapter adapter = new AllureAdapter();
            adapter.GenerateTestResults("sample.trx", mTargetDir);

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.Schemas.Add(null, Path.Combine("xsd", "allure.xsd"));
            readerSettings.ValidationType = ValidationType.Schema;

            string[] files = Directory.GetFiles(mTargetDir, "*.xml");

            if (files.Length == 0)
            {
                Assert.Fail("No generated files were found.");
            }

            foreach (string file in files)
            {
                // XmlDocument.Load will also work but we don't want to load the entire XML to memory.
                XmlReader xmlReader = XmlReader.Create(file, readerSettings);
                while (xmlReader.Read());
            }
        }

        [Test]
        public void Match_Categories()
        {
            Dictionary<string, string> expected = new Dictionary<string, string>();
            Dictionary<string, string> actual = new Dictionary<string, string>();



            AllureAdapter adapter = new AllureAdapter();
            adapter.GenerateTestResults("sample.trx", mTargetDir);

            XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            xmlNamespaceManager.AddNamespace("prefix", "urn:model.allure.qatools.yandex.ru");

            FillCategoryToXmlMap("sample-output", expected);
            FillCategoryToXmlMap(mTargetDir, actual);

            if (expected.Keys.Count != actual.Keys.Count)
            {
                Assert.Fail("The expected {0} categories but found {1}.", expected.Keys.Count, actual.Keys.Count);
            }

            foreach (string category in actual.Keys)
            {
                if (!expected.ContainsKey(category))
                {
                    Assert.Fail("The category " + category + " was not expected.");
                }

                string expectedFile = expected[category];
                string actualFile = actual[category];

                XmlInput control = new XmlInput(File.ReadAllText(expectedFile));
                XmlInput test = new XmlInput(File.ReadAllText(actualFile));
                
                XmlDiff xmlDiff = new XmlDiff(control, test);
                
                DiffResult diffResult = xmlDiff.Compare();
                if (!diffResult.Identical)
                {
                    Assert.Fail("The expected file {0} was different from the actual file {1}", expectedFile, actualFile);
                }
            }
        }

        private void FillCategoryToXmlMap(string source, IDictionary<string, string> map)
        {
            string[] files = Directory.GetFiles(source, "*.xml");

            foreach (string file in files)
            {
                string category = String.Empty;
                XDocument xDoc = XDocument.Load(file);
                XElement element = xDoc.Root.XPathSelectElement("./name");

                if (element != null)
                {
                    category = element.Value;
                }
                
                map[category] = file;
            }
        }
    }
}

