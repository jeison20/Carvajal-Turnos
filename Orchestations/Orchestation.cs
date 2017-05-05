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
                    if (ObjectManager.SaveDataEDI(Url))
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

        public void ProcessrecAdv()
        {
            IEnumerable<string> FullFile = File.ReadLines(@"C:\Users\USUARIO\Documents\Mis archivos recibidos\LVRECADV1.edi");
            string LineBGM = string.Empty;
            string LinesNAD = string.Empty;

            LineBGM = FullFile
                    .Where(line => line.Contains("BGM")).FirstOrDefault();

            string[] ArrayLineBGM = LineBGM.Replace("BGM+", "").Replace("'", "").Split('+');
            string PurchaseOrderNumber = ArrayLineBGM[1];
            int CuttingIndex = ArrayLineBGM[0].IndexOf(":");
            if (CuttingIndex > 0)
            {
                ArrayLineBGM[0] = ArrayLineBGM[0].Substring(0, CuttingIndex);
            }
            string str = File.ReadAllText(@"C:\Users\USUARIO\Documents\Mis archivos recibidos\LVRECADV1.edi");
            str = str.Replace("'", System.Environment.NewLine);
            File.WriteAllText(@"C:\Users\USUARIO\Documents\Mis archivos recibidos\LVRECADV1.edi", str);

            LineBGM = FullFile
                    .Where(line => line.Contains("NAD+")).FirstOrDefault();


        }
        public void StartsProcess()
        {
            ProcessOrder("Comercio1");
        }
    }
}