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
using System.Web.Script.Serialization;
using DigitizingDataBizLayer.Repositories;
using DigitizingDataDomain.Model;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace DigitizingDataAdminApp.Controllers
{
    public class LoginController : Controller
    {
        UserRepo _userRepo = null;
        ActivityLoggingSystem activityLoggingSystem;
        readonly log4net.ILog logger =null;  

        public LoginController()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            activityLoggingSystem = new ActivityLoggingSystem();
            _userRepo = new UserRepo();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                Users _currentUser = _userRepo.checkIfUserExists(user.Username, user.Password);
                if (_currentUser != null)
                {
                    Session["UserId"] = _currentUser.Id.ToString();
                    Session["Username"] = _currentUser.Username;
                    Session["UserLevel"] = _currentUser.UserLevel;
                    FormsAuthentication.SetAuthCookie(user.Id.ToString(), false);

                    String logString = _currentUser.Username + " Logged In Successfully";
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

