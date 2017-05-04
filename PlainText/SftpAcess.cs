using PlainText.Resources;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlainText
{
    public class SftpAcess : ISftpAcess
    {
        #region Propiedades

        private string User { get { return ConfigurationManager.AppSettings["User"]; } }
        private string Password { get { return ConfigurationManager.AppSettings["Password"]; } }
        private string UrlSftp { get { return ConfigurationManager.AppSettings["UrlSftp"]; } }
        private string Port { get { return ConfigurationManager.AppSettings["Port"]; } }

        #endregion Propiedades

        #region Metodos

        /// <summary>
        /// Metodo para subir archivos al sftp
        /// </summary>
        /// <param name="NameFile">Nombre del archivo incluyendo extension</param>
        /// <param name="File">Stream del archivo original</param>
        /// <param name="IdCommerce">id asignado a la carpeta que pertenece al cliente</param>
        /// <param name="UrlFtpDestination">la ruta en el ftp donde se entregara el archivo.</param>
        /// <returns>true si el archivo fue correctamente cargado en caso contrario false</returns>
        public bool Uploadfile(string NameFile, Stream File, string IdCommerce, string UrlFtpDestination)
        {
            try
            {
                using (SftpClient Sftp = new SftpClient(UrlSftp, Convert.ToInt32(Port), User, Password))
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        if (!Sftp.Exists(UrlFtpDestination))
                        {
                            Sftp.ChangeDirectory(@"./" + IdCommerce + "/" + UrlFtpDestination + "/");
                            Sftp.BufferSize = 4 * 1024;
                            Sftp.UploadFile(File, NameFile);
                            Sftp.Disconnect();
                            return true;
                        }
                        else
                        {
                            Sftp.Disconnect();
                            LogHelper.WriteLog(Messages.E0002);
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(Messages.E0001);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Metodo para descargar un archivo especifico
        /// </summary>
        /// <param name="UrlOrigenFtp">url sin la direccion del servidor sftp y con el nombre y extension del archivo </param>
        /// <param name="UrlDestinoLocal">direccion de donde se almacenara el archivo</param>
        /// <returns>true si el archivo fue correctamente descargado en caso contrario false</returns>
        public bool DownloadFile(string UrlSourceFtp, string UrlLocalDestination)
        {
            try
            {
                if (!UrlSourceFtp.Contains("."))
                {
                    LogHelper.WriteLog(Messages.E0003 + " " + UrlSourceFtp);
                    return false;
                }

                using (SftpClient Sftp = new SftpClient(UrlSftp, Convert.ToInt32(Port), User, Password))
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        UrlSourceFtp = UrlSourceFtp.Replace(UrlSftp, "");

                        if (Sftp.Exists(UrlSourceFtp))
                        {
                            foreach (Renci.SshNet.Sftp.SftpFile ftpfile in Sftp.ListDirectory(UrlSourceFtp))
                            {
                                using (FileStream fs = new FileStream(UrlLocalDestination + "\\" + ftpfile.Name, FileMode.Create))
                                {
                                    Sftp.DownloadFile(UrlSourceFtp, fs);
                                    fs.Close();
                                }
                            }
                            Sftp.Disconnect();
                            return true;
                        }
                        else
                        {
                            Sftp.Disconnect();
                            LogHelper.WriteLog(Messages.E0002);
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(Messages.E0001);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Error " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Metodo para descargar un archivo especifico
        /// </summary>
        /// <param name="UrlOrigenFtp">url sin la direccion del servidor sftp y con el nombre de la carpeta de la cual se descargaran todos los archivos</param>
        /// <param name="UrlDestinoLocal">direccion de donde se almacenara el archivo</param>
        /// <returns>true si los archivos fueron correctamente descargados en caso contrario false</returns>
        public List<string> DownloadFilesByFolder(string UrlSourceFtp, string UrlLocalDestination, out List<string> ErrorListing)
        {
            List<string> ListadoDescargados = new List<string>();
            ErrorListing = new List<string>();
            try
            {
                if (UrlSourceFtp.Contains("."))
                {
                    LogHelper.WriteLog(Messages.E0004 + " " + UrlSourceFtp);
                    return ListadoDescargados;
                }

                using (SftpClient Sftp = new SftpClient(UrlSftp, Convert.ToInt32(Port), User, Password))
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        UrlSourceFtp = UrlSourceFtp.Replace(UrlSftp, "");

                        if (Sftp.Exists(UrlSourceFtp))
                        {
                            foreach (Renci.SshNet.Sftp.SftpFile ftpfile in Sftp.ListDirectory(UrlSourceFtp))
                            {
                                try
                                {
                                    using (FileStream fs = new FileStream(UrlLocalDestination + "\\" + ftpfile.Name, FileMode.Create))
                                    {
                                        Sftp.DownloadFile(UrlSourceFtp + "//" + ftpfile.Name, fs);
                                        fs.Close();
                                        ListadoDescargados.Add(UrlLocalDestination + "\\" + ftpfile.Name);
                                    }
                                }
                                catch
                                {
                                    ErrorListing.Add(ftpfile.Name);
                                    LogHelper.WriteLog(string.Format(Messages.M0001, ftpfile.FullName));
                                }
                            }
                            Sftp.Disconnect();
                            return ListadoDescargados;
                        }
                        else
                        {
                            Sftp.Disconnect();
                            LogHelper.WriteLog(Messages.E0002);
                            return ListadoDescargados;
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(Messages.E0001);
                        return ListadoDescargados;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("Error " + ex.Message);
                ErrorListing.Add("Error " + ex.Message);
                return ListadoDescargados;
            }
        }

        /// <summary>
        /// Permite crear una carpeta en el sftp
        /// </summary>
        /// <param name="NombreCarpeta">Nombre de la carpeta que se creara</param>
        /// <param name="UrlDirectorio">Url donde se creara la carpeta en el Sftp</param>
        /// <returns>true si la carpeta fue correctamente creada en caso contrario false</returns>
        public bool CreateFTPFolder(string NameFolder, string UrlDirectory)
        {
            try
            {
                using (SftpClient Sftp = new SftpClient(UrlSftp, Convert.ToInt32(Port), User, Password))
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        if (!Sftp.Exists(NameFolder))
                        {
                            string UrlCarpeta = string.IsNullOrEmpty(UrlDirectory) ? "../" + NameFolder : UrlDirectory + "/" + NameFolder;
                            Sftp.CreateDirectory(UrlCarpeta);
                            if (Sftp.Exists(NameFolder))
                                return true;
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                    else
                    {
                        LogHelper.WriteLog(Messages.E0001);
                        return false;
                    }
                }
            }
            catch
            {
                LogHelper.WriteLog(Messages.E0001);
                return false;
            }
        }

        /// <summary>
        /// Metodo que elimina archivo especifico en el sftp
        /// </summary>
        /// <param name="UrlOriginFtp">ubicacion actual del archivo en el Sftp sin la direccion del servidor sftp y con el nombre y extension del archivo</param>
        /// <returns>true si el archivo fue correctamente eliminado en caso contrario false</returns>
        public bool DeleteFile(string UrlOriginFtp)
        {
            try
            {
                using (SftpClient Sftp = new SftpClient(UrlSftp, Convert.ToInt32(Port), User, Password))
                {
                    Sftp.Connect();
                    if (Sftp.IsConnected)
                    {
                        if (Sftp.Exists(UrlOriginFtp))
                        {
                            Sftp.Delete(UrlOriginFtp);
                            if (!Sftp.Exists(UrlOriginFtp))
                            {
                                Sftp.Disconnect();
                                return true;
                            }
                            else
                            {
                                Sftp.Disconnect();
                                return false;
                            }
                        }
                        else
                        {
                            Sftp.Disconnect();
                            LogHelper.WriteLog(Messages.E0002);
                            return false;
                        }
                    }
                    else
                    {
                        LogHelper.WriteLog(Messages.E0001);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return false;
            }
        }

        #endregion Metodos
    }

    public class SftpObject
    {
        public string NameFile { get; set; }
        public Stream File { get; set; }
        public string IdCommerce { get; set; }
        public string NameFolder { get; set; }
        public string UrlOrigin { get; set; }
        public string UrlDestination { get; set; }
    }
}
