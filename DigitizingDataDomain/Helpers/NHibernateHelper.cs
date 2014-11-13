using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.ComponentModel;
using System.Collections;
using System.IO;
using NHibernate.Dialect;

namespace DigitizingDataDomain.Helpers
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        //The Path for the _ApplicationPath
        private static string _ApplicationPath = string.Empty;
        public const string LOG_FOLDER_SERVER = "Server";

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
            set { _sessionFactory = value;  }
        }
        private static void InitializeSessionFactory()
        {
            try
            {
                FluentConfiguration cfg = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008
                                  .ConnectionString(c=>c.FromConnectionStringWithKey("MSSQL2012"))
                                    .Dialect<MsSql2012Dialect>()
                    );
                
                cfg.Mappings(m => m
                    .FluentMappings
                        .AddFromAssemblyOf<NHibernateHelper>()
                    );

                _sessionFactory =  cfg.BuildSessionFactory();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += (Environment.NewLine + " Inner Exception->" + ex.InnerException.Message);
                }
                LogToFileServer(LOG_FOLDER_SERVER, errorMessage + Environment.NewLine + ex.StackTrace);

            }
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        static ISessionFactory CreateSessionFactoryForDdl()
        {
            FluentConfiguration cfg = Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2008
                                  .ConnectionString(c => c.FromConnectionStringWithKey("MSSQL2012"))
                                    .Dialect<MsSql2012Dialect>()
                    );

                cfg.Mappings(m => m
                    .FluentMappings
                        .AddFromAssemblyOf<NHibernateHelper>()
                    );

                cfg.ExposeConfiguration(config =>
                        {
                            SchemaExport schemaExport = new SchemaExport(config);
                            schemaExport.SetOutputFile(AppDomain.CurrentDomain.BaseDirectory + @"\LedgerLinkDbScript.sql");
                            schemaExport.Drop(true, true);
                            schemaExport.Create(true, true);
                            schemaExport.Execute(false, false, false);
                        });
               SessionFactory = cfg.BuildSessionFactory();
            

            return SessionFactory;
        }

        public static ISession OpenSessionForDdl()
        {
            CreateSessionFactoryForDdl();
            return SessionFactory.OpenSession();
        }




        public static PropertyDescriptorCollection GetDisplayableProperties<T>()
        {
            // Get the standard descriptors as a starting point
            PropertyDescriptorCollection origProperties = TypeDescriptor.GetProperties(typeof(T));

            // This is the list of properties I want to use in the end
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

            foreach (PropertyDescriptor descriptor in origProperties)
            {
                // Based on some logic, decide whether we want to use that original
                // descriptor or not.
                //OMM: Leave out the Collections properties of the Employee class
                if (!typeof(ICollection).IsAssignableFrom(descriptor.PropertyType))
                    properties.Add(descriptor);
            }

            // Possibly do further modifications or add custom descriptors to the list
            //...             

            // Finally, create and return the result
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        #region Logging Methods
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
