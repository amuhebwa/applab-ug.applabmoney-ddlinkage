using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;

namespace DigitizingDataAdminApp.Models
{
    public class DataLogging
    {
        string filePath;
        public DataLogging() {

            filePath = @"~/App_Data/Logs.txt";

        }
        // Method to do the actual writing to the file
        public void writeLogsToFile(string action) {
            String data = action + " " + DateTime.Now.ToString();

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
            }
            using (StreamWriter writer = new StreamWriter(System.Web.Hosting.HostingEnvironment.MapPath(filePath), true))
            {
                writer.WriteLine(data);
            }
        }
    }
}