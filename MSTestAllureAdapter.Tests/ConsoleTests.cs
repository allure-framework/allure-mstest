using System;
using MSTestAllureAdapter.Console;
using NUnit.Framework;

namespace MSTestAllureAdapter.Tests
{
    [TestFixture]
    public class ConsoleTests
    {
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
            int actual = MainClass.Main(new string[]{ "non-existing-trx-file.trx" });

            Assert.AreEqual(expected, actual, "Main did not return the correct error code on a missing TRX file.");
        }

        [Test]
        public void Invalid_Trx_Returns_Error()
        {
            int expexted = 1;
            int actual = MainClass.Main(new string[]{ "InvalidFile.trx" });

            Assert.AreEqual(expexted, actual);
        }

        [Test]
        public void Valid_Trx_Returns_OK()
        {
            int expexted = 0;
            int actual = MainClass.Main(new string[]{ "sample.trx" });

            Assert.AreEqual(expexted, actual);
        }
    }
}

