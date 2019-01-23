using System;
using System.IO;

namespace QModInstaller
{
    public static class Logger
    {
        private static string logFile = "";

        public static void WriteLog(string msg)
        {
            if (logFile != ""){
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(logFile, true))
                {
                    file.WriteLine(msg);
                }
            }
            else
            {
                Console.WriteLine("Log file not initialized properly");
            }
        }

        public static void StartNewLog(string gykPath)
        {
            logFile = gykPath + @"\output_log.txt";
            File.WriteAllText(logFile, "Starting Log\n");
        }
    }
}
