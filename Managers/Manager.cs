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
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Carvajal.Turns.CodeResponses;


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
        public string AuthenticArchiveMD5(string MerchantId, string Route)
        {
            if (File.Exists(Route))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(Route))
                    {
                        string HashArchivo = Convert.ToBase64String(md5.ComputeHash(stream));
                        LogManager.WriteLog(MerchantId, "0", String.Format(Responses.A3, Path.GetFileName(Route), HashArchivo));
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




        public bool ValidCSVFileStructure(string PathRoute, string MerchantIdentifier, out int WrongRecords, out int TotalRecords)
        {
                
            String Filename;
            WrongRecords = 0; TotalRecords = 0;
            using (TextFieldParser parser = new TextFieldParser(PathRoute))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.TrimWhiteSpace = true;
                    parser.ReadLine(); //Omite la primera linea
                    while (!parser.EndOfData)
                    {
                        string[] fieldRow = parser.ReadFields();
                   // Aplica para Usuarios de Comercio: Administradores y Operadores de CE
                    Filename = "Usuario_CO_" + MerchantIdentifier+"_";
                    if (PathRoute.Contains(Filename))
                    {
                        TotalRecords++;
                        // Puede tener 5 campos (minimos), 6 campos (Opcionales) u 8 (Maximo).
                        if (!((fieldRow.Length ==5) || (fieldRow.Length == 6) || (fieldRow.Length == 8)))
                        {
                            if (fieldRow.Length == 7)
                            {
                                LogManager.WriteWarn(MerchantIdentifier, "0", Responses.M48 + "Responsable Centro" + Responses.M48_1);
                            }
                            else {
                                LogManager.WriteWarn(MerchantIdentifier, "0", Responses.M65 + Path.GetFileName(PathRoute) + Responses.M65_1 + fieldRow.Length + "  debia ser 5, 6 o 8.");
                                WrongRecords++;
                            }
                        }
                    }
                    Filename = "Usuario_FA_" + MerchantIdentifier + "_";
                    if (PathRoute.Contains("Usuario_FA_"))
                        {
                            TotalRecords++;
                        // Minimo pueden ser 10 campos, maximo 11,
                        if (!((fieldRow.Length == 10) || (fieldRow.Length == 11)))
                        {
                            WrongRecords++;
                            LogManager.WriteWarn(MerchantIdentifier, "0", Responses.M65 + Path.GetFileName(PathRoute) + Responses.M65_1 + fieldRow.Length + "  debia ser 10 o 11.");
                        }
                        }
                    Filename = "Centro_" + MerchantIdentifier + "_";
                    if (PathRoute.Contains(Filename))
                    {
                        TotalRecords++;
                        // Minimo pueden ser 11 campos, maximo 16
                        if ((fieldRow.Length < 11) || (fieldRow.Length > 16))
                        {
                            WrongRecords++;
                            LogManager.WriteWarn(MerchantIdentifier, "0", Responses.M65 + Path.GetFileName(PathRoute) + Responses.M65_1 + fieldRow.Length + "  debia ser 11 a 16.");
                        }
                    }
                    Filename = "Tiempo_" + MerchantIdentifier + "_";
                    if (PathRoute.Contains("Tiempo_"))
                        {
                            TotalRecords++;
                        // Tiene que ser 8 campos. Obligatorio.
                        if (fieldRow.Length != 8)
                        {
                            WrongRecords++;
                            LogManager.WriteWarn(MerchantIdentifier, "0", Responses.M65 + Path.GetFileName(PathRoute) + Responses.M65_1 + fieldRow.Length + "  debia ser 8.");
                        }
                        }

                }
                }
            return  ((TotalRecords>0) && (TotalRecords > WrongRecords))? true: false;
        
        }
        /// <summary>
        /// Metodo que valida la estructura del archivo  edi
        /// </summary>
        /// <param name="Path">ruta  y nombre  del archivo con su respectiva extension </param>
        /// <param name="MerchantIdentifier">Identificador de Comercio</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool ValidEDIFileStructure(string Path, string MerchantIdentifier)
        {
            try
            {
                bool BitValidFileStructure = true;

                IEnumerable<string> FullFile = File.ReadLines(Path);
                int LinesBGM = FullFile.Where(line => line.Contains("BGM+")).Count();
                if (LinesBGM > 1 || LinesBGM < 1)
                {
                    LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "BGM" + Responses.M47_1);
                    BitValidFileStructure = false;
                }

                if (BitValidFileStructure)
                {
                    int LinesNAD = FullFile.Where(line => line.Contains("NAD+")).Count();
                    if (LinesNAD < 3)
                    {
                        LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "NAD" + Responses.M47_1);
                        BitValidFileStructure = false;
                    }

                    if (BitValidFileStructure)
                    {
                        int LinesDTM = FullFile.Where(line => line.Contains("DTM+")).Count();
                        if (LinesDTM < 1)
                        {
                            LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "DTM" + Responses.M47_1);
                            BitValidFileStructure = false;
                        }

                        if (BitValidFileStructure)
                        {
                            int LinesLIN = FullFile.Where(line => line.Contains("LIN+")).Count();
                            if (LinesLIN < 1)
                            {
                                LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "LIN" + Responses.M47_1);
                                BitValidFileStructure = false;
                            }

                            if (BitValidFileStructure)
                            {
                                int LinesQTY = FullFile.Where(line => line.Contains("QTY+")).Count();
                                if (LinesQTY < 1)
                                {
                                    LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "QTY" + Responses.M47_1);
                                    BitValidFileStructure = false;
                                }

                                if (BitValidFileStructure)
                                {
                                    int LinesUNZ = FullFile.Where(line => line.Contains("UNZ")).Count();
                                    if (LinesUNZ < 1 || LinesUNZ > 1)
                                    {
                                        LogManager.WriteError(MerchantIdentifier, "0", Responses.M47 + "UNZ" + Responses.M47_1);
                                        BitValidFileStructure = false;
                                    }
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
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataOrdersEDI(string MerchantIdentifier, string PathFile)
        {
            try
            {
                HeaderElement ObjectHeader = new HeaderElement();
                ObjectHeader = ReadOrderEdi(MerchantIdentifier, PathFile);

                if (ObjectHeader.Details.Count > 0 && !string.IsNullOrEmpty(ObjectHeader.PurchaseOrderNumber) && !string.IsNullOrEmpty(ObjectHeader.TypeOfPurchaseOrder) && !string.IsNullOrEmpty(ObjectHeader.DeadLine) && !string.IsNullOrEmpty(ObjectHeader.MerchandiseDeliverySite))
                {
                    try
                    {
                       
                        Users UserMerchant = CUsers.Instance.SearchUser(ObjectHeader.Commerce);
                        if (UserMerchant == null)
                        {
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.M69+ObjectHeader.PurchaseOrderNumber+Responses.M67_1+ ObjectHeader.Commerce+Responses.M67_2);
                            return false;
                        }
                        Users UserProvider = CUsers.Instance.SearchUser(ObjectHeader.Provider);
                        if (UserProvider == null)
                        {
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.M69 + ObjectHeader.PurchaseOrderNumber + Responses.M58_1 + ObjectHeader.Provider + Responses.M58_2);
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
                                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.A8 + JsonConvert.SerializeObject(Product));
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
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.A12 + JsonConvert.SerializeObject(ObjectOrders));
                            return false;
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.A12 +"."+ex.Message);
                        return false;
                    }
                }
                else
                {
                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.PurchaseOrderNumber, Responses.A10);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A11 + Path.GetFileName(PathFile) + "." + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Metodo que guarda la informacion del archivo Recibo de pedido Edi 
        /// </summary>
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataRecAdvEDI(string MerchantIdentifier, string PathFile)
        {
            try
            {
                HeaderElement ObjectHeader = new HeaderElement();

                ObjectHeader = ReadRecAdvEdi(MerchantIdentifier, PathFile);

                Orders AssignedOrder = COrders.Instance.SearchOrder(ObjectHeader.PurchaseOrderNumber);
                if (AssignedOrder == null)
                {
                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.M66  + ObjectHeader.PurchaseOrderNumber + Responses.M66_1 + ObjectHeader.ReceiptWarningNumber);
                    return false;
                }

                Turns Turn = CTurns.Instance.SearchTurnsForOrderActive(ObjectHeader.PurchaseOrderNumber);
                if (Turn == null)
                {
                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.M62+ ObjectHeader.PurchaseOrderNumber + Responses.M62_1);
                    return false;
                }

                if (ObjectHeader.Details.Count > 0 && !string.IsNullOrEmpty(ObjectHeader.PurchaseOrderNumber) && !string.IsNullOrEmpty(ObjectHeader.ReceiptWarningNumber) && !string.IsNullOrEmpty(ObjectHeader.DeadLine) && !string.IsNullOrEmpty(ObjectHeader.MerchandiseDeliverySite))
                {
                    try
                    {
                        Users UserProvider = CUsers.Instance.SearchUser(ObjectHeader.Provider);
                        if (UserProvider == null)
                        {
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.M58 + ObjectHeader.PurchaseOrderNumber + Responses.M58_1 + ObjectHeader.Provider + Responses.M58_2);
                            return false;
                        }

                        Users UserMerchant = CUsers.Instance.SearchUser(ObjectHeader.Commerce);
                        if (UserMerchant == null)
                        {
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.M67 + ObjectHeader.ReceiptWarningNumber + Responses.M67_1 + ObjectHeader.Commerce + Responses.M67_2);
                            return false;
                        }
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
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.M68 + ObjectHeader.ReceiptWarningNumber + Responses.M68_1 + ObjectHeader.MerchandiseDeliverySite + Responses.M68_2);
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
                                //TODO Preguntar a Jeison Vargas porque se borran todos los productos o definirlo en los documentos.
                                if (!CAdvicesProducts.Instance.SaveAdvicesProduct(ObjectAdvicesProduct))
                                {
                                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.A7 + JsonConvert.SerializeObject(ObjectAdvicesProduct));
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
                            LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.A9 +JsonConvert.SerializeObject(ObjectAdvicesProducts));
                            return false;
                        }

                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.A9 +"."+ ex.Message);
                        return false;
                    }
                }
                else
                {
                    LogManager.WriteLog(MerchantIdentifier, ObjectHeader.ReceiptWarningNumber, Responses.A10);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A11+ Path.GetFileName(PathFile)+"."+ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Metodo que guarda la informacion del archivo usuarios de Comercio
        /// </summary>
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataMerchantUsersCSV(string MerchantIdentifier, string PathFile)   
        {
            try
            {
                int WrongRecords, TotalRecords, CounterUsers=0;
                List<Users> MerchantUser = new List<Users>();
                 MerchantUser = ReadMerchantUser(MerchantIdentifier, PathFile, out WrongRecords, out TotalRecords);
                try
                {
                    foreach (Users newUser in MerchantUser)
                    {
                        
                        if (!CUsers.Instance.SaveUser(newUser))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.A18 + newUser.PkIdentifier);
                            return false;
                        }
                        CounterUsers++;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(MerchantIdentifier, "0", Responses.A19 + ex.Message);
                    return false;
                }
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M51 + CounterUsers + Responses.M51_1);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A17 + Path.GetFileName(PathFile) + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Metodo que guarda la informacion del archivo Fabricante de Comercio
        /// </summary>
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataManufacturerUsersCSV(string MerchantIdentifier, string PathFile)
        {
            try
            {
                int WrongRecords, TotalRecords, CounterUsers=0;
                List<Companies> ManufacturerUser = new List<Companies>();
                ManufacturerUser = ReadManufacturerUser(MerchantIdentifier, PathFile, out WrongRecords, out TotalRecords);
                try
                {
                    Companies newManufacturerUser = new Companies();
                    foreach (Companies newUser in ManufacturerUser)
                    {    
                      if (!CCompanies.Instance.SaveCompanies(newUser))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.A15 + newUser.PkIdentifier);
                            return false;
                        }
                        CounterUsers++;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(MerchantIdentifier, "0", Responses.A16 + ex.Message);
                    return false;
                }
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M51 + CounterUsers + Responses.M51_1);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A14 + Path.GetFileName(PathFile)+"."+ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Metodo que guarda la informacion de los Centros de Entrega
        /// </summary>
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataCentreCSV(string MerchantIdentifier, string PathFile)
        {
            try
            {
                int WrongRecords, TotalRecords, CounterCentre=0;
                List<Centres> CentreList = new List<Centres>();
                CentreList = ReadCentre(MerchantIdentifier, PathFile, out WrongRecords,out TotalRecords);
                try
                {
                    Centres newCentreToAdd = new Centres();
                    foreach (Centres newCentre in CentreList)
                    {
                        if (!CCentres.Instance.SaveCentres(newCentre))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.A22 + newCentreToAdd.PkIdentifier);
                            return false;
                        }
                        CounterCentre++;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(MerchantIdentifier, "0", Responses.A23 + ex.Message);
                    return false;
                }
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M51 + CounterCentre + Responses.M51_1);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A21 + Path.GetFileName(PathFile) + "." + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Metodo que guarda la informacion de los Tiempos de Descarga
        /// </summary>
        /// <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        public bool SaveDataUnloadingTimeCSV(string MerchantIdentifier, string PathFile)
        {
            try
            {
                int WrongRecords = 0, TotalRecords = 0, CounterTime=0;
                List<UnloadingTime> UnloadingTimeList = new List<UnloadingTime>();
                UnloadingTimeList = ReadUnloadingTime(MerchantIdentifier, PathFile, out WrongRecords, out TotalRecords);
                try
                {
                    foreach (UnloadingTime NewUnloadingTime in UnloadingTimeList)
                    {
                        if (!CUnloadingTime.Instance.SaveUnloadingTime(NewUnloadingTime))
                        {
                            LogManager.WriteLog(MerchantIdentifier, NewUnloadingTime.FkCentres_Identifier, Responses.A26+ NewUnloadingTime.FkCentres_Identifier);
                            return false;
                        }
                        CounterTime++;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteLog(MerchantIdentifier, "0",
                                Responses.A27 + ex.Message);
                    return false;
                }
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M51 + CounterTime + Responses.M51_1);
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0",Responses.A25 + Path.GetFileName(PathFile) + "." + ex.Message);
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
        /// Limita la cantidad de caracteres de un texto 
        /// </summary>
        /// <param name="Length">Extencion maxima de la cadena</param>
        /// <param name="Line">cararcteres que van a la izquierda de los ceros</param>
        /// <returns>retorna true si el proceso fue exitoso en caso contrario false</returns>
        string NormalizeLength(string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        /// <summary>
        /// Metodo que lee y extrae la informacion del archivo edi de una orden de pedido
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public HeaderElement ReadOrderEdi(string MerchantIdentifier, string PathFile)
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
                
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A6 + PathFile + "." + ex.Message);
                return new HeaderElement();
            }
        }
        /// <summary>
        /// Metodo que lee y extrae la informacion del archivo edi de una Recibo de pedido
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio </returns>
        public HeaderElement ReadRecAdvEdi(string MerchantIdentifier, string PathFile)
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
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A6 + PathFile +"."+ ex.Message);
                return new HeaderElement();
            }
        }
        /// <summary>
        /// Metodo para revisar si el correo electronico es correcto
        /// </summary>
        /// <param name="email">Correo al que se desea realizar la revision</param>
        /// <returns>bool</returns>
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
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
        /// <summary>
        /// Metodo que lee el archivo de usuarios del Comercio para crear multiples usuarios
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public List<Users> ReadMerchantUser(string MerchantIdentifier, string PathFile, out int WrongRegisters, out int TotalRegisters)
        {

            var MerchantUsersToReturn = new List<Users>();
            WrongRegisters = 0;
            TotalRegisters = 0;
            try
            {

                Companies MerchantUser = new Companies();
                Users CheckUser = new Users();
                using (TextFieldParser parser = new TextFieldParser(PathFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadLine(); //Omite la primera linea
                    
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        Users NewUser = new Users();
                        TotalRegisters++;
                        NewUser.PkIdentifier = NormalizeLength(fields[0], 35);
                        NewUser.FkCompanies_Identifier = NormalizeLength(fields[1], 35);
                        NewUser.ChangePasswordNextTime = true;
                        NewUser.Name = NormalizeLength(fields[2], 175);
                        //TODO Busca la tabla adecuada
                        NewUser.FkRole_Identifier = NormalizeLength(fields[3], 2);
                        NewUser.Email = NormalizeLength(fields[4], 512);
                        if (!IsValidEmail(NewUser.Email))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M70 + NewUser.Email);
                            WrongRegisters++;
                            continue;
                        }
                        NewUser.Status = true;
                        // Busca usuarios con el mismo ID en Usuarios o Companias
                        CheckUser = CUsers.Instance.SearchUser(NewUser.PkIdentifier);
                        if (!Object.ReferenceEquals(null, CheckUser))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M73 + NewUser.PkIdentifier);
                            if (!CUsers.Instance.DeleteUser(CheckUser))
                            {
                                LogManager.WriteWarn(MerchantIdentifier, "0", Responses.A30 + NewUser.PkIdentifier);                            
                                WrongRegisters++;
                                continue;
                            }
                                
                            continue;
                        }
                        MerchantUser = CCompanies.Instance.SearchCompaniesForId(NewUser.PkIdentifier);
                        if (!Object.ReferenceEquals(null, MerchantUser))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M74 + NewUser.PkIdentifier);
                            WrongRegisters++;
                            continue;

                        }
                        // Busca el comercio asociado
                        MerchantUser = CCompanies.Instance.SearchCompaniesForId(NewUser.FkCompanies_Identifier);
                        if (Object.ReferenceEquals(null, MerchantUser)) {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M71 + NewUser.FkCompanies_Identifier);
                            WrongRegisters++;
                            continue;
                        }

                        NewUser.FkCompanies_Identifier = MerchantUser.PkIdentifier;
                        NewUser.FkCountries_Identifier = MerchantUser.FkCountries_Identifier;
                        
                        //Opcional el Telefono
                        if (fields.Count() >= 6)
                        {
                            if (fields[5].Length > 0)
                                NewUser.Phone = NormalizeLength(fields[5], 15);
                        } 
                        //TODO Hacer la vinculacion a los centros de Entrega que no se han realizado             
                        MerchantUsersToReturn.Add(NewUser);
                    }
                }

                return MerchantUsersToReturn;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A17 +Path.GetFileName(PathFile) + "."+ex.Message);
                return new List<Users>();
            }
        }
        /// <summary>
        /// Metodo que lee el archivo de usuarios del Fabricante para crear multiples usuarios Fabricantes al mismo comercio
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public List<Companies> ReadManufacturerUser(string MerchantIdentifier, string PathFile, out int WrongRegisters, out int TotalRegisters)
        {

            var ManufacturersToReturn = new List<Companies>();
            WrongRegisters = 0;
            TotalRegisters = 0;
            try
            {

                Companies MerchantUser = new Companies();
                Users CheckUser = new Users();                
                using (TextFieldParser parser = new TextFieldParser(PathFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadLine(); //Omite la primera linea
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        Companies ManufacturerUser = new Companies();
                        TotalRegisters++;                        
                        ManufacturerUser.PkIdentifier = NormalizeLength(fields[0], 35);
                        ManufacturerUser.Companies_Identifier = NormalizeLength(fields[1], 35);
                        ManufacturerUser.ChangePasswordNextTime = true;
                        ManufacturerUser.Name = NormalizeLength(fields[2], 175);
                        //TODO Busca el role comercio
                        ManufacturerUser.FkRole_Identifier = NormalizeLength(fields[3], 2);
                        ManufacturerUser.Email = NormalizeLength(fields[4], 512);
                        if (!IsValidEmail(ManufacturerUser.Email))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M70 + ManufacturerUser.Email);
                            WrongRegisters++;
                            continue;
                        }
                        ManufacturerUser.Status = true;
                        // Busca el comercio asociado
                        MerchantUser = CCompanies.Instance.SearchCompaniesForId(ManufacturerUser.Companies_Identifier);
                        if (Object.ReferenceEquals(null, MerchantUser) || (MerchantUser.FkRole_Identifier.Equals("FA")))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M71 + ManufacturerUser.Companies_Identifier);
                            WrongRegisters++;
                            continue;
                        }
                        ManufacturerUser.Companies_Identifier = MerchantUser.PkIdentifier;
                        ManufacturerUser.FkCountries_Identifier = MerchantUser.FkCountries_Identifier;
                        // Busca usuarios con el mismo ID en Usuarios Comerciante
                        CheckUser = CUsers.Instance.SearchUser(ManufacturerUser.PkIdentifier);
                        if (!Object.ReferenceEquals(null, CheckUser))
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M72 + ManufacturerUser.PkIdentifier);
                            WrongRegisters++;
                            continue;
                        }
                        //Modificar el fabricante si lo creo el mismo comercio que el actual
                        MerchantUser = CCompanies.Instance.SearchCompaniesForId(ManufacturerUser.PkIdentifier);
                        if (!Object.ReferenceEquals(null, MerchantUser))
                        {
                            if (MerchantUser.FkRole_Identifier.Equals("FA") && MerchantIdentifier.Equals(ManufacturerUser.Companies_Identifier))
                            {
                                if (!CCompanies.Instance.DeleteCompanies(MerchantUser))
                                {
                                    LogManager.WriteWarn(MerchantIdentifier, "0", Responses.A13 + ManufacturerUser.PkIdentifier);
                                    WrongRegisters++;
                                    continue;
                                }
                                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M52 + ManufacturerUser.PkIdentifier+Responses.M52_1);
                            }
                            if (!MerchantIdentifier.Equals(ManufacturerUser.Companies_Identifier))
                            {
                                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M56 + ManufacturerUser.PkIdentifier + Responses.M56_1);
                                WrongRegisters++;
                                continue;
                            }
                        }
                            
                        //End TODO
                        

                        ManufacturerUser.AddressStreet = NormalizeLength(fields[5], 70);
                        ManufacturerUser.AddressNumber = NormalizeLength(fields[6], 70);
                        ManufacturerUser.PostCode = NormalizeLength(fields[7], 9);
                        ManufacturerUser.Town = NormalizeLength(fields[8], 35);
                        ManufacturerUser.Region = NormalizeLength(fields[9], 9);

                        if (fields.Count() >= 11)
                        {
                            if (fields[10].Length > 0)
                                ManufacturerUser.Phone = NormalizeLength(fields[10], 15);
                        }
                        
                        ManufacturersToReturn.Add(ManufacturerUser);
                    }
                }

                return ManufacturersToReturn;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A14+ Path.GetFileName(PathFile) + "." + ex.Message);
                return new List<Companies>();
            }
        }
        /// <summary>
        /// Metodo que lee el archivo de centros de entrega para el mismo comercio
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public List<Centres> ReadCentre(string MerchantIdentifier, string PathFile, out int WrongRegisters, out int TotalRegisters)
        {

            var CentresToReturn = new List<Centres>();
            Companies MerchantUser = new Companies();
            Timezones TimezoneToCheck = new Timezones();
            WrongRegisters = 0;
            TotalRegisters = 0;
            try
            {
                using (TextFieldParser parser = new TextFieldParser(PathFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadLine(); //Omite la primera linea
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        Centres CentreToAdd = new Centres();
                        Centres CentreToCheck = new Centres();
                        CentreToAdd.PkIdentifier = NormalizeLength(fields[0], 35);
                        CentreToCheck = CCentres.Instance.SearchCentresForId(CentreToAdd.PkIdentifier);
                        if (CentreToCheck != null)
                        {
                            if (CentreToCheck.FkUsers_Merchant_Identifier.Equals(MerchantIdentifier))
                            {
                                if (!CCentres.Instance.DeleteCenter(CentreToCheck))
                                {
                                    LogManager.WriteWarn(MerchantIdentifier, "0", Responses.A31 + CentreToAdd.PkIdentifier);
                                    WrongRegisters++;
                                    continue;
                                }
                                LogManager.WriteLog(MerchantIdentifier, "0", Responses.M57 + CentreToAdd.PkIdentifier + Responses.M57_1);
                            }
                        }
                        CentreToAdd.FkUsers_Merchant_Identifier = NormalizeLength(fields[1], 35);
                        MerchantUser = CCompanies.Instance.SearchCompaniesForId(CentreToAdd.FkUsers_Merchant_Identifier);
                        if (MerchantUser==null)
                        {
                            LogManager.WriteLog(MerchantIdentifier, "0", Responses.M71 + CentreToAdd.FkUsers_Merchant_Identifier);
                            WrongRegisters++;
                            continue;
                        }
                        CentreToAdd.Name = NormalizeLength(fields[2], 175);
                        CentreToAdd.AddressStreet = NormalizeLength(fields[3], 70);
                        CentreToAdd.AddressNumber = NormalizeLength(fields[4], 70);
                        CentreToAdd.PostCode = NormalizeLength(fields[5], 9);
                        CentreToAdd.Town = NormalizeLength(fields[6], 35);
                        CentreToAdd.Region = NormalizeLength(fields[7], 9);
                        CentreToAdd.Status = true;
                        //Hacer la consulta para encontrar la zona horaria adecuada
                        TimezoneToCheck = CTimezones.Instance.SearchTimezonesForCode(NormalizeLength(fields[8], 4));
                        if (TimezoneToCheck == null)
                        {
                            LogManager.WriteWarn(MerchantIdentifier, CentreToAdd.PkIdentifier, Responses.A33 + NormalizeLength(fields[8], 4) + Responses.A33_1 + CentreToAdd.PkIdentifier);
                            WrongRegisters++;
                            continue;
                        }
                        CentreToAdd.FkTimezones_Identifier = TimezoneToCheck.PkIdentifier;
                        CentreToAdd.NumberOfDocks = Int16.Parse(NormalizeLength(fields[9], 5));
                        CentreToAdd.TimeBetweenSuppliers = Int16.Parse(NormalizeLength(fields[10], 4));
                        //Valores por Defecto
                        CentreToAdd.WeeklyCapacity = 200;
                        CentreToAdd.CurrentWeekCapacity = 0;
                        CentreToAdd.FirstDay = "D";
                        CentreToAdd.ListOfWorkingDays = "LMXJV";
                        DateTime HoursToEmulate = DateTime.Parse("08:00", System.Globalization.CultureInfo.CurrentCulture);
                        CentreToAdd.StartTime = HoursToEmulate;
                        HoursToEmulate = DateTime.Parse("17:00", System.Globalization.CultureInfo.CurrentCulture);
                        CentreToAdd.EndTime = HoursToEmulate;

                        if (fields.Count() > 11)
                        {    
                            if (fields[11].Length > 0)
                                CentreToAdd.WeeklyCapacity = Int32.Parse(NormalizeLength(fields[11], 4));
                        }
                        if (fields.Count() > 12)
                        {
                            if (fields[12].Length > 0)
                                CentreToAdd.FirstDay = NormalizeLength(fields[12], 1);
                        }
                        if (fields.Count() > 13)
                        {
                            if (fields[13].Length > 0)
                                CentreToAdd.ListOfWorkingDays = NormalizeLength(fields[13], 7);
                        }
                        
                        
                        if (fields.Count() > 14)
                        {
                            string HoursMinutes = NormalizeLength(fields[14], 4).Substring(0, 2) + ":" + NormalizeLength(fields[14], 4).Substring(2, 2);
                            HoursToEmulate = DateTime.Parse(HoursMinutes, System.Globalization.CultureInfo.CurrentCulture);
                            if (fields[14].Length > 0)
                                CentreToAdd.StartTime = HoursToEmulate;
                        }
                        if (fields.Count() > 15)
                        {
                            string HoursMinutes = NormalizeLength(fields[15], 4).Substring(0, 2) + ":" + NormalizeLength(fields[15], 4).Substring(2, 2);
                            HoursToEmulate = DateTime.Parse(HoursMinutes, System.Globalization.CultureInfo.CurrentCulture);
                            if (fields[15].Length > 0)
                                CentreToAdd.EndTime = HoursToEmulate;
                        }
                        CentresToReturn.Add(CentreToAdd);
                    }
                }

                return CentresToReturn;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A20 + Path.GetFileName(PathFile) + "." + ex.Message);
                return new List<Centres>();
            }
        }
        /// <summary>
        /// Metodo que lee el archivo de tiempos de descarga para los centros de entrega del comercio
        /// </summary>
        ///  <param name="MerchantIdentifier">Codigo del Comercio</param>
        /// <param name="PathFile">ruta y nombre  del archivo con su respectiva extension</param>
        /// <returns>retorna un objeto HeaderElement lleno si el proceso fue exitoso en caso contrario retorna un objeto vacio</returns>
        public List<UnloadingTime> ReadUnloadingTime(string MerchantIdentifier, string PathFile, out int WrongRegisters, out int TotalRegisters)
        {
            WrongRegisters = 0;
            TotalRegisters = 0;
            var UnloadingTimesToReturn = new List<UnloadingTime>();
            try
            {

                
                using (TextFieldParser parser = new TextFieldParser(PathFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.ReadLine(); //Omite la primera linea
                    while (!parser.EndOfData)
                    {
                        //Processing row
                        Companies CheckCompanies = new Companies();
                        Centres CheckCentres = new Centres();
                        UnloadingTime UnloadingTimeToAdd = new UnloadingTime();
                        string[] fields = parser.ReadFields();
                        UnloadingTimeToAdd.FkCentres_Identifier = NormalizeLength(fields[1], 35);
                        CheckCentres = CCentres.Instance.SearchCentresForId(UnloadingTimeToAdd.FkCentres_Identifier);                     
                        if (Object.ReferenceEquals(null, CheckCentres))
                        {
                            LogManager.WriteLog(MerchantIdentifier, UnloadingTimeToAdd.FkCentres_Identifier, Responses.M75+ UnloadingTimeToAdd.FkCentres_Identifier);
                            WrongRegisters++;
                            continue;
                        }
                        //Existe un comercio del centro de entrega y es igual al que viene en el archivo
                        CheckCompanies = CCompanies.Instance.SearchCompaniesForId(CheckCentres.FkUsers_Merchant_Identifier);
                        if (!CheckCompanies.PkIdentifier.Equals(NormalizeLength(fields[0], 35)))
                        {
                            LogManager.WriteLog(MerchantIdentifier, UnloadingTimeToAdd.FkCentres_Identifier, Responses.M75 + UnloadingTimeToAdd.FkCentres_Identifier+ Responses.M76+ NormalizeLength(fields[0], 35));
                            WrongRegisters++;
                            continue;
                        }

                        UnloadingTimeToAdd.FkUsers_Manufacturer_Identifier = NormalizeLength(fields[2], 35);
                        CheckCompanies = CCompanies.Instance.SearchCompaniesForId(UnloadingTimeToAdd.FkUsers_Manufacturer_Identifier);
                        if (Object.ReferenceEquals(null, CheckCompanies))
                        {
                            LogManager.WriteLog(MerchantIdentifier, UnloadingTimeToAdd.FkCentres_Identifier, Responses.M77 + UnloadingTimeToAdd.FkUsers_Manufacturer_Identifier);
                            WrongRegisters++;
                            continue;
                        }
                        UnloadingTimeToAdd.ProductCode = NormalizeLength(fields[3], 35);
                        UnloadingTimeToAdd.MeasureUnit = NormalizeLength(fields[4], 35);
                        UnloadingTimeToAdd.Dock= Convert.ToSByte(NormalizeLength(fields[5], 35));
                        UnloadingTimeToAdd.AmountPerPallet = Convert.ToDecimal( NormalizeLength(fields[6], 5));
                        UnloadingTimeToAdd.SpecificUnloadingTime = Convert.ToInt32(NormalizeLength(fields[7], 4));
                        UnloadingTimeToAdd.Status = true;
                        UnloadingTimeToAdd.LastChangeDate = DateTime.Now;
                        TotalRegisters++;
                        UnloadingTimesToReturn.Add(UnloadingTimeToAdd);


                        //TODO Cargar la descripcion de los productos AQUI...
                    }
                }

                return UnloadingTimesToReturn;
            }
            catch (Exception ex)
            {
               

                LogManager.WriteLog(MerchantIdentifier, "0", Responses.A24 + Path.GetFileName(PathFile) + "."+ex.Message);
                WrongRegisters = 0;
                TotalRegisters = 0;
                return new List<UnloadingTime>();
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