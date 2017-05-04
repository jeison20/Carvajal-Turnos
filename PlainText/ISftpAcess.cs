using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlainText
{
    public interface ISftpAcess
    {
        bool Uploadfile(string NameFile, Stream File, string IdCommerce, string UrlFtpDestination);
        bool DownloadFile(string UrlSourceFtp, string UrlLocalDestination);
        List<string> DownloadFilesByFolder(string UrlSourceFtp, string UrlLocalDestination, out List<string> ErrorListing);
        bool CreateFTPFolder(string NameFolder, string UrlDirectory);
        bool DeleteFile(string UrlOrigenFtp);
    }
}
