using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;


namespace TextFile
{
    public static class LogHelper
    {
        private static log4net.ILog GetLogger([CallerFilePath]string filename = "")
        {
            return log4net.LogManager.GetLogger(filename);
        }

        public static void WriteLog(string Mensaje)
        {
            log4net.ILog Log = GetLogger();
            Log.Info(Mensaje);
        }
    }
}
