using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;

namespace DigitizingDataBizLayer.Data
{
    public class DBConfigParameters
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string EncryptionKey { get; set; }
        public bool ParamsLoadedStatus { get; set; }
    }

    public class OracleDatabaseHelper
    {
        private const int MAX_DB_CONNECT_ATTEMPTS = 5;
        private static string _Path = string.Empty;
        private static string _ApplicationPath = string.Empty;
        private static Oracle.DataAccess.Client.OracleConnection _OdacConn = null;
        private static string _OdacConnectionString = string.Empty;
        private static DBConfigParameters _DbConfigParams = null;
        //Random Number Generator
        //OMM: Not very sure whether to use the CryptoRNG or the Random
        private static Random _MComRNG = null;

        #region Static Properties
        //A Property to return the DbConfigParameters
        public static DBConfigParameters DbConfigParams
        {
            get
            {
                if (_DbConfigParams.ParamsLoadedStatus == true)
                {
                    return _DbConfigParams;
                }
                else
                {
                    return GetDbConfigParameters(true);
                }
            }
        }

        public static string OdacConnectionString
        {
            get
            {
                try
                {
                    _OdacConnectionString = string.Format("Data Source={0}; User Id={1}; Password={2}", DbConfigParams.Database, DbConfigParams.Username, DbConfigParams.Password);
                    return _OdacConnectionString;
                }
                catch (Exception ex)
                {
                    LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                    return string.Empty;
                }
            }
        }

        //A Static Property to return the Connection object
        public static Oracle.DataAccess.Client.OracleConnection DbConnection
        {
            get
            {
                try
                {

                    if (_OdacConn == null || _OdacConn.State != System.Data.ConnectionState.Open)
                    {
                        OpenDbConnection();
                    }

                    //Return the Connection
                    return _OdacConn;
                }
                catch (Exception ex)
                {
                    LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                    return new Oracle.DataAccess.Client.OracleConnection();
                }
            }
        }

        #endregion

        static OracleDatabaseHelper()
        {
            try
            {
                _OdacConn = new Oracle.DataAccess.Client.OracleConnection();
                _DbConfigParams = new DBConfigParameters();

                //Set the ApplicationPath
                _ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;

                //Instantiate the Random Number Generator class
                _MComRNG = new Random();
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
            }
        }

        public static Oracle.DataAccess.Client.OracleConnection CreateDbConnection()
        {
            try
            {

                if (_OdacConn == null || _OdacConn.State != System.Data.ConnectionState.Open)
                {
                    OpenDbConnection();
                }

                //Return the Connection
                return _OdacConn;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return new Oracle.DataAccess.Client.OracleConnection();
            }
        }

        //Opens the Connection
        private static void OpenDbConnection()
        {
            try
            {
                if (_OdacConn == null)
                {
                    _OdacConn = new Oracle.DataAccess.Client.OracleConnection();
                }

                //Just Force the ConnectionString
                _OdacConn.ConnectionString = OdacConnectionString;

                if (_OdacConn.State != System.Data.ConnectionState.Closed)
                {
                    _OdacConn.Close();
                }

                int connAttempts = 0;
                do
                {
                    connAttempts++;
                    _OdacConn.Open();
                }
                while (_OdacConn.State != System.Data.ConnectionState.Open && connAttempts < MAX_DB_CONNECT_ATTEMPTS);
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
            }
        }

        //This will use the Default Setting, if data is already there, don't read again
        public static DBConfigParameters GetDbConfigParameters()
        {
            return GetDbConfigParameters(false);
        }

        //Always force refreshing of the DbParameters
        public static DBConfigParameters GetDbConfigParameters(bool forceRefresh)
        {
            try
            {
                //If no Refreshing is enforced and Parameters are already loaded, just return
                if (forceRefresh == false && _DbConfigParams.ParamsLoadedStatus == true)
                {
                    return _DbConfigParams;
                }
                //Otherwise, return a refreshed configuration param values set
                _Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ORACLE_DB_INFO.INI");
                return DecryptFile(_Path);
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                _DbConfigParams.ParamsLoadedStatus = false;
                return _DbConfigParams;
            }
        }

        public static Oracle.DataAccess.Client.OracleDataReader ExecuteDataReader(string query)
        {
            Oracle.DataAccess.Client.OracleCommand cmd = null;
            Oracle.DataAccess.Client.OracleDataReader reader = null;

            try
            {
                cmd = new OracleCommand(query, DbConnection);
                cmd.CommandType = System.Data.CommandType.Text;
                reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return null;
            }
        }

        public static Oracle.DataAccess.Client.OracleDataReader ExecuteDataReader(string sprocName, OracleParameter[] sprocParams)
        {
            Oracle.DataAccess.Client.OracleCommand cmd = null;
            Oracle.DataAccess.Client.OracleDataReader reader = null;

            try
            {
                cmd = new OracleCommand(sprocName, DbConnection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (sprocParams != null && sprocParams.Length > 0)
                {
                    cmd.Parameters.AddRange(sprocParams);
                }
                reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return null;
            }
        }

        public static int ExecuteNonQuery(string query)
        {
            Oracle.DataAccess.Client.OracleCommand cmd = null;

            try
            {
                cmd = new OracleCommand(query, DbConnection);
                cmd.CommandType = System.Data.CommandType.Text;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return -1;
            }
        }

        public static int ExecuteNonQuery(string sprocName, OracleParameter[] sprocParams)
        {
            Oracle.DataAccess.Client.OracleCommand cmd = null;

            try
            {
                cmd = new OracleCommand(sprocName, DbConnection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (sprocParams != null && sprocParams.Length > 0)
                {
                    cmd.Parameters.AddRange(sprocParams);
                }

                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return -1;
            }
        }

        //To Return the Command Object in order to Access Output Parameters
        public static Oracle.DataAccess.Client.OracleCommand ExecuteNonQuery(string sprocName, OracleParameter[] sprocParams, bool returnCommandObj)
        {
            Oracle.DataAccess.Client.OracleCommand cmd = null;

            try
            {
                cmd = new OracleCommand(sprocName, DbConnection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (sprocParams != null && sprocParams.Length > 0)
                {
                    cmd.Parameters.AddRange(sprocParams);
                }

                int retVal = cmd.ExecuteNonQuery();
                return cmd;
            }
            catch (Exception ex)
            {
                LogToFileServer("server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return new OracleCommand(sprocName, DbConnection);
            }
        }

        private static DBConfigParameters DecryptFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MCASH_APP_MANAGER.INI");
            }
            FileStream fs = null;
            StreamReader srINI = null;
            string strContents = string.Empty;

            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                srINI = new StreamReader(fs);
                //To know the Line being read
                int lineNo = 0;
                while (srINI.Peek() != -1)
                {
                    strContents = Crypt(srINI.ReadLine());
                    if (lineNo == 0)
                        _DbConfigParams.Username = strContents;
                    else if (lineNo == 1)
                        _DbConfigParams.Password = strContents;
                    else if (lineNo == 2)
                        _DbConfigParams.Database = strContents;
                    else if (lineNo == 3)
                        _DbConfigParams.EncryptionKey = strContents;

                    //Increment the Line Number
                    lineNo++;
                }
                //mark that the DB Config Params have been set
                _DbConfigParams.ParamsLoadedStatus = true;
                return _DbConfigParams;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                _DbConfigParams.ParamsLoadedStatus = false;
                return _DbConfigParams;
            }
            finally
            {
                if (srINI != null)
                    srINI.Close();
            }
        }

        private static string Crypt(string _Text)
        {
            string strTempChar = string.Empty;
            char x;
            string strPlainText = string.Empty;
            try
            {
                for (int i = 0; i <= _Text.Length - 1; i++)
                {
                    x = _Text[i];
                    if ((int)x < 128)
                    {
                        strTempChar = ((char)(((int)x) + 128)).ToString();
                    }
                    else if ((int)x > 128)
                    {
                        strTempChar = ((char)(((int)x) - 128)).ToString();
                    }
                    strPlainText += strTempChar;
                }
                return strPlainText;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                return string.Empty;
            }
        }

        //Replace password PIN with ****
        public static string MaskPassword(string plainTextPassword)
        {
            try
            {
                string maskedPwd = "*".PadLeft(plainTextPassword.Length, '*');
                return maskedPwd;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                return string.Empty;
            }
        }


        //Generate a Random Number, using default range of 6 digits
        public static int GenerateRandomNumber()
        {
            try
            {
                if (_MComRNG == null)
                {
                    _MComRNG = new Random();
                }

                return _MComRNG.Next(100000, 999999);
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                return 0;
            }
        }

        //Generate a Random Number, using range supplied by caller
        public static int GenerateRandomNumber(int lowRange, int highRange)
        {
            try
            {
                if (_MComRNG == null)
                {
                    _MComRNG = new Random();
                }

                return _MComRNG.Next(lowRange, highRange);
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                return 0;
            }
        }

        public static void LogToFileServer(string targetFolder, string messageToLog)
        {
            string fileName;
            System.IO.FileStream fs;
            string startupPath;
            string logPath;

            if (System.IO.Directory.Exists(_ApplicationPath))
            {
                startupPath = _ApplicationPath;
            }
            else
            {
                startupPath = AppDomain.CurrentDomain.BaseDirectory;
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
                sWriter.WriteLine("[" + DateTime.Now.ToString("HHmmss") + "]" + "~" + messageToLog);
                sWriter.Close();
                fs.Close();
            }
            catch
            {
            }
        }

    }
}