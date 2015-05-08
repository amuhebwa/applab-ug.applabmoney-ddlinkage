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
        String filePath;
        public DataLogging() {
            filePath = "~/App_Data/Logs/Text.txt";
        }
        // Method to do the actual writing to the file
        public void writeLogsToFile(string action) {
            String data = action + " " + DateTime.Now.ToString();
            using (StreamWriter writer = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath(filePath), true))
            {
                writer.WriteLine(data);
            }
        }
    }
}