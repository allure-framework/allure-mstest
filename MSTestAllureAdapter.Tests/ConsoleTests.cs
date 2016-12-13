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
        public void MissingArgumentsReturnError()
        {
            int expected = 1;
            int actual = MainClass.Main(new string[0]);

            Assert.AreEqual(expected, actual, "Main did not return the correct error code when no arguments were passed.");
        }

        [Test]
        public void MissingTrxReturnsError()
        {
            int expected = 1;
            int actual = MainClass.Main(new string[]{ "non-existing-trx-file.trx", mTargetDir });

            Assert.AreEqual(expected, actual, "Main did not return the correct error code on a missing TRX file.");
        }

        [Test]
        public void InvalidTrxReturnsError()
        {
            int expexted = 1;

            int actual = MainClass.Main(new string[]{ Path.Combine("MSTestAllureAdapter.Tests\\trx", "InvalidFile.trx"), mTargetDir });

            Assert.AreEqual(expexted, actual);
        }

        [Test]
        public void ValidTrxReturnsOK()
        {
            int expexted = 0;
            int actual = MainClass.Main(new string[]{ Path.Combine("MSTestAllureAdapter.Tests\\trx", "sample.trx"), mTargetDir });

            Assert.AreEqual(expexted, actual);
        }
    }
}

