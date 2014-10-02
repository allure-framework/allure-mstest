using System;
using System.IO;

namespace MSTestAllureAdapter.Console
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            string resultsDir;
            string trxPath;

            if (!ParseCommandLineOptions(args, out trxPath, out resultsDir))
            {
                DisplayHelp();
                return 1;
            }

            if (!File.Exists(trxPath))
            {
                System.Console.WriteLine("Unable to open file supplied TRX file: '" + trxPath + "'");
                return 1;
            }

            AllureAdapter adapter = new AllureAdapter();
            adapter.GenerateTestResults(trxPath, resultsDir);

            return 0;
        }

        /// <summary>
        /// Parses the command line options.
        /// Because the there are only 2 options the parsing is done manually without a library.
        /// </summary>
        /// <returns><c>true</c>, if command line options were parsed, <c>false</c> otherwise.</returns>
        /// <param name="args">The command line arguments.</param>
        /// <param name="trxPath">Trx path.</param>
        /// <param name="reportPath">Report path.</param>
        private static bool ParseCommandLineOptions(string[] args, out string trxPath, out string outputPath)
        {
            outputPath = null;
            trxPath = null;

            if (args == null || args.Length == 0)
                return false;

            trxPath = args[0];

            outputPath = args.Length > 2 ? args[1] : "results";

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            if (!outputPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                outputPath += Path.DirectorySeparatorChar;
            }

            return true;
        }

        private static void DisplayHelp()
        {
            // if GetCommandLineArgs cannot obtain the program name then the first element of the array is
            // string.Empty but the array size will always be greater than zero.
            // Path.GetFileName handles string.Empty correctly.

            string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

            string help = String.Empty;

            help += "Usage: " + programName + " ";

            string reportDirDisplayName = "<output target dir>";

            help += "<TRX file> " + reportDirDisplayName;
            help += Environment.NewLine;
            help += "If " + reportDirDisplayName + " is missing the current directory is used.";

            System.Console.WriteLine(help);
        }
    }
}
