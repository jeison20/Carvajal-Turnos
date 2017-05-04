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
                new Orchestation().StartsProcess();
            }
            catch (Exception ex)
            {
                LogOrchestation.WriteLog(ex.Message);
            }
        }
    }
}