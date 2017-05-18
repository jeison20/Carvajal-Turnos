using Carvajal.Shifts.Data;
using EDIFACT;
using Managers.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using TextFile;

namespace Managers
{
    public class Manager
    {
        private ISftpAcess ISftpAccesObject;

        #region Methods

        /// <summary>
        ///Metodo encargado de convertir el .Edi a xml
        /// </summary>
        /// <param name="UrlArchiveEdi">ruta fisica donde se encuentra el archivo a convertir </param>
        /// <returns>retorna un XmlDocument si la operacion fue exitosa en caso contrario retorna un null</returns>
        public XmlDocument ConvertEdiToXML(string UrlArchiveEdi)
        {
            if (File.Exists(UrlArchiveEdi))
            {
                EDIMessage msg = new EDIMessage(UrlArchiveEdi);
                XmlDocument[] xDoc = null;
                xDoc = msg.SerializeToXml();

                using (var stringWriter = new StringWriter())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xDoc[0].WriteTo(xmlTextWriter);

                    xmlTextWriter.Flush();
                }
                XmlDocument archivoXML = xDoc[0];
                return archivoXML;
            }
            else
                return null;
        }

        /// <summary>
        /// Metodo que autentica archivo con MD5 y retorna un string
        /// </summary>
        /// <param name="Route">ruta que incluye nombre y extension del archivo donde se encuentra el archivo ha autenticar</param>
        /// <returns>retorna hash del archivo autenticado si el proceso fue exitoso en caso contrario retorna un string vacio </returns>
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

        /// <summary>
        /// Metodo que permite subir un archivo al sftp 
        /// </summary>
        /// <param name="NameFile">Nombre del archivo</param>
        /// <param name="File">Archivo convertido a stream </param>
        /// <param name="IdCommerce">Id unico del comercio</param>
        /// <param name="UrlFtpDestination">Url del lugar donde se guardara el archivo</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool UploadFile(string NameFile, Stream File, string IdCommerce, string UrlFtpDestination)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.Uploadfile(NameFile, File, IdCommerce, UrlFtpDestination);
        }

        /// <summary>
        /// Metodo que permite descargar archivos del sftp
        /// </summary>
        /// <param name="UrlSourceFtp">url incluyendo el nombre y extencion del archivo a descargar</param>
        /// <param name="UrlLocalDestination">url de la ruta donde se descargara el archivo </param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool DownloadFile(string UrlSourceFtp, string UrlLocalDestination)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.DownloadFile(UrlSourceFtp, UrlLocalDestination);
        }
        /// <summary>
        /// Metodo que permite descargar los archivos de una carpeta especifica
        /// </summary>
        /// <param name="UrlSourceFtp">url de la carpeta en sftp</param>
        /// <param name="UrlLocalDestination">Ruta fisica donde se guardaran los archivos</param>
        /// <param name="ErrorListing">lista de archivos que presentaron errores durante el proceso de descarga</param>
        /// <returns>Retorna una lista con los archivos que se descargaron exitosamente en caso de no descargar ninguno retorna una lista vacia</returns>
        public List<string> DownloadFilesByFolder(string UrlSourceFtp, string UrlLocalDestination, out List<string> ErrorListing)
        {
            ISftpAccesObject = new SftpAcess();
            ErrorListing = new List<string>();
            return ISftpAccesObject.DownloadFilesByFolder(UrlSourceFtp, UrlLocalDestination, out ErrorListing);
        }
        /// <summary>
        /// /Metodo que permite crear una carpeta en una ruta en el sftp
        /// </summary>
        /// <param name="NameFolder">Nombre de la carpeta que se creara</param>
        /// <param name="UrlDirectory">direccion donde se creara la carpeta</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool CreateFTPFolder(string NameFolder, string UrlDirectory)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.CreateFTPFolder(NameFolder, UrlDirectory);
        }
        /// <summary>
        /// Metodo que permite eliminar un archivo en el sftp
        /// </summary>
        /// <param name="UrlOrigenFtp">ruta completa </param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool DeleteFile(string UrlOrigenFtp)
        {
            ISftpAccesObject = new SftpAcess();
            return ISftpAccesObject.DeleteFile(UrlOrigenFtp);
        }
        /// <summary>
        /// Metodo que valida la estructura del archivo  edi
        /// </summary>
        /// <param name="Path">ruta  y nombre  del archivo con su respectiva extension </param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
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
                                int LinesQTY = FullFile.Where(line => line.Contains("QTY+")).Count();
                                if (LinesQTY < 1)
                                    BitValidFileStructure = false;

                                if (BitValidFileStructure)
                                {
                                    int LinesUNZ = FullFile.Where(line => line.Contains("UNZ")).Count();
                                    if (LinesUNZ < 1 || LinesUNZ > 1)
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
        /// <summary>
        /// Metodo que guarda la informacion del archivo orden de pedido Edi
        /// </summary>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataEDI(string PathFile)
        {
            try
            {
                HeaderElement ObjectHeader = new HeaderElement();
                ObjectHeader = ReadOrderEdi(PathFile);

                if (ObjectHeader.Details.Count > 0 && !string.IsNullOrEmpty(ObjectHeader.PurchaseOrderNumber) && !string.IsNullOrEmpty(ObjectHeader.TypeOfPurchaseOrder) && !string.IsNullOrEmpty(ObjectHeader.DeadLine) && !string.IsNullOrEmpty(ObjectHeader.MerchandiseDeliverySite))
                {
                    try
                    {
                        Users UserProvider = CUsers.Instance.SearchUser(ObjectHeader.Provider);
                        Users UserMerchant = CUsers.Instance.SearchUser(ObjectHeader.Commerce);
                        if (UserMerchant == null)
                        {
                            LogManager.WriteLog(string.Format("Error el Comercio con identificaciòn {0} no se encuentra registrado", ObjectHeader.Commerce));
                            return false;
                        }

                        if (UserProvider == null)
                        {
                            LogManager.WriteLog(string.Format("Error el Proveedor con identificaciòn {0} no se encuentra registrado", ObjectHeader.Commerce));
                            return false;
                        }

                        Orders ObjectOrders = new Orders();
                        ObjectOrders.OrderNumber = ObjectHeader.PurchaseOrderNumber;
                        ObjectOrders.MaxDeliveryDate = ConvertDateTimeEdi(ObjectHeader.DeadLine);
                        ObjectOrders.OrderType = ObjectHeader.TypeOfPurchaseOrder;
                        ObjectOrders.FkStatus_Identifier = 1;
                        ObjectOrders.FkUsers_Merchant_Identifier = UserMerchant.PkIdentifier;
                        ObjectOrders.FkUsers_Manufacturer_Identifier = UserProvider.PkIdentifier;
                        ObjectOrders.LastChangeDate = ConvertDateTimeEdi(ObjectHeader.DateOfIssue);
                        ObjectOrders.ProcessingDate = DateTime.Now;
                        if (COrders.Instance.SaveOrders(ObjectOrders))
                        {
                            foreach (DetailElement Detail in ObjectHeader.Details)
                            {
                                OrdersProducts Product = new OrdersProducts();
                                Product.FkOrders_Identifier = ObjectOrders.PkIdentifier;
                                Product.Description = Detail.Description;
                                Product.Code = Detail.ProductEAN;
                                Product.SplitQuantity = Convert.ToInt64(Detail.QuantityPerPointOfSale);
                                Product.Line = Convert.ToInt32(Detail.Line);
                                if (!COrdersProducts.Instance.SaveOrdersProducts(Product))
                                {
                                    LogManager.WriteLog("Error al tratar de guardar elemento OrdersProducts: " + JsonConvert.SerializeObject(Product));
                                    List<OrdersProducts> ListProductInserted = COrdersProducts.Instance.SearchOrdersProducts(ObjectOrders.PkIdentifier);
                                    foreach (var item in ListProductInserted)
                                    {
                                        COrdersProducts.Instance.DeleteOrdersProducts(item);
                                    }
                                    COrders.Instance.DeleteOrder(ObjectOrders);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            LogManager.WriteLog("Error al tratar de guardar elemento Orders: " + JsonConvert.SerializeObject(ObjectOrders));
                            return false;
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog("Error al durante el guarde de elemento Orders: " + ex.Message);
                        return false;
                    }
                }
                else
                {
                    LogManager.WriteLog("Se presento problemas con los datos obligatorios suministrados ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(string.Format("Se generaron inconvenientes en el Guarde del archivo {0} " + ex.Message, Path.GetFileName(PathFile)));
                return false;
            }
        }
        /// <summary>
        /// Metodo que guarda la informacion del archivo Recibo de pedido Edi 
        /// </summary>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataRecAdvEDI(string PathFile)
        {
            try
            {
                HeaderElement ObjectHeader = new HeaderElement();

                ObjectHeader = ReadRecAdvEdi(PathFile);

                Orders AssignedOrder = COrders.Instance.SearchOrder(ObjectHeader.PurchaseOrderNumber);
                if (AssignedOrder == null)
                {
                    LogManager.WriteLog("Error no existe una orden para el numero de orden " + ObjectHeader.PurchaseOrderNumber + " en el archivo " + Path.GetFileName(PathFile));
                    return false;
                }

                Turns Turn = CTurns.Instance.SearchTurnsForOrderActive(ObjectHeader.PurchaseOrderNumber);
                if (Turn == null)
                {
                    LogManager.WriteLog("Error no existe un turno para el numero de orden " + ObjectHeader.PurchaseOrderNumber + " en el archivo " + Path.GetFileName(PathFile));
                    return false;
                }

                if (ObjectHeader.Details.Count > 0 && !string.IsNullOrEmpty(ObjectHeader.PurchaseOrderNumber) && !string.IsNullOrEmpty(ObjectHeader.ReceiptWarningNumber) && !string.IsNullOrEmpty(ObjectHeader.DeadLine) && !string.IsNullOrEmpty(ObjectHeader.MerchandiseDeliverySite))
                {
                    try
                    {
                        Users UserProvider = CUsers.Instance.SearchUser(ObjectHeader.Provider);
                        Users UserMerchant = CUsers.Instance.SearchUser(ObjectHeader.Commerce);
                        Advices ObjectAdvicesProducts = new Advices();
                        ObjectAdvicesProducts.FkUsers_Merchant_Identifier = UserMerchant.PkIdentifier;
                        ObjectAdvicesProducts.FkUsers_Manufacturer_Identifier = UserProvider.PkIdentifier;

                        ObjectAdvicesProducts.AdviceNumber = ObjectHeader.ReceiptWarningNumber;
                        ObjectAdvicesProducts.Orders_OrderNumber = ObjectHeader.PurchaseOrderNumber;
                        ObjectAdvicesProducts.ReceiptDate = ConvertDateTimeEdi(ObjectHeader.DeadLine);
                        ObjectAdvicesProducts.ManualAdvise = false;
                        ObjectAdvicesProducts.ProcessingDate = DateTime.Now;
                        Centres Center = CCentres.Instance.SearchCentresForId(ObjectHeader.MerchandiseDeliverySite);
                        if (Center == null)
                        {
                            LogManager.WriteLog("Error durante el procesamiento del RecAdv el centro enviado no existe  " + ObjectHeader.MerchandiseDeliverySite);
                            return false;
                        }
                        ObjectAdvicesProducts.FkCentres_Identifier = Center.PkIdentifier;

                        if (CAdvices.Instance.SaveAdvices(ObjectAdvicesProducts))
                        {
                            foreach (var item in ObjectHeader.Details)
                            {
                                AdvicesProducts ObjectAdvicesProduct = new AdvicesProducts();
                                ObjectAdvicesProduct.FkAdvices_Identifier = ObjectAdvicesProducts.PkIdentifier;
                                ObjectAdvicesProduct.Code = item.ProductEAN;
                                //ObjectAdvicesProduct.Description = item.Description;
                                ObjectAdvicesProduct.ReceivedAndAcceptedQuantity = Convert.ToInt64(item.AmountReceivedAccepted);
                                if (!CAdvicesProducts.Instance.SaveAdvicesProduct(ObjectAdvicesProduct))
                                {
                                    LogManager.WriteLog("Error al tratar de guardar elemento AdvicesProducts: " + JsonConvert.SerializeObject(ObjectAdvicesProduct));
                                    List<AdvicesProducts> ListProductInserted = CAdvicesProducts.Instance.SearchAdvicesProducts(ObjectAdvicesProducts.PkIdentifier);
                                    foreach (var itemProductInsert in ListProductInserted)
                                    {
                                        CAdvicesProducts.Instance.DeleteAdvicesProduct(itemProductInsert);
                                    }
                                    CAdvices.Instance.DeleteAdvices(ObjectAdvicesProducts);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            LogManager.WriteLog("Error al tratar de guardar elemento Advices: " + JsonConvert.SerializeObject(ObjectAdvicesProducts));
                            return false;
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog("Error  durante el guarde del RecAdv " + ex.Message);
                        return false;
                    }
                }
                else
                {
                    LogManager.WriteLog("Se presento problemas con los datos obligatorios suministrados ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(string.Format("Se generaron inconvenientes en el procesamiento RecAdv del archivo {0} " + ex.Message, Path.GetFileName(PathFile)));
                return false;
            }
        }
        /// <summary>
        /// Metodo que completa con ceros a la derecha un texto 
        /// </summary>
        /// <param name="Length">Extencion maxima de la cadena</param>
        /// <param name="Line">cararcteres que van a la izquierda de los ceros</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public string CompleteZeros(int Length, string Line)
        {
            string CountZeros = string.Empty;
            for (int i = 0; i < Length; i++)
            {
                CountZeros += "0";
            }
            if (Length > Line.Length)
                CountZeros = CountZeros.Substring(0, CountZeros.Length - Line.Length) + Line;
            else
                CountZeros = Line;

            return CountZeros;
        }
        /// <summary>
        /// Metodo que lee y extrae la informacion del archivo edi de una orden de pedido
        /// </summary>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public HeaderElement ReadOrderEdi(string PathFile)
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
                                DetailElement.Line = ArchiveXML.GetElementsByTagName("LIN").Item(i).Attributes[0].Value;
                                DetailElement.AmountRequested = ListAmountRequested[i];

                                string Loc7 = string.Empty;

                                Loc7 = ListDetailElementLin[b].Replace("LOC+7+", "");
                                int Index = Loc7.IndexOf(":");
                                DetailElement.PointSale = Loc7.Substring(0, Index);
                                if (string.IsNullOrEmpty(DetailElement.PointSale))
                                    DetailElement.PointSale = ObjectHeader.MerchandiseDeliverySite;
                                DetailElement.QuantityPerPointOfSale = ListDetailElementLin[b + 1].Replace("QTY+11:", "").Replace("'", "");

                                if (!string.IsNullOrEmpty(DetailElement.ProductEAN) && !string.IsNullOrEmpty(DetailElement.AmountRequested) && !string.IsNullOrEmpty(DetailElement.PointSale) && !string.IsNullOrEmpty(DetailElement.QuantityPerPointOfSale))
                                    ListDetailElement.Add(DetailElement);
                            }
                        }
                        else
                        {
                            DetailElement DetailElement = new DetailElement();
                            DetailElement.ProductEAN = ArchiveXML.GetElementsByTagName("LIN").Item(i).Attributes[2].Value;
                            DetailElement.AmountRequested = ListAmountRequested[i];
                            DetailElement.Line = ArchiveXML.GetElementsByTagName("LIN").Item(i).Attributes[0].Value;
                            DetailElement.PointSale = ObjectHeader.MerchandiseDeliverySite;
                            DetailElement.QuantityPerPointOfSale = ListAmountRequested[i].Replace("'", "");
                            if (!string.IsNullOrEmpty(DetailElement.ProductEAN) && !string.IsNullOrEmpty(DetailElement.AmountRequested) && !string.IsNullOrEmpty(DetailElement.PointSale) && !string.IsNullOrEmpty(DetailElement.QuantityPerPointOfSale))
                                ListDetailElement.Add(DetailElement);
                        }
                    }
                    ObjectHeader.Details = ListDetailElement;
                }
                return ObjectHeader;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Errores durante la lectura del Edi " + ex.Message);
                return new HeaderElement();
            }
        }
        /// <summary>
        /// Metodo que lee y extrae la informacion del archivo edi de una Recibo de pedido
        /// </summary>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio </returns>
        public HeaderElement ReadRecAdvEdi(string PathFile)
        {
            try
            {
                HeaderElement ObjectHeader = new HeaderElement();
                List<DetailElement> ListDetail = new List<DetailElement>();

                IEnumerable<string> FullFile = File.ReadLines(PathFile);
                string LineBGM = string.Empty;

                LineBGM = FullFile
                        .Where(line => line.Contains("BGM")).FirstOrDefault();

                string[] ArrayLineBGM = LineBGM.Replace("BGM+", "").Replace("'", "").Split('+');
                string PurchaseOrderNumber = ArrayLineBGM[1];
                if (PurchaseOrderNumber.Contains(":"))
                {
                    PurchaseOrderNumber = PurchaseOrderNumber.Substring(0, PurchaseOrderNumber.IndexOf(":"));
                }

                int CuttingIndex = ArrayLineBGM[0].IndexOf(":");
                if (CuttingIndex > 0)
                {
                    ArrayLineBGM[0] = ArrayLineBGM[0].Substring(0, CuttingIndex);
                }
                ObjectHeader.ReceiptWarningNumber = ArrayLineBGM[0];

                IEnumerable<string> LinesNAD = FullFile
                        .Where(line => line.Contains("NAD"));

                string LineNadBy = LinesNAD.Where(c => c.Contains("NAD+BY")).FirstOrDefault();
                ObjectHeader.Commerce = LineNadBy.Replace("NAD+BY", "").Split('+')[1].Split(':')[0];

                string LineNadSu = LinesNAD.Where(c => c.Contains("NAD+SU")).FirstOrDefault();
                ObjectHeader.Provider = LineNadSu.Replace("NAD+SU", "").Split('+')[1].Split(':')[0];

                string LineNadDp = LinesNAD.Where(c => c.Contains("NAD+DP")).FirstOrDefault();
                ObjectHeader.MerchandiseDeliverySite = LineNadDp.Replace("NAD+DP", "").Split('+')[1].Split(':')[0];

                ObjectHeader.DeadLine = FullFile.FirstOrDefault(line => line.Contains("DTM"));
                ObjectHeader.DeadLine = ObjectHeader.DeadLine.Split(':')[1];

                string LineReff = FullFile.FirstOrDefault(line => line.Contains("RFF+ON"));
                LineReff = LineReff.Split(':')[1];
                ObjectHeader.PurchaseOrderNumber = LineReff.Replace("'", "");

                List<string> ListCountLinesLIN = FullFile
                       .Where(line => line.Contains("LIN+")).ToList();

                for (int i = 0; i < ListCountLinesLIN.Count; i++)
                {
                    DetailElement Detail = new DetailElement();

                    string delimiter = string.Empty;
                    if (i.Equals(ListCountLinesLIN.Count - 1))
                        delimiter = "UNZ+";
                    else
                        delimiter = "LIN+" + CompleteZeros(4, (i + 2).ToString());
                    string LineLin = ListCountLinesLIN[i];
                    LineLin = LineLin.Split('+')[3];
                    int Index = LineLin.IndexOf(":");
                    if (Index > 0)
                    {
                        LineLin = LineLin.Substring(0, Index);
                    }
                    Detail.ProductEAN = LineLin;

                    List<string> ListDetailElementLin = FullFile
                                          .SkipWhile(line => !line.Contains("LIN+" + CompleteZeros(4, (i + 1).ToString()))).Skip(1).TakeWhile(line => !line.Contains(delimiter)).ToList();

                    if (ListDetailElementLin.Where(c => c.Contains("IMD+F")).Count() > 0)
                    {
                        string DetailElementIMD = ListDetailElementLin.FirstOrDefault(c => c.Contains("IMD+F"));
                        string[] ListDetailElementIMD = DetailElementIMD.Split('+');
                        DetailElementIMD = string.Empty;
                        if (ListDetailElementIMD.Length > 2)
                        {
                            ListDetailElementIMD = ListDetailElementIMD[3].Split(':');
                            DetailElementIMD = ListDetailElementIMD[ListDetailElementIMD.Length - 1];
                        }
                        Detail.Description = DetailElementIMD.Replace("'", "");
                    }
                    if (ListDetailElementLin.Where(c => c.Contains("QTY+46")).Count() > 0)
                    {
                        Detail.DeliveredQuantity = ListDetailElementLin.FirstOrDefault(c => c.Contains("QTY+46")).Split(':')[1];
                    }
                    if (ListDetailElementLin.Where(c => c.Contains("QTY+194")).Count() > 0)
                    {
                        Detail.AmountReceivedAccepted = ListDetailElementLin.FirstOrDefault(c => c.Contains("QTY+194")).Split(':')[1];
                        Detail.AmountReceivedAccepted = Detail.AmountReceivedAccepted.Replace(".", ",");
                        Detail.AmountReceivedAccepted = Convert.ToDouble(Detail.AmountReceivedAccepted).ToString();
                    }
                    if (!string.IsNullOrEmpty(Detail.ProductEAN) && !string.IsNullOrEmpty(Detail.Description) && !string.IsNullOrEmpty(Detail.AmountReceivedAccepted))
                        ListDetail.Add(Detail);
                }

                ObjectHeader.Details = ListDetail;
                return ObjectHeader;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Se presento error durante la lectura del archivo: " + ex.Message);
                return new HeaderElement();
            }
        }
        /// <summary>
        /// Metodo para realizar conversion de fechas contenidas en los archivos edi
        /// </summary>
        /// <param name="Date">fecha a la que se desea hacer la conversion</param>
        /// <returns>datetime</returns>
        public DateTime ConvertDateTimeEdi(string Date)
        {
            DateTime dt = new DateTime();
            try
            {
                dt = Convert.ToDateTime(Date);
            }
            catch
            {
                try
                {
                    dt = DateTime.ParseExact(Date, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                    try
                    {
                        dt = DateTime.ParseExact(Date, "ddMMyyyy", System.Globalization.CultureInfo.CurrentUICulture);
                    }
                    catch (Exception)
                    {
                        dt = DateTime.ParseExact(Date, "yyyyMMddhhmm", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }

            return dt;
        }

        #endregion Methods
    }

    public class DetailElement
    {
        public string ProductEAN { get; set; }
        public string AmountRequested { get; set; }
        public string PointSale { get; set; }
        public string QuantityPerPointOfSale { get; set; }
        public string Description { get; set; }
        public string DeliveredQuantity { get; set; }
        public string AmountReceivedAccepted { get; set; }
        public string Line { get; set; }
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
        public string ReceiptWarningNumber { get; set; }
        public List<DetailElement> Details { get; set; }
    }
}