using Managers;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Orchestations
{
    public class Orchestation
    {
        public bool ProcessOrder(string IdentificacionComercio)
        {
            try
            {
                Manager ObjectManager = new Manager();

                LogOrchestation.WriteLog("Inicio de ejecucion orden de pedido");
                if (!Directory.Exists(ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio))
                {
                    DirectoryInfo di = Directory.CreateDirectory(ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio);
                }
                List<string> ListaArchivosNoDescargados = new List<string>();
                List<string> ListaArchivosDescargados = ObjectManager.DownloadFilesByFolder(IdentificacionComercio + "/SinProcesar", ConfigurationManager.AppSettings["Url"] + "\\" + IdentificacionComercio, out ListaArchivosNoDescargados);
                List<string> ListFilesOK = new List<string>();
                List<string> ListFilesError = new List<string>();
                foreach (string Url in ListaArchivosDescargados)
                {
                    try
                    {
                        if (File.ReadAllLines(Url).Length == 1)
                        {
                            string Content = File.ReadAllText(Url);
                            Content = Content.Replace("'", "'" + System.Environment.NewLine);
                            File.WriteAllText(Url, Content);
                        }

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

                            if (!Path.GetFileName(Url).Contains("RECADV"))
                            {
                                ValidSaveData = ObjectManager.SaveDataEDI(Url);
                            }
                            else
                            {
                                ValidSaveData = ObjectManager.SaveDataRecAdvEDI(Url);
                            }

                            if (ValidSaveData)
                            {
                                LogOrchestation.WriteLog(string.Format("El archivo {0} fue Subido exitosamente a la carpeta Procesados y fue guardado exitosamente", Path.GetFileName(Url)));
                                ListFilesOK.Add(string.Format("El archivo {0} perteneciente al comercio " + IdentificacionComercio + " fue Subido exitosamente a la carpeta Procesados y fue guardado exitosamente", Path.GetFileName(Url)));
                                if (ObjectManager.DeleteFile(IdentificacionComercio + "/SinProcesar/" + Path.GetFileName(Url)))
                                {

                                    LogOrchestation.WriteLog(string.Format("El archivo {0} fue Eliminado exitosamente a la carpeta SinProcesar", Path.GetFileName(Url)));
                                }
                                else
                                {
                                    ObjectManager.DeleteFile(IdentificacionComercio + "/Procesados/" + Path.GetFileName(Url));
                                    LogOrchestation.WriteLog(string.Format("El archivo {0} No fue eliminado de la carpeta SinProcesar", Path.GetFileName(Url)));
                                }
                            }
                            else
                            {
                                ObjectManager.DeleteFile(IdentificacionComercio + "/Procesados/" + Path.GetFileName(Url));
                                ListFilesError.Add(string.Format("No fue posible guardar la informacion del archivo {0} perteneciente al comercio " + IdentificacionComercio, Path.GetFileName(Url)));
                                LogOrchestation.WriteLog(string.Format("No fue posible guardar la informacion del archivo {0}.", Path.GetFileName(Url)));
                            }
                        }
                        else
                        {
                            ListFilesError.Add(string.Format("No fue posible subir el archivo {0} perteneciente al comercio " + IdentificacionComercio, Path.GetFileName(Url)));
                            LogOrchestation.WriteLog("El archivo no fue Subido exitosamente a la carpeta Procesados");
                        }

                        Archivo.Close();
                    }
                    catch (System.Exception ex)
                    {
                        ListFilesError.Add(string.Format("No fue posible procesar el archivo {0} perteneciente al comercio " + IdentificacionComercio + " Mensaje:" + ex.Message, Path.GetFileName(Url)));
                    }
                }
                foreach (var Nombre in ListaArchivosNoDescargados)
                {
                    ListFilesError.Add(string.Format("No fue posible subir el archivo {0} perteneciente al comercio " + IdentificacionComercio, Path.GetFileName(Url)));
                    LogOrchestation.WriteLog(string.Format("El archivo {0} no fue descargado exitosamente.", Nombre));
                }

                LogOrchestation.WriteLog("Fin de ejecucion orden de pedido");

                if (ListFilesError.Count == 0)
                    return true;
                else
                    return false;
            }
            catch (System.Exception Ex)
            {
                LogOrchestation.WriteLog("Error durante la ejecucion de de ejecucion orden de ProcessOrder " + Ex.Message);
                return false;
            }
        }

        public void StartsProcess()
        {
            ProcessOrder("Comercio1");
        }
    }
}