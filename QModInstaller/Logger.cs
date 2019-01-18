using System.IO;

namespace QModInstaller
{
    public static class Logger
    {
        private static string logFile = @"C:\Program Files (x86)\Steam\steamapps\common\Graveyard Keeper\output_log.txt";

        public static void WriteLog(string msg)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logFile, true))
            {
                file.WriteLine(msg);
            }
        }

        public static void StartNewLog()
        {
            File.WriteAllText(logFile, "Starting Log\n");
        }
    }
}
