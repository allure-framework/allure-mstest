using System;
using MSTestAllureAdapter.Console;
using NUnit.Framework;
using System.IO;

namespace MSTestAllureAdapter.Tests
{
    [TestFixture]
    public class ConsoleTests : MSTestAllureAdapterTestBase
    {
        string mTargetDir = "results";

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
            if (Directory.Exists(mTargetDir))
                Directory.Delete(mTargetDir, true);
        }

        [Test]
        public void Missing_Arguments_Return_Error()
        {
            int expected = 1;
            int actual = MainClass.Main(new string[0]);

            Assert.AreEqual(expected, actual, "Main did not return the correct error code when no arguments were passed.");
        }

        [Test]
        public void Missing_Trx_Returns_Error()
        {
            int expected = 1;
            int actual = MainClass.Main(new string[]{ "non-existing-trx-file.trx", mTargetDir });

            Assert.AreEqual(expected, actual, "Main did not return the correct error code on a missing TRX file.");
        }

        [Test]
        public void Invalid_Trx_Returns_Error()
        {
            int expexted = 1;

            int actual = MainClass.Main(new string[]{ "InvalidFile.trx", mTargetDir });

            Assert.AreEqual(expexted, actual);
        }

        [Test]
        public void Valid_Trx_Returns_OK()
        {
            int expexted = 0;
            int actual = MainClass.Main(new string[]{ "sample.trx", mTargetDir });

            Assert.AreEqual(expexted, actual);
        }
    }
}

