using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ConsoleCarvajal
{
    internal static class ConfigureServices
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<Program>(service =>
                {
                    service.ConstructUsing(s => new Program());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName("Turnos Cargue Archivos");
                configure.SetDisplayName("Turnos Cargue Archivos");
                configure.SetDescription("Cargue de Archivos Maestros: OC, RecAdv, Usuarios, CE, Tiempos");
                configure.StartAutomatically();
            });
        }
    }
}
