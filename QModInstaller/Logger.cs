using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace QModInstaller
{
    public static class Logger
    {

        public static void WriteLog(string msg)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\oldar\Desktop\testfolder\output_log.txt", true))
            {
                file.WriteLine(msg);
            }
        }

        public static void StartNewLog(string location)
        {
            File.WriteAllText(@location, "Starting Log");
        }
    }
}
