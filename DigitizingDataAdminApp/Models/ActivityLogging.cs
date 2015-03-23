using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DigitizingDataAdminApp.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace DigitizingDataAdminApp.Models
{
    public class ActivityLogging
    {
        /**
         * Log all interactions between the user and the system
         * */
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