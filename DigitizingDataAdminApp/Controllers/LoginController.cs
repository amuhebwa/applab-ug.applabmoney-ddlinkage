﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;

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
        public ActionResult Index(UserLogin user) {
            if (ModelState.IsValid) {
                using (ledgerlinkEntities database = new ledgerlinkEntities()) {
                    var current_user = database.Users.Where(a => a.Username.Equals(user.Username) 
                        && a.Password.Equals(user.Password)).FirstOrDefault();
                    if (current_user != null) {
                        Session["UserId"] = current_user.Id.ToString();
                        Session["Username"] = current_user.Username;
                        return RedirectToAction("Home");
                    }

                }
                
            }
            return View(user);
        }
        // This renders the landing page after logging in
        public ActionResult Home() {
            return View();
        }

    }
}
