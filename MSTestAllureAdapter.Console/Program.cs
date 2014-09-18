using System;
using System.IO;

namespace MSTestAllureAdapter.Console
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string resultsDir = "results";

            if (!Directory.Exists(resultsDir))
            {
                Directory.CreateDirectory(resultsDir);
            }

            if (!resultsDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                resultsDir += Path.DirectorySeparatorChar;
            }

            AllureAdapter adapter = new AllureAdapter();
            adapter.Run("sample.trx", resultsDir);
        }
    }
}
