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

namespace DigitizingDataAdminApp.Controllers
{
    public class LoginController : Controller
    {
        DataLogging dataLogging;
        ledgerlinkEntities database;
        PasswordHashing passwordHashing;
        public LoginController()
        {
            dataLogging = new DataLogging();
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
                string action;
                string password = passwordHashing.hashedPassword(user.Password);
                var current_user = database.Users.Where(a => a.Username.Equals(user.Username)
                    && a.Password.Equals(password)).FirstOrDefault();
                if (current_user != null)
                {
                    Session["UserId"] = current_user.Id.ToString();
                    Session["Username"] = current_user.Username;
                    FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);
                    action = "Logged in as " + user.Username;
                    dataLogging.writeLogsToFile(action);

                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("", "The Username or Password provided is wrong!!");
                    action = "Failed to login with " + " " + user.Username + " and " + user.Password;
                    dataLogging.writeLogsToFile(action);
                }
            }

            return View();
        }
    }
}

