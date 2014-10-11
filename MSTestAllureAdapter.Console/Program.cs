using System;
using System.IO;

namespace MSTestAllureAdapter.Console
{
    internal class MainClass
    {
        const int ERROR = 1;
        const int OK = 0;
        const string DEFAULT_RESULT_DIR = "results";

        public static int Main(string[] args)
        {
            string resultsDir;
            string trxPath;

            if (!ParseCommandLineOptions(args, out trxPath, out resultsDir))
            {
                DisplayHelp();
                return ERROR;
            }

            if (!File.Exists(trxPath))
            {
                System.Console.WriteLine("The supplied TRX file: '" + trxPath + "' was not found.");
                return ERROR;
            }

            try {
                AllureAdapter adapter = new AllureAdapter();
                System.Console.Write("Generating allure files... ");
                adapter.GenerateTestResults(trxPath, resultsDir);
                System.Console.WriteLine("Done.");
                return OK;
            }
            catch (Exception e)
            {
                System.Console.WriteLine("There was an error generating allure files into '" + resultsDir + "' from the TRX file '" + trxPath + "'.");
                // perhaps in the future we'll use log4net.
                System.Console.WriteLine(e.ToString());
                return ERROR;
            }
        }

        /// <summary>
        /// Parses the command line options.
        /// Because the there are only 2 options the parsing is done manually instead of using a library.
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

            outputPath = args.Length > 2 ? args[1] : DEFAULT_RESULT_DIR;

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
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

            string targetDirDisplayName = "[output target dir]";

            help += "<TRX file> " + targetDirDisplayName;
            help += Environment.NewLine;
            help += "If '" + targetDirDisplayName + "' is missing the reslts are saved in the current directory in a folder named '" + DEFAULT_RESULT_DIR  + "'.";

            System.Console.WriteLine(help);
        }
    }
}
