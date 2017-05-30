using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using log4net;

namespace Orchestations
{
    
    public class LogOrchestation
    {
        // private static readonly ILog Log = LogManager.GetLogger(typeof(LogOrchestation));
        private static log4net.ILog GetLogger([CallerFilePath]string filename = "")
        {

            return log4net.LogManager.GetLogger(filename);
        }

        public static void WriteLog(string MerchantId, string ProcessID, string InfoMessage)
        {
            log4net.ILog Log = GetLogger();
            Log.Info(string.Format("[{0}][{1}] {2}", MerchantId, ProcessID, InfoMessage));
        }
        public static void WriteWarn(string MerchantId, string ProcessID, string InfoMessage)
        {
            log4net.ILog Log = GetLogger();
            Log.Warn(string.Format("[{0}][{1}] {2}", MerchantId, ProcessID, InfoMessage));
        }
        public static void WriteError(string MerchantId, string ProcessID, string InfoMessage)
        {
            log4net.ILog Log = GetLogger();
            Log.Error(string.Format("[{0}][{1}] {2}", MerchantId, ProcessID, InfoMessage));
        }
    }
}
