using Orchestations;
using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Carvajal.Turns.CodeResponses;

namespace ConsoleCarvajal
{
    //internal class Program
    public class Program
    {
        // private System.Diagnostics.EventLog eventLog1;
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
         private static void Main(string[] args)
        {
            ConfigureServices.Configure();
            string eventSourceName = "Turnos";
            string sEvent = "Sample Event";
            string sLog = "Application";

            // eventLog1 = new EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                EventLog.CreateEventSource(eventSourceName, sLog);
                LogOrchestation.WriteError("0", "0", "Creando EventSource");
            }
            EventLog.WriteEntry(eventSourceName, sEvent);
            /*try
            {
                new Orchestation().StartsProcess();
            }
            catch (Exception ex)
            {
                LogOrchestation.WriteError("0","0", Responses.A4 +ex.Message);
            }*/
        }
        public void Start()
        {
            // write code here that runs when the Windows Service starts up.  
            try
            {
                string eventSourceName = "Turnos";
                string sLog = "Application";

                // eventLog1 = new EventLog();
                if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
                {
                    EventLog.CreateEventSource(eventSourceName, sLog);
                    LogOrchestation.WriteError("0", "0", "Creando EventSource");
                }
                EventLog.WriteEntry(eventSourceName, "In OnStart 6");
                new Orchestation().StartsProcess();
                EventLog.WriteEntry(eventSourceName, "In OnStart 7");
            }
            catch (Exception ex)
            {
                LogOrchestation.WriteError("0", "0", Responses.A4 + ex.Message);
            }
        }
        public void Stop()
        {
            string eventSourceName = "Turnos";
            EventLog.WriteEntry(eventSourceName, "In OnStop 7");
            // write code here that runs when the Windows Service stops.  
        }


    }
}