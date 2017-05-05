using Orchestations;
using System;

namespace ConsoleCarvajal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                new Orchestation().ProcessrecAdv();
            }
            catch (Exception ex)
            {
                LogOrchestation.WriteLog(ex.Message);
            }
        }
    }
}