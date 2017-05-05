using EDIFACT;
using TextFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;

namespace Managers
{
    public class Manager
    {
        private ISftpAcess ISftpAccesObject;

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="UrlArchiveEdi"></param>
        /// <returns></returns>
        public XmlDocument ConvertEdiToXML(string UrlArchiveEdi)
        {
            if (File.Exists(UrlArchiveEdi))
            {
                EDIMessage msg = new EDIMessage(UrlArchiveEdi);
                XmlDocument[] xDoc = null;
                xDoc = msg.SerializeToXml();

                //string data = "";
                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xDoc[0].WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();
                    //data = stringWriter.GetStringBuilder().ToString();
                }
                XmlDocument archivoXML = xDoc[0];
                return archivoXML;
            }
            else
                return null;

            //var query = archivoXML.GetElementsByTagName("BGM").Item(0).Attributes[3].Value;
        }

        public object ProcessEdi(XmlDocument Document)
        {


            return null;
        }

        /// <summary>
        /// Metodo que autentica archivo con MD5 y retorna un string
        /// </summary>
        /// <param name="Route">ruta que incluye nombre y extension del archivo donde se encuentra el archivo ha autenticar</param>
        /// <returns>retorna hash del archivo autenticado </returns>
        public string AuthenticArchiveMD5(string Route)
        {
            if (File.Exists(Route))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(Route))
                    {
                        string HashArchivo = Convert.ToBase64String(md5.ComputeHash(stream));
                        LogManager.WriteLog(string.Format("El Archivo {0}  fue autenticado el hash correspondiente es {1}.", Path.GetFileName(Route), HashArchivo));
                        return HashArchivo;
                    }
                }
            }
            else
                return "";
        }

        public bool UploadFile(string NameFile, Stream File, string IdCommerce, string UrlFtpDestination)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.Uploadfile(NameFile, File, IdCommerce, UrlFtpDestination);
        }

        public bool DownloadFile(string UrlSourceFtp, string UrlLocalDestination)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.DownloadFile(UrlSourceFtp, UrlLocalDestination);
        }

        public List<string> DownloadFilesByFolder(string UrlSourceFtp, string UrlLocalDestination, out List<string> ErrorListing)
        {
            ISftpAccesObject = new SftpAcess();
            ErrorListing = new List<string>();
            return ISftpAccesObject.DownloadFilesByFolder(UrlSourceFtp, UrlLocalDestination, out ErrorListing);
        }

        public bool CreateFTPFolder(string NameFolder, string UrlDirectory)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.CreateFTPFolder(NameFolder, UrlDirectory);
        }

        public bool DeleteFile(string UrlOrigenFtp)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.DeleteFile(UrlOrigenFtp);
        }

        public bool ValidFileStructure(string Path)
        {

            try
            {
                bool BitValidFileStructure = true;

                IEnumerable<string> FullFile = File.ReadLines(Path);
                int LinesBGM = FullFile.Where(line => line.Contains("BGM+")).Count();
                if (LinesBGM > 1 || LinesBGM < 1)
                    BitValidFileStructure = false;

                if (BitValidFileStructure)
                {
                    int LinesNAD = FullFile.Where(line => line.Contains("NAD+")).Count();
                    if (LinesNAD < 3)
                        BitValidFileStructure = false;

                    if (BitValidFileStructure)
                    {
                        int LinesDTM = FullFile.Where(line => line.Contains("DTM+")).Count();
                        if (LinesDTM < 1)
                            BitValidFileStructure = false;

                        if (BitValidFileStructure)
                        {
                            int LinesLIN = FullFile.Where(line => line.Contains("LIN+")).Count();
                            if (LinesLIN < 1)
                                BitValidFileStructure = false;

                            if (BitValidFileStructure)
                            {
                                int LinesQTY = FullFile.Where(line => line.Contains("QTY+21")).Count();
                                if (LinesQTY < 1 || LinesQTY != LinesLIN)
                                    BitValidFileStructure = false;

                                if (BitValidFileStructure)
                                {
                                    int LinesUNS = FullFile.Where(line => line.Contains("UNS+")).Count();
                                    if (LinesUNS < 1 || LinesUNS > 1)
                                        BitValidFileStructure = false;
                                }
                            }
                        }
                    }
                }

                return BitValidFileStructure;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveDataEDI(string PathFile)
        {

            try
            {
                IEnumerable<string> FullFile = File.ReadLines(PathFile);
                XmlDocument ArchiveXML = new Manager().ConvertEdiToXML(PathFile);
                HeaderElement ObjectHeader = new HeaderElement();
                string LineBGM = string.Empty;
                bool ValidityFirstHierarchy = false;
                bool ValiditySecondHierarchy = false;
                List<string> ListAmountRequested = new List<string>();
                List<DetailElement> ListDetailElement = new List<DetailElement>();


                if (ArchiveXML.GetElementsByTagName("BGM").Count > 0)
                {
                    LineBGM = FullFile
                            .Where(line => line.Contains("BGM")).FirstOrDefault();

                    string[] ArrayLineBGM = LineBGM.Replace("BGM+", "").Replace("'", "").Split('+');
                    ObjectHeader.PurchaseOrderNumber = ArrayLineBGM[1];
                    int CuttingIndex = ArrayLineBGM[0].IndexOf(":");
                    if (CuttingIndex > 0)
                    {
                        ArrayLineBGM[0] = ArrayLineBGM[0].Substring(0, CuttingIndex);
                    }
                    ObjectHeader.TypeOfPurchaseOrder = ArrayLineBGM[0];
                }



                for (int i = 0; i < ArchiveXML.GetElementsByTagName("NAD").Count; i++)
                {
                    if (ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[0].Value.Equals("SU"))
                    {
                        ObjectHeader.Provider = ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[1].Value;
                    }
                    else if (ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[0].Value.Equals("BY"))
                    {
                        ObjectHeader.Commerce = ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[1].Value;
                    }
                    else if (ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[0].Value.Equals("DP"))
                    {
                        ObjectHeader.MerchandiseDeliverySite = ArchiveXML.GetElementsByTagName("NAD").Item(i).Attributes[1].Value;
                    }
                }


                for (int i = 0; i < ArchiveXML.GetElementsByTagName("DTM").Count; i++)
                {
                    if (ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[0].Value.Equals("2"))
                    {
                        ObjectHeader.DeadLine = ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[1].Value;
                        ValidityFirstHierarchy = true;
                    }
                    else if (ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[0].Value.Equals("63") && !ValidityFirstHierarchy)
                    {
                        ObjectHeader.DeadLine = ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[1].Value;
                        ValiditySecondHierarchy = true;
                    }
                    else if (ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[0].Value.Equals("64") && !ValidityFirstHierarchy && !ValiditySecondHierarchy)
                    {
                        ObjectHeader.DeadLine = ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[1].Value;
                    }

                    if (ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[0].Value.Equals("137"))
                    {
                        ObjectHeader.DateOfIssue = ArchiveXML.GetElementsByTagName("DTM").Item(i).Attributes[1].Value;
                    }
                }

                if (string.IsNullOrEmpty(ObjectHeader.DeadLine))
                {
                    int Index = 0;
                    int IndexSub = 0;

                    ObjectHeader.DeadLine = FullFile
                           .Where(line => line.Contains("DTM+43E")).FirstOrDefault();

                    Index = ObjectHeader.DeadLine.IndexOf(":");
                    IndexSub = ObjectHeader.DeadLine.IndexOf(":", Index + 1);
                    ObjectHeader.DeadLine = ObjectHeader.DeadLine.Substring(Index + 1, IndexSub - (Index + 1));
                }


                List<string> PrimeListAmountRequested = FullFile
                             .Where(line => line.Contains("QTY+21")).ToList();

                foreach (var item in PrimeListAmountRequested)
                {
                    ListAmountRequested.Add(item.Substring(item.IndexOf(":") + 1));
                }


                if (ArchiveXML.GetElementsByTagName("LIN").Count > 0)
                {
                    for (int i = 0; i < ArchiveXML.GetElementsByTagName("LIN").Count; i++)
                    {
                        string delimiter = string.Empty;
                        if (i.Equals(ArchiveXML.GetElementsByTagName("LIN").Count - 1))
                            delimiter = "UNS+";
                        else
                            delimiter = "LIN+" + (i + 2);

                        List<string> ListDetailElementLin = FullFile
                                       .SkipWhile(line => !line.Contains("LIN+" + (i + 1))).Skip(1).TakeWhile(line => !line.Contains(delimiter)).ToList();
                        ListDetailElementLin = ListDetailElementLin.Where(c => c.Contains("QTY+11") || c.Contains("LOC+7")).ToList();

                        if (ListDetailElementLin.Count > 0)
                        {
                            for (int b = 0; b < ListDetailElementLin.Count; b = b + 2)
                            {
                                DetailElement DetailElement = new DetailElement();
                                DetailElement.ProductEAN = ArchiveXML.GetElementsByTagName("LIN").Item(i).Attributes[2].Value;
                                DetailElement.AmountRequested = ListAmountRequested[i];

                                string Loc7 = string.Empty;

                                Loc7 = ListDetailElementLin[b].Replace("LOC+7+", "");
                                int Index = Loc7.IndexOf(":");
                                DetailElement.PointSale = Loc7.Substring(0, Index);
                                if (string.IsNullOrEmpty(DetailElement.PointSale))
                                    DetailElement.PointSale = ObjectHeader.MerchandiseDeliverySite;
                                DetailElement.QuantityPerPointOfSale = ListDetailElementLin[b + 1].Replace("QTY+11:", "").Replace("'", "");
                                ListDetailElement.Add(DetailElement);
                            }
                        }
                        else
                        {
                            DetailElement DetailElement = new DetailElement();
                            DetailElement.ProductEAN = ArchiveXML.GetElementsByTagName("LIN").Item(i).Attributes[2].Value;
                            DetailElement.AmountRequested = ListAmountRequested[i];
                            DetailElement.PointSale = ObjectHeader.MerchandiseDeliverySite;
                            DetailElement.QuantityPerPointOfSale = ListAmountRequested[i];
                            ListDetailElement.Add(DetailElement);
                        }
                    }
                }
                if (ListDetailElement.Count > 0 && !string.IsNullOrEmpty(ObjectHeader.PurchaseOrderNumber) && !string.IsNullOrEmpty(ObjectHeader.TypeOfPurchaseOrder) && !string.IsNullOrEmpty(ObjectHeader.DeadLine) && !string.IsNullOrEmpty(ObjectHeader.MerchandiseDeliverySite))
                    return true;
                else
                    return false;
            }
            catch
            {
                LogManager.WriteLog(string.Format("Se generaron inconvenientes en el procesamiento del archivo {0} ", Path.GetFileName(PathFile)));
                return false;
            }
        }

        #endregion Methods
    }
    public class DetailElement
    {
        public string ProductEAN { get; set; }
        public string AmountRequested { get; set; }
        public string PointSale { get; set; }
        public string QuantityPerPointOfSale { get; set; }
    }

    public class HeaderElement
    {
        public string PurchaseOrderNumber { get; set; }
        public string TypeOfPurchaseOrder { get; set; }
        public string Commerce { get; set; }
        public string Provider { get; set; }
        public string DateOfIssue { get; set; }
        public string DeadLine { get; set; }
        public string MerchandiseDeliverySite { get; set; }

    }
}