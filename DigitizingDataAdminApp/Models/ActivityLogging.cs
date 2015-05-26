using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitizingDataAdminApp.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;

namespace DigitizingDataAdminApp.Models
{
    public class ActivityLogging
    {
        /**
         * Log all interactions between the user and the system
         * */
        string user_action;
        string path;
        FileStream fileStream;
        // Constructor
        public ActivityLogging()
        {
            fileStream = null;
            user_action = "";
            path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/LogFile.txt");

        }
        public void addLogIfnormation(String action)
        {
            user_action = action + " ---> " + System.DateTime.Now;
            if (!File.Exists(path))
            {
                using (fileStream = File.Create(path))
                {
                }
            }
            else if (File.Exists(path))
            {
                using (StreamWriter streamWriter = new StreamWriter(path, true))
                {
                    streamWriter.WriteLine(user_action);
                }

            }
        }
        public void logUserActivity(string action)
        {
            object session_id = HttpContext.Current.Session["UserId"];

            // Only log the data if the session is not null
            if (session_id != null)
            {
                Audit_Log log = new Audit_Log();
                log.LogDate = DateTime.Today.Date;

                log.UserId = Convert.ToInt32(session_id);
                log.ActionPerformed = action;
                ledgerlinkEntities ll = new ledgerlinkEntities();
                ll.Audit_Log.Add(log);
                ll.SaveChanges();
            }
        }
    }
}