using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace DigitizingDataBizLayer.Data
{
    public class MssqlDatabaseHelper
    {
        private static string _ConnectionString = string.Empty;
        private static SqlConnection _MssqlConn = null;
        private const int MAX_DB_CONNECT_ATTEMPTS = 5;

        public static string MssqlConnectionString
        {
            get
            {
                try
                {
                    _ConnectionString = string.Format("Server={0}; Database={1}; User Id={2}; Password={3}", "localhost", "BARCLAYS_DD", "vslauser", "vslauser");
                    return _ConnectionString;
                }
                catch (Exception ex)
                {
                    LogToFileServer("Server", "ERR:" + ex.Message + " TRACE: " + ex.StackTrace);
                    return string.Empty;
                }
            }
        }

        //A Static Property to return the Connection object
        public static System.Data.SqlClient.SqlConnection DbConnection
        {
            get
            {
                try
                {

                    if (_MssqlConn == null || _MssqlConn.State != System.Data.ConnectionState.Open)
                    {
                        OpenDbConnection();
                    }

                    //Return the Connection
                    return _MssqlConn;
                }
                catch (Exception ex)
                {
                    LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                    return new SqlConnection();
                }
            }
        }

        public static SqlConnection CreateDbConnection()
        {
            try
            {

                if (_MssqlConn == null || _MssqlConn.State != System.Data.ConnectionState.Open)
                {
                    OpenDbConnection();
                }

                //Return the Connection
                return _MssqlConn;
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return new SqlConnection();
            }
        }

        //Opens the Connection
        private static void OpenDbConnection()
        {
            try
            {
                if (_MssqlConn == null)
                {
                    _MssqlConn = new SqlConnection();
                }

                //Just Force the ConnectionString
                _MssqlConn.ConnectionString = _ConnectionString;

                if (_MssqlConn.State != System.Data.ConnectionState.Closed)
                {
                    _MssqlConn.Close();
                }

                int connAttempts = 0;
                do
                {
                    connAttempts++;
                    _MssqlConn.Open();
                }
                while (_MssqlConn.State != System.Data.ConnectionState.Open && connAttempts < MAX_DB_CONNECT_ATTEMPTS);
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
            }
        }

        public static SqlDataReader ExecuteDataReader(string query)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                cmd = new SqlCommand(query, DbConnection);
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

        public static SqlDataReader ExecuteDataReader(string sprocName, SqlParameter[] sprocParams)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                cmd = new SqlCommand(sprocName, DbConnection);
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
            SqlCommand cmd = null;

            try
            {
                cmd = new SqlCommand(query, DbConnection);
                cmd.CommandType = System.Data.CommandType.Text;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                LogToFileServer("Server", "ERR: " + ex.Message + " TRACE: " + ex.StackTrace);
                return -1;
            }
        }

        public static int ExecuteNonQuery(string sprocName, SqlParameter[] sprocParams)
        {
            SqlCommand cmd = null;

            try
            {
                cmd = new SqlCommand(sprocName, DbConnection);
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
        public static SqlCommand ExecuteNonQuery(string sprocName, SqlParameter[] sprocParams, bool returnCommandObj)
        {
            SqlCommand cmd = null;

            try
            {
                cmd = new SqlCommand(sprocName, DbConnection);
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
                return new SqlCommand(sprocName, DbConnection);
            }
        }


        public static void LogToFileServer(string targetFolder, string messageToLog)
        {
            string fileName;
            System.IO.FileStream fs;
            string startupPath;
            string logPath;

            startupPath = AppDomain.CurrentDomain.BaseDirectory;

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