using Managers;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using log4net;
using System;
using Carvajal.Turns.CodeResponses;


namespace Orchestations
{
    public class Orchestation
    {
        /// <summary>
        /// Metodo que permite descargar los archivos de una carpeta especifica
        /// </summary>
        /// <param name="FileListonSFTP">Listado de archivos que se tiene el SFTP. Se tienen que reordenan para evitar problemas de integridad de datos</param>
        /// <returns>Retorna una lista con los archivos a procesar ordenados, en caso de no tener lista de entrada retorna una lista vacia</returns>
        public List<string> ReorderFiles(List<string> CompleteFileListonSFTP)
        {
            List<string> FileList = new List<string>();
            List<string> FileListonSFTP = CompleteFileListonSFTP;
            int i = 4; //Numero de pasos a realizar: Fabricantes, Centros de Entrega, Tiempos de Descarga, y finalmente Administradores y Operadores.
            while (i > 0)
            {
                foreach (string FileName in FileListonSFTP)
                {
                    switch (i)
                    {
                        case 4:                            
                            if (Path.GetFileName(FileName).StartsWith("Usuario_FA_")) { FileList.Add(FileName);  } break;
                        case 3:
                            if (Path.GetFileName(FileName).StartsWith("Centro_")) { FileList.Add(FileName); } break;
                        case 2:
                            if (Path.GetFileName(FileName).StartsWith("Tiempo_")) { FileList.Add(FileName); } break;
                        case 1:
                            if (Path.GetFileName(FileName).StartsWith("Usuario_CO_")) { FileList.Add(FileName); } break;
                    }
                }
                i--;
            }
            // Poner el resto de los archivos en el listado a procesar
            foreach (string FileName in FileListonSFTP)
            {
                if (!((Path.GetFileName(FileName).StartsWith("Usuario_CO_")) || (Path.GetFileName(FileName).StartsWith("Tiempo_")) || (Path.GetFileName(FileName).StartsWith("Centro_")) || (Path.GetFileName(FileName).StartsWith("Usuario_FA_"))))
                FileList.Add(FileName); 
            }
            return FileList;
        }
        public bool ProcessOrder(string MerchantIdentifier)
        {
            try
            {
                Manager ObjectManager = new Manager();
                int WrongRecords = 0, TotalRecords = 0;
                LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A1 + MerchantIdentifier);
                //TODO Si no existe el directorio entonces hacer la notificacion y alarma de auditoria
                if (!Directory.Exists(ConfigurationManager.AppSettings["Url"] + "\\" + MerchantIdentifier))
                {
                    // DirectoryInfo di = Directory.CreateDirectory(ConfigurationManager.AppSettings["Url"] + "\\" + MerchantIdentifier);
                    LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.A28 + MerchantIdentifier);
                    return false;
                }
                List<string> NoDownloadFileList = new List<string>();
                List<string> TempFileList = ObjectManager.DownloadFilesByFolder(MerchantIdentifier + "/SinProcesar", ConfigurationManager.AppSettings["Url"] + "\\" + MerchantIdentifier, out NoDownloadFileList);
                List<string> DownloadFileList = ReorderFiles(TempFileList);
                List<string> ListFilesOK = new List<string>();
                List<string> ListFilesError = new List<string>();
                int CurrentFileCounter = 0;
                foreach (string Url in DownloadFileList)
                {
                    try
                    {
                        if (File.ReadAllLines(Url).Length == 1)
                        {
                            string Content = File.ReadAllText(Url);
                            Content = Content.Replace("'", "'" + System.Environment.NewLine);
                            File.WriteAllText(Url, Content);
                        }

                        ObjectManager.AuthenticArchiveMD5(MerchantIdentifier, Url);
                        CurrentFileCounter++;
                        if (Path.GetFileName(Url).Contains("RECADV") || Path.GetFileName(Url).Contains("ORDERS"))
                        {
                            if (!Path.GetExtension(Url).Equals(".edi"))
                            {
                                LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.M64 + ". Nombre Archivo: " + Path.GetFileName(Url));
                                continue;
                            }
                            if (!ObjectManager.ValidEDIFileStructure(Url, MerchantIdentifier))
                            {
                                LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.M45 + Path.GetFileName(Url) + Responses.M45_1 + Responses.A5);
                                continue;
                            }
                        }
                        else if (Path.GetFileName(Url).StartsWith("Usuario_CO_") || Path.GetFileName(Url).StartsWith("Usuario_FA_") || Path.GetFileName(Url).StartsWith("Centro_") || Path.GetFileName(Url).StartsWith("Tiempo_"))
                        {
                            if (!Path.GetExtension(Url).Equals(".csv"))
                            {
                                LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.M46 + ". Nombre Archivo: " + Path.GetFileName(Url));
                                continue;
                            }
                            if (!ObjectManager.ValidCSVFileStructure(Url, MerchantIdentifier, out WrongRecords, out TotalRecords))
                            {
                                LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.M45 + Path.GetFileName(Url) + Responses.M45_1);
                                continue;
                            }
                        }
                        else
                        {
                            LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.A2 + Path.GetFileName(Url) + Responses.A2_1);
                            continue;
                        }

                        Stream PlainTextFile = File.OpenRead(Url);
                        if (ObjectManager.UploadFile(Path.GetFileName(Url), PlainTextFile, MerchantIdentifier, "Procesados"))
                        {
                            PlainTextFile.Close();
                            bool ValidSaveData = false;
                            if (Path.GetFileName(Url).Contains("RECADV"))
                            {
                                ValidSaveData = ObjectManager.SaveDataRecAdvEDI(MerchantIdentifier, Url);
                            }
                            else if (Path.GetFileName(Url).Contains("ORDERS"))
                            {
                                ValidSaveData = ObjectManager.SaveDataOrdersEDI(MerchantIdentifier, Url);
                            }
                            else if (Path.GetFileName(Url).Contains("Usuario_CO_"))
                            {
                                ValidSaveData = ObjectManager.SaveDataMerchantUsersCSV(MerchantIdentifier, Url);
                            }
                            else if (Path.GetFileName(Url).Contains("Usuario_FA_"))
                             {
                                 ValidSaveData = ObjectManager.SaveDataManufacturerUsersCSV(MerchantIdentifier, Url);
                             }
                              else if (Path.GetFileName(Url).Contains("Centro_"))
                             {

                                ValidSaveData = ObjectManager.SaveDataCentreCSV(MerchantIdentifier, Url);
                             }
                             else if (Path.GetFileName(Url).Contains("Tiempo_"))
                             {
                                 ValidSaveData = ObjectManager.SaveDataUnloadingTimeCSV(MerchantIdentifier, Url);
                             }

                            if (ValidSaveData)
                            {
                                LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A2 + Path.GetFileName(Url) + Responses.A32);
                                ListFilesOK.Add(string.Format("El archivo {0} perteneciente al comercio " + MerchantIdentifier + " fue Subido exitosamente a la carpeta Procesados y fue guardado exitosamente", Path.GetFileName(Url)));
                                if (ObjectManager.DeleteFile(MerchantIdentifier + "/SinProcesar/" + Path.GetFileName(Url)))
                                {

                                    LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A2 + Path.GetFileName(Url) + Responses.A34);
                                }
                                else
                                {
                                    ObjectManager.DeleteFile(MerchantIdentifier + "/Procesados/" + Path.GetFileName(Url));
                                    LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.A2 + Path.GetFileName(Url) + Responses.A35);
                                }
                            }
                            else
                            {
                                ObjectManager.DeleteFile(MerchantIdentifier + "/Procesados/" + Path.GetFileName(Url));
                                ListFilesError.Add(string.Format("No fue posible guardar la informacion del archivo {0} perteneciente al comercio " + MerchantIdentifier, Path.GetFileName(Url)));
                                LogOrchestation.WriteWarn(MerchantIdentifier, "0", Responses.A36 + Path.GetFileName(Url));
                            }
                        }
                        else
                        {
                            ListFilesError.Add(string.Format("No fue posible subir el archivo {0} perteneciente al comercio " + MerchantIdentifier, Path.GetFileName(Url)));
                            LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A37 + Path.GetFileName(Url));
                        }

                        PlainTextFile.Close();
                    }
                    catch (System.Exception ex)
                    {
                        ListFilesError.Add(string.Format("No fue posible procesar el archivo {0} perteneciente al comercio " + MerchantIdentifier + " Mensaje:" + ex.Message, Path.GetFileName(Url)));
                    }
                }
                foreach (var Nombre in NoDownloadFileList)
                {
                    //ListFilesError.Add(string.Format("No fue posible subir el archivo {0} perteneciente al comercio " + MerchantIdentifier, Path.GetFileName(Url)));
                    LogOrchestation.WriteLog(MerchantIdentifier, "0", string.Format("El archivo {0} no fue descargado exitosamente.", Nombre));
                }

                LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A29 + MerchantIdentifier);

                if (ListFilesError.Count == 0)
                    return true;
                else
                    return false;
            }
            catch (System.Exception Ex)
            {
                LogOrchestation.WriteLog(MerchantIdentifier, "0", Responses.A38 + Ex.Message);
                return false;
            }
        }

        public void StartsProcess()
        {
            ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            //TODO Aqui viene la obtencion de los Identificadores de los comercios activos para ser la ejecucion en linea
            string path = String.Format("{0}_{1}_{2}{3}{4}{5}", "LOG_TURNOS", "33708218079", DateTime.Now.Day.ToString("d2"), DateTime.Now.Month.ToString("d2"), DateTime.Now.Year.ToString(), ".log");
            ChangeFilePath("MyRollingFileAppender", path);
            
            ProcessOrder("33708218079");
        }
        public static void ChangeFilePath(string appenderName, string newFilename)
        {
            // Recordar agregar el [assembly: log4net.Config.XmlConfiguratorAttribute(Watch = true)] y en App.config para el Log4net
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.GetRepository();
            try
            {
                foreach (log4net.Appender.IAppender appender in repository.GetAppenders())
                {
                    if (appender.Name.CompareTo(appenderName) == 0 && appender is log4net.Appender.FileAppender)
                    {
                        log4net.Appender.FileAppender fileAppender = (log4net.Appender.FileAppender)appender;
                        fileAppender.File = System.IO.Path.Combine(fileAppender.File, newFilename);
                        fileAppender.ActivateOptions();
                    }
                }
            }
            catch (Exception ex)
            {
                LogOrchestation.WriteError("0", "0", Responses.A4 + ex.Message);
            }
        }


    }
}