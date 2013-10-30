using System;
using System.Collections.Generic;
using System.IO;
using DigitizingDataDomain.Helpers;
using NHibernate;
using System.ComponentModel;

namespace DigitizingDataBizLayer.Helpers
{
    public class AppGlobals
    {
        public static string COMPANY_NAME = "Grameen Foundation";
        public static string APP_TITLE = "Ledger Link";
        public static string ERROR_INTRO = "Sorry! An error has occurred.";

        public static System.Threading.Thread MainThread = null;

        //The Path for the _ApplicationPath
        private static string _ApplicationPath = string.Empty;

        public const string LOG_FOLDER_SERVER = "Server";

        //Session Proxy
        private static ISession sessionProxy = null;
        public static ISession SessionProxy
        {
            get
            {
                if (sessionProxy == null || !sessionProxy.IsOpen)
                {
                    //Open the Session
                    sessionProxy = OpenSessionProxy();
                }
                return sessionProxy;
            }
        }

        private static ISession OpenSessionProxy()
        {
            return NHibernateHelper.OpenSession();
        }

        //Will store config Options
        private static Dictionary<string, string> _SystemConfigSettings;
        public static Dictionary<string, string> SystemConfigSettings
        {
            get
            {
                if (_SystemConfigSettings == null)
                {
                    _SystemConfigSettings = new Dictionary<string, string>();
                }
                return _SystemConfigSettings;
            }
            set
            {
                _SystemConfigSettings = value;
            }
        }

        //Global RowCount
        public static int GlobalRowCount;

        //The endpoint Address URL for the Service. Default is one hard-coded in startup form MDIParent1
        private static Uri _EndpointAddress;

        #region Public Static Methods
        public static string ExtractExceptionMessageFromXml(string XmlDoc)
        {
            try
            {
                string retVal = string.Empty;

                //Create an XMLDocument and Load the XML String
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(XmlDoc);
                System.Xml.XmlNodeList nodeList = doc.GetElementsByTagName("message");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    retVal = retVal + nodeList[i].InnerXml;
                }

                return retVal;
            }
            catch
            {
                //Just return the original string
                return XmlDoc;
            }

        }
        #endregion

        #region Public static Properties        

        //Endpoint Address used for connecting to the Service: Default is hard-coded in Startup Form
        public static Uri ProxyEndPointAddress
        {
            get { return _EndpointAddress; }
            set { _EndpointAddress = value; }
        }
        #endregion

        
       

        #region Logging Methods
        public static void LogToFileServer(string targetFolder, string messageToLog)
        {
            string fileName;
            FileStream fs;
            string startupPath;
            string logPath;

            if (System.IO.Directory.Exists(_ApplicationPath))
            {
                startupPath = _ApplicationPath;
            }
            else
            {
                startupPath = AppDomain.CurrentDomain.BaseDirectory;
                _ApplicationPath = startupPath;
            }

            //Check the Log Folder. If it doesnt exist create it
            if (!System.IO.Directory.Exists(startupPath + @"\Log"))
            {
                System.IO.Directory.CreateDirectory(startupPath + @"\Log");
            }

            //Get the Log Path
            if (!string.IsNullOrEmpty(targetFolder))
            {
                //Check whether the Specified Directory Exists
                if (!System.IO.Directory.Exists(startupPath + @"\Log\" + targetFolder))
                {
                    System.IO.Directory.CreateDirectory(startupPath + @"\Log\" + targetFolder);
                }
                //Add a Slash to the logPath so as to just add the file
                logPath = startupPath + @"\Log\" + targetFolder + @"\";
            }
            else
            {
                //Force Creation of the Server folder in case it doesn't exist and write there
                if (!System.IO.Directory.Exists(startupPath + @"\Log\Server"))
                {
                    System.IO.Directory.CreateDirectory(startupPath + @"\Log\Server");
                }
                //Now specify the Log\Server as the default path
                logPath = startupPath + @"\Log\Server\";
            }

            fileName = logPath + System.DateTime.Now.ToString("ddMMMyyyy@HH00") + ".log";
            try
            {
                if (File.Exists(fileName))
                {
                    fs = new FileStream(fileName, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                }
                StreamWriter sWriter = new StreamWriter(fs);
                sWriter.BaseStream.Seek(0, SeekOrigin.End);
                sWriter.WriteLine("[" + DateTime.Now.ToString("HHmmss.ttt") + "] ~" + messageToLog);
                sWriter.Close();
                fs.Close();
            }
            catch
            {
            }
        }

        #endregion
    }
}
