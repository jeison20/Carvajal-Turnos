using Managers;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Orchestations
{
    public class Orchestation
    {
        public void ProcessOrder(string IdentificacionComercio)
        {
            Manager ObjectManager = new Manager();

            ObjectManager.CompleteZeros(4, "21");

            LogOrchestation.WriteLog("Inicio de ejecucion orden de pedido");
            if (!Directory.Exists(ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio))
            {
                DirectoryInfo di = Directory.CreateDirectory(ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio);
            }
            List<string> ListaArchivosNoDescargados = new List<string>();
            List<string> ListaArchivosDescargados = ObjectManager.DownloadFilesByFolder(IdentificacionComercio + "/SinProcesar", ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio, out ListaArchivosNoDescargados);

            foreach (string Url in ListaArchivosDescargados)
            {
                string Content = File.ReadAllText(Url);
                Content = Content.Replace("'", System.Environment.NewLine);
                File.WriteAllText(Url, Content);

                ObjectManager.AuthenticArchiveMD5(Url);
                if (!ObjectManager.ValidFileStructure(Url))
                {
                    LogOrchestation.WriteLog(string.Format("El archivo {0} no posee la estructura correcta.", Path.GetFileName(Url)));
                    continue;
                }

                Stream Archivo = File.OpenRead(Url);
                if (ObjectManager.UploadFile(Path.GetFileName(Url), Archivo, IdentificacionComercio, "Procesados"))
                {
                    Archivo.Close();
                    bool ValidSaveData = false;

                    if (Path.GetFileName(Url).Contains("RECADV"))
                    {
                        ValidSaveData = ObjectManager.SaveDataEDI(Url);
                    }
                    else
                    {
                        ValidSaveData = ObjectManager.SaveDataRecAdvEDI(Url);
                    }

                    if (ValidSaveData)
                    {
                        LogOrchestation.WriteLog(string.Format("El archivo {0} fue Subido exitosamente a la carpeta Procesados", Path.GetFileName(Url)));
                        if (ObjectManager.DeleteFile(IdentificacionComercio + "/SinProcesar/" + Path.GetFileName(Url)))
                            LogOrchestation.WriteLog(string.Format("El archivo {0} fue Eliminado exitosamente a la carpeta SinProcesar", Path.GetFileName(Url)));
                        else
                        {
                            ObjectManager.DeleteFile(IdentificacionComercio + "/Procesados/" + Path.GetFileName(Url));
                            LogOrchestation.WriteLog(string.Format("El archivo {0} No fue eliminado de la carpeta SinProcesar", Path.GetFileName(Url)));
                        }
                    }
                    else
                    {
                        ObjectManager.DeleteFile(IdentificacionComercio + "/Procesados/" + Path.GetFileName(Url));
                        LogOrchestation.WriteLog(string.Format("No fue posible guardar la informacion del archivo {0}.", Path.GetFileName(Url)));
                    }
                }
                else
                    LogOrchestation.WriteLog("El archivo no fue Subido exitosamente a la carpeta Procesados");

                Archivo.Close();
            }
            foreach (var Nombre in ListaArchivosNoDescargados)
            {
                LogOrchestation.WriteLog(string.Format("El archivo {0} no fue descargado exitosamente.", Nombre));
            }

            LogOrchestation.WriteLog("Fin de ejecucion orden de pedido");
        }

        public void StartsProcess()
        {
            ProcessOrder("Comercio1");
        }
    }


}