using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;
using System.Web.Security;

namespace DigitizingDataAdminApp.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }
        // Action result for logging in
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                using (ledgerlinkEntities database = new ledgerlinkEntities())
                {
                    var current_user = database.Users.Where(a => a.Username.Equals(user.Username)
                        && a.Password.Equals(user.Password)).FirstOrDefault();
                    if (current_user != null)
                    {
                        Session["UserId"] = current_user.Id.ToString();
                        Session["Username"] = current_user.Username;
                        FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                        string action = "Logged in";
                        logUserActivity(action);
                        return RedirectToAction("Dashboard", "Dashboard");
                    }

                }

            }
            return View(user);
        }
        // Log the user activities
        public void logUserActivity(string action)
        {
            object session_id = Session["UserId"];
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
        } // EOM

    }
}

