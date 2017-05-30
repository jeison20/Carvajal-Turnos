using Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.IO;

namespace UnitTest.Carvajal.Turns.Service
{
    [TestClass]
    public class UTestManagers
    {
        //[TestInitialize]
        //public void InitSettings()
        //{
        //    DbContextManager.AddDbContextInfo(ConnectionStringName: "Model", DbContextType: typeof(Model));
        //}
        private static Manager ObjManagers = new Manager();
        [TestMethod()]
        public void ConvertEdiToXMLTest()
        {
            //string PathFile = @"C:\Logs\Comercio1\LV'7798084010000'9930707451978'6004033191-1'ORDERS'6'1'5188405.edi";
            //XmlDocument Document = ObjManagers.ConvertEdiToXML(PathFile);
            //Assert.IsNotNull(Document);
            Assert.Fail();
        }

        [TestMethod()]
        public void AuthenticArchiveMD5Test()
        {
            //string PathFile = @"C:\Logs\Comercio1\LV'7798084010000'9930707451978'6004033191-1'ORDERS'6'1'5188405.edi";
            //string AuthenticFile = ObjManagers.AuthenticArchiveMD5(PathFile);
            //Assert.IsFalse(string.IsNullOrEmpty(AuthenticFile));
            Assert.Fail();
        }

        [TestMethod()]
        public void UploadFileTest()
        {
            //string PathFile = @"C:\Logs\Comercio1\LV'7798084010000'9930707451978'6004033191-1'ORDERS'6'1'5188405.edi";
            //Stream Archivo = File.OpenRead(PathFile);
            //Assert.IsTrue(ObjManagers.UploadFile(Path.GetFileName(PathFile), Archivo, "Comercio1", "Procesados"));
            Assert.Fail();
        }

        [TestMethod()]
        public void DownloadFileTest()
        {

            Assert.Fail();
        }

        [TestMethod()]
        public void DownloadFilesByFolderTest()
        {
            List<string> ListaArchivosNoDescargados = new List<string>();
            List<string> ListaArchivosDescargados = new List<string>();
            ListaArchivosDescargados = ObjManagers.DownloadFilesByFolder("Comercio1" + "/SinProcesar", @"C:\Logs\Comercio1", out ListaArchivosNoDescargados);
            Assert.IsTrue(ListaArchivosDescargados.Count > 0 && ListaArchivosNoDescargados.Count == 0);
        }

        [TestMethod()]
        public void CreateFTPFolderTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void DeleteFileTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidFileStructureTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveDataEDITest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void SaveDataRecAdvEDITest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CompleteZerosTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadOrderEdiTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ReadRecAdvEdiTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ConvertDateTimeEdiTest()
        {
            Assert.Fail();
        }
    }
}
