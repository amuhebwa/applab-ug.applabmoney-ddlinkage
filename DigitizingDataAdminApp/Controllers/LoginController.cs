using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace DigitizingDataAdminApp.Controllers
{
    public class LoginController : Controller
    {
        ActivityLoggingSystem activityLoggingSystem;
        ledgerlinkEntities database;
        PasswordHashing passwordHashing;
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LoginController()
        {
            activityLoggingSystem = new ActivityLoggingSystem();
            database = new ledgerlinkEntities();
            passwordHashing = new PasswordHashing();
        }
        public ActionResult Index()
        {
            return View();
        }
        /**
         * Queries the database and validates the submitted name against registered users
         * logs in user if the registered cresential are right, and re-directs to the login
         * page if they are wrong
         **/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                string password = passwordHashing.hashedPassword(user.Password);
                var current_user = database.Users.Where(a => a.Username.Equals(user.Username)
                    && a.Password.Equals(password)).FirstOrDefault();
                if (current_user != null)
                {
                    Session["UserId"] = current_user.Id.ToString();
                    Session["Username"] = current_user.Username;
                    Session["UserLevel"] = current_user.UserLevel;
                    FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                    String logString = current_user.Username + " Logged In Successfully";
                    activityLoggingSystem.logActivity(logString, 0);
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "The Username or Password provided is wrong!");
                    String logString = "Failed to login with Username : " + " " + user.Username + " and Password : " + user.Password;
                    activityLoggingSystem.logActivity(logString, 1);
                }
            }

            return View();
        }
    }
}

