using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;

namespace DigitizingDataAdminApp.Controllers
{
    public class LoginController : Controller
    {
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
                    ledgerlinkEntities database = new ledgerlinkEntities();
                    PasswordHashing passwordHashing = new PasswordHashing();
                    ActivityLogging activityLogging = new ActivityLogging();
                    string password = passwordHashing.hashedPassword(user.Password);
                    var current_user = database.Users.Where(a => a.Username.Equals(user.Username)
                        && a.Password.Equals(password)).FirstOrDefault();
                    if (current_user != null)
                    {
                        Session["UserId"] = current_user.Id.ToString();
                        Session["Username"] = current_user.Username;
                        FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                        string action = "Logged in";
                        activityLogging.logUserActivity(action);
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                    else {
                        ModelState.AddModelError("", "The Username or password provided is wrong!!");
                    }
            }
          
            return View();
        }
    }
}

