using System;
using System.IO;
using V2RayGCon.Resource.Resx;

namespace V2RayGCon.Lib.Sys
{
    // https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-open-and-append-to-a-log-file
    class Log
    {
        static readonly object writeLogLocker = new object();

        #region public method
        public static void Info(string message)
        {
            AppendLog("Info", message);
        }

        public static void Warn(string message)
        {
            AppendLog("Warn", message);
        }

        public static void Error(string message)
        {
            AppendLog("Error", message);
        }

        public static void Dump()
        {
            using (StreamReader r = File.OpenText(StrConst.LogFileName))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
        #endregion

        #region private method
        static void AppendLog(string prefix, string message)
        {
#if !DEBUG
            return;
#endif
            lock (writeLogLocker)
            {
                using (StreamWriter w = File.AppendText(StrConst.LogFileName))
                {
                    w.WriteLine("[{0}] {1} {2}",
                        prefix,
                        DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"),
                        message);
                }
            }
        }
        #endregion
    }
}
