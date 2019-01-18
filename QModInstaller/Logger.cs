using System.IO;

namespace QModInstaller
{
    public static class Logger
    {
        private static string logFile;

        public static void WriteLog(string msg)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine(msg);
            }
        }

        public static void StartNewLog(string location)
        {
            logFile = location;
            File.WriteAllText(logFile, "Starting Log");
        }
    }
}
