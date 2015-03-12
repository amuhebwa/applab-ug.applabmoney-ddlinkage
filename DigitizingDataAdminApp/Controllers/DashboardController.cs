﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;
using System.Web.Security;

namespace DigitizingDataAdminApp.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/
        [Authorize]
        public ActionResult Dashboard()
        {
            if (Session["UserId"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Index");
            }
        }
        // Action Result for user details
        public ActionResult UsersData()
        {
            AllUsersInformation all_users = new AllUsersInformation();
            List<UserInformation> single_user = new List<UserInformation>();
            single_user = users_infor();
            all_users.AllUsersList = single_user;
            string action = "Viewed all users";
            logUserActivity(action);
            return View(all_users);
        }
        // Action Result for vsla details
        public ActionResult VslaData()
        {
            AllVslaInformation all_vsla = new AllVslaInformation();
            List<VslaInformation> single_vsla = new List<VslaInformation>();
            single_vsla = vsla_info();
            all_vsla.AllVslaList = single_vsla;
            string action = "Viewed all village lending and saving associations information";
            logUserActivity(action);
            return View(all_vsla);

        }
        // Action Result for CBT Data
        public ActionResult CbtData()
        {
            AllCbtInformation all_cbt = new AllCbtInformation();
            List<CbtInformation> single_cbt = new List<CbtInformation>();
            single_cbt = cbt_info();
            all_cbt.AllCbtList = single_cbt;
            string action = "Viewed all commnity based trainers information";
            logUserActivity(action);
            return View(all_cbt);
        }
        // Audit user logs
        public ActionResult LogsData()
        {
            AllLogsInformation all_logs = new AllLogsInformation();
            List<LogsInformation> info_logs = new List<LogsInformation>();
            info_logs = logs_list();
            all_logs.AllLogsList = info_logs;
            string action = "Viewed log information";
            logUserActivity(action);
            return View(all_logs);
        }
        /**
         * Logout
         * Note : The user-activity logging function should be called
         * before logging FormAutentication.SignOut() ie before the user
         * session is destroyed
         * 
         **/
        public ActionResult Logout()
        {
            string action = "logged out";
            logUserActivity(action);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");

        }
        // Function to add a new user to the system
        public ActionResult AddUser()
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            List<UserPermission> permission = db.UserPermissions.OrderBy(a => a.Level_Id).ToList();
            SelectList accessPermissions = new SelectList(permission, "Level_Id", "UserType");
            UserInformation data = new UserInformation
            {
                AccessLevel = accessPermissions
            };
            return View(data);
        }

        [HttpPost]
        public ActionResult AddUser(User user, int Level_Id)
        {
            if (ModelState.IsValid)
            {
                ledgerlinkEntities db = new ledgerlinkEntities();
                User usr = new User
                {
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Password = user.Password,
                    Email = user.Email,
                    UserLevel = Level_Id
                };
                db.Users.Add(usr);
                db.SaveChanges();
                ModelState.Clear();
                string action = "Added a new user called " + user.Username;
                logUserActivity(action);
                user = null;
            }
            return RedirectToAction("UsersData");
        }
        // Function to edit the user
        [HttpGet]
        public ActionResult EditUser(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var user_details = (from table_users in db.Users
                                join table_permissions in db.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                where table_users.Id == id
                                select new { db_user = table_users, db_permissions = table_permissions }).Single();
            List<UserPermission> user_levels = db.UserPermissions.OrderBy(a => a.UserType).ToList();
            SelectList user_types = new SelectList(user_levels, "Level_Id", "UserType", user_details.db_user.UserLevel);
            UserInformation user_data = new UserInformation
            {
                Id = user_details.db_user.Id,
                Username = user_details.db_user.Username,
                Password = user_details.db_user.Password,
                Fullname = user_details.db_user.Fullname,
                Email = user_details.db_user.Email,
                UserTypes = user_types,
            };
            return View(user_data);

        }
        [HttpPost]
        public ActionResult EditUser(UserInformation info, int id, int Level_Id)
        {
            using (ledgerlinkEntities database = new ledgerlinkEntities())
            {
                var query = database.Users.Find(id);
                query.Username = info.Username;
                query.Password = info.Password;
                query.Fullname = info.Fullname;
                query.Email = info.Email;
                query.UserLevel = Level_Id;
                database.SaveChanges();
                string action = "Edited information for " + info.Fullname;
                logUserActivity(action);
                return RedirectToAction("UsersData");
            }
        }
        // Delete a user
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var user_details = (from table_users in db.Users
                                join table_permissions in db.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                where table_users.Id == id
                                select new { db_user = table_users, db_permissions = table_permissions }).Single();

            UserInformation user_data = new UserInformation
            {
                Id = user_details.db_user.Id,
                Username = user_details.db_user.Username,
                Password = user_details.db_user.Password,
                Fullname = user_details.db_user.Fullname,
                Email = user_details.db_user.Email,
                UserLevel = user_details.db_permissions.UserType
            };
            string action = "Deleted information for " + user_details.db_user.Fullname;
            logUserActivity(action);
            return View(user_data);
        }

        [HttpPost]
        public ActionResult DeleteUser(User user, int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            user.Id = id;
            db.Users.Attach(user);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("UsersData");
        }

        // Display all the user details
        public ActionResult UserDetails(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var user_details = (from table_users in db.Users
                                join table_permissions in db.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                where table_users.Id == id
                                select new { db_user = table_users, db_permissions = table_permissions }).Single();


            UserInformation user_data = new UserInformation
            {
                Id = user_details.db_user.Id,
                Username = user_details.db_user.Username,
                Password = user_details.db_user.Password,
                Fullname = user_details.db_user.Fullname,
                Email = user_details.db_user.Email,
                UserLevel = user_details.db_permissions.UserType
            };
            string action = "Viewed list of all users in the System";
            logUserActivity(action);
            return View(user_data);
        }
        // Get the VSLA detailed information
        public ActionResult VslaDetails(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vsla_info = (from table_vsla in database.Vslas
                             join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                             join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                             where table_vsla.VslaId == id
                             select new { db_vsla = table_vsla, db_cbt = table_cbt, db_regions = table_regions, db_status = table_status }).Single();

            VslaInformation vsla_data = new VslaInformation
            {
                VslaId = vsla_info.db_vsla.VslaId,
                VslaCode = vsla_info.db_vsla.VslaCode ?? "--",
                VslaName = vsla_info.db_vsla.VslaName ?? "--",
                RegionId = vsla_info.db_regions.RegionName,
                DateRegistered = vsla_info.db_vsla.DateRegistered.HasValue ? vsla_info.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla_info.db_vsla.DateLinked.HasValue ? vsla_info.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla_info.db_vsla.ContactPerson,
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber,
                CBT = vsla_info.db_cbt.Name ?? "--",
                Status = vsla_info.db_status.CurrentStatus
            };
            string action = "Viewed all information for VSLA named " + vsla_info.db_vsla.VslaName;
            logUserActivity(action);
            return View(vsla_data);
        }
        // Edit VSLA
        [HttpGet]
        public ActionResult EditVsla(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();

            var vsla_info = (from table_vsla in database.Vslas
                             join table_cbt in
                                 database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             where table_vsla.VslaId == id
                             select new { db_vsla = table_vsla, db_cbt = table_cbt }).Single();

            // Get a list of all vsla regions to populate in the dropdown list
            List<VslaRegion> all_vsla_regions = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList vsla_Regions = new SelectList(all_vsla_regions, "RegionId", "RegionName", vsla_info.db_vsla.RegionId);
            // Get the list of all cbts to populate in the dropdown list
            List<Cbt_info> cbt_list = database.Cbt_info.OrderBy(a => a.Name).ToList();
            SelectList cbt_info = new SelectList(cbt_list, "Id", "Name", vsla_info.db_vsla.CBT);
            // Get the status type ie active/inactive,
            List<StatusType> status = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList status_types = new SelectList(status, "Status_Id", "CurrentStatus", vsla_info.db_cbt.Status);

            VslaInformation vsla_data = new VslaInformation
            {
                VslaId = vsla_info.db_vsla.VslaId,
                VslaCode = vsla_info.db_vsla.VslaCode ?? "--",
                VslaName = vsla_info.db_vsla.VslaName ?? "--",
                VslaRegionsModel = vsla_Regions,
                DateRegistered = vsla_info.db_vsla.DateRegistered.HasValue ? vsla_info.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla_info.db_vsla.DateLinked.HasValue ? vsla_info.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla_info.db_vsla.ContactPerson,
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber,
                CbtModel = cbt_info,
                StatusType = status_types
            };
            string action = "Edited information for VSLA named " + vsla_info.db_vsla.VslaName ?? "--";
            logUserActivity(action);
            return View(vsla_data);
        }
        [HttpPost]
        public ActionResult EditVsla(VslaInformation vsla, int VslaId, int Id, int RegionId, int Status_Id)
        {
            using (ledgerlinkEntities database = new ledgerlinkEntities())
            {
                var query = database.Vslas.Find(VslaId);
                query.VslaCode = vsla.VslaCode;
                query.VslaName = vsla.VslaName;
                query.VslaPhoneMsisdn = vsla.VslaPhoneMsisdn;
                query.GpsLocation = vsla.GpsLocation;
                query.DateRegistered = vsla.DateRegistered;
                query.DateLinked = vsla.DateLinked;
                query.RegionId = RegionId;
                query.ContactPerson = vsla.ContactPerson;
                query.PositionInVsla = vsla.PositionInVsla;
                query.PhoneNumber = vsla.PhoneNumber;
                query.CBT = Id;
                query.Status = Status_Id;

                database.SaveChanges();
                return RedirectToAction("VslaData");
            }
        }
        // Method for adding a new vsla
        public ActionResult AddVsla()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();

            // Get all cbts
            List<Cbt_info> cbt_list = database.Cbt_info.OrderBy(a => a.Name).ToList();
            SelectList cbt_info = new SelectList(cbt_list, "Id", "Name");

            // Get all vsla regions
            List<VslaRegion> all_regions = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regions_list = new SelectList(all_regions, "RegionId", "RegionName");

            // Get the status type ie active/inactive
            List<StatusType> status = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList status_list = new SelectList(status, "Status_Id", "CurrentStatus");

            VslaInformation vsla_list_options = new VslaInformation
            {
                VslaRegionsModel = regions_list,
                CbtModel = cbt_info,
                StatusType = status_list
            };
            return View(vsla_list_options);
        }
        [HttpPost]
        public ActionResult AddVsla(Vsla new_vsla, int RegionId, int Id, int Status_Id)
        {
            if (ModelState.IsValid)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                Vsla added_vsla = new Vsla
                {
                    VslaCode = new_vsla.VslaCode,
                    VslaName = new_vsla.VslaName,
                    RegionId = RegionId,
                    DateRegistered = new_vsla.DateRegistered.HasValue ? new_vsla.DateRegistered : System.DateTime.Now,
                    DateLinked = new_vsla.DateLinked.HasValue ? new_vsla.DateLinked : System.DateTime.Now,
                    PhysicalAddress = new_vsla.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = new_vsla.VslaPhoneMsisdn ?? "--",
                    GpsLocation = new_vsla.GpsLocation ?? "--",
                    ContactPerson = new_vsla.ContactPerson,
                    PositionInVsla = new_vsla.PositionInVsla,
                    PhoneNumber = new_vsla.PhoneNumber,
                    CBT = Id,
                    Status = Status_Id
                };
                database.Vslas.Add(added_vsla);
                database.SaveChanges();
                string action = "Added new  VSLA named " + new_vsla.VslaName;
                logUserActivity(action);
                return RedirectToAction("VslaData");
            }
            return View(new_vsla);
        }
        // Delete a VSLA
        [HttpGet]
        public ActionResult DeleteVsla(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vsla_info = (from table_vsla in database.Vslas
                             join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                             join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                             where table_vsla.VslaId == id
                             select new { db_vsla = table_vsla, db_cbt = table_cbt, db_regions = table_regions, db_status = table_status }).Single();

            VslaInformation vsla_data = new VslaInformation
            {
                VslaId = vsla_info.db_vsla.VslaId,
                VslaCode = vsla_info.db_vsla.VslaCode ?? "--",
                VslaName = vsla_info.db_vsla.VslaName ?? "--",
                RegionId = vsla_info.db_regions.RegionName,
                DateRegistered = vsla_info.db_vsla.DateRegistered.HasValue ? vsla_info.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla_info.db_vsla.DateLinked.HasValue ? vsla_info.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla_info.db_vsla.ContactPerson,
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber,
                CBT = vsla_info.db_cbt.Name ?? "--",
                Status = vsla_info.db_status.CurrentStatus
            };
            string action = "Deleted all information for VSLA named " + vsla_info.db_vsla.VslaName;
            logUserActivity(action);
            return View(vsla_data);
        }
        [HttpPost]
        public ActionResult DeleteVsla(Vsla vsla, int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            vsla.VslaId = id;
            db.Vslas.Attach(vsla);
            db.Vslas.Remove(vsla);
            db.SaveChanges();
            return RedirectToAction("VslaData");
        }

        // Add a new CBT
        public ActionResult AddCbt()
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            // Regions
            List<VslaRegion> all_regions = db.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regions_list = new SelectList(all_regions, "RegionId", "RegionName");
            // Status types
            List<StatusType> status = db.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList statusTypes = new SelectList(status, "Status_Id", "CurrentStatus");
            CbtInformation regionsSelector = new CbtInformation
            {
                VslaRegionsModel = regions_list,
                StatusType = statusTypes
            };
            return View(regionsSelector);
        }
        [HttpPost]
        public ActionResult AddCbt(Cbt_info new_cbt, int RegionId, int Status_Id)
        {
            if (ModelState.IsValid)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                Cbt_info cbx = new Cbt_info
                {
                    Name = new_cbt.Name,
                    Region = RegionId,
                    PhoneNumber = new_cbt.PhoneNumber,
                    Email = new_cbt.Email,
                    Status = Status_Id

                };
                string action = "Added a new CBT called " + new_cbt.Name;
                logUserActivity(action);
                database.Cbt_info.Add(cbx);
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
            return View(new_cbt);
        }

        // View individual cbt information
        public ActionResult CbtDetails(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                   join table_status in db.StatusTypes on table_cbt.Status equals table_status.Status_Id
                                   where table_cbt.Id == id
                                   select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status }).Single();
            CbtInformation cbt_data = new CbtInformation
            {
                Id = all_information.dt_cbt.Id,
                Name = all_information.dt_cbt.Name,
                Region = all_information.dt_region.RegionName,
                PhoneNumber = all_information.dt_cbt.PhoneNumber,
                Email = all_information.dt_cbt.Email,
                Status = all_information.dt_status.CurrentStatus
            };
            string action = "Viewed CBT details for  " + all_information.dt_cbt.Name;
            logUserActivity(action);
            return View(cbt_data);
        }
        // Edit CBT information
        [HttpGet]
        public ActionResult EditCbt(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                   where table_cbt.Id == id
                                   select new { dt_cbt = table_cbt, db_region = table_region }).Single();

            // all reegions
            List<VslaRegion> all_regions = db.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regions_list = new SelectList(all_regions, "RegionId", "RegionName", all_information.db_region.RegionId);
            // Get the status ie active/inactive
            List<StatusType> status = db.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList status_list = new SelectList(status, "Status_Id", "CurrentStatus", all_information.dt_cbt.Status);

            // Create a cbt object
            CbtInformation cbt_data = new CbtInformation
            {
                Id = all_information.dt_cbt.Id,
                Name = all_information.dt_cbt.Name,
                VslaRegionsModel = regions_list,
                PhoneNumber = all_information.dt_cbt.PhoneNumber,
                Email = all_information.dt_cbt.Email,
                StatusType = status_list
            };
            string action = "Edited CBT information for  " + all_information.dt_cbt.Name;
            logUserActivity(action);
            return View(cbt_data);
        }
        [HttpPost]
        public ActionResult EditCbt(Cbt_info cbt, int id, int RegionId, int Status_Id)
        {
            using (ledgerlinkEntities database = new ledgerlinkEntities())
            {
                var query = database.Cbt_info.Find(id);
                query.Name = cbt.Name;
                query.Region = RegionId;
                query.PhoneNumber = cbt.PhoneNumber;
                query.Email = cbt.Email;
                query.Status = Status_Id;
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }

        }
        // Delete CBT information
        [HttpGet]
        public ActionResult DeleteCbt(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                   join table_status in db.StatusTypes on table_cbt.Status equals table_status.Status_Id
                                   where table_cbt.Id == id
                                   select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status }).Single();
            CbtInformation cbt_data = new CbtInformation
            {
                Id = all_information.dt_cbt.Id,
                Name = all_information.dt_cbt.Name,
                Region = all_information.dt_region.RegionName,
                PhoneNumber = all_information.dt_cbt.PhoneNumber,
                Email = all_information.dt_cbt.Email,
                Status = all_information.dt_status.CurrentStatus
            };
            string action = "Deletedd CBT information for  " + all_information.dt_cbt.Name;
            logUserActivity(action);
            return View(cbt_data);
        }
        [HttpPost]
        public ActionResult DeleteCbt(Cbt_info cbt, int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            cbt.Id = id;
            db.Cbt_info.Attach(cbt);
            db.Cbt_info.Remove(cbt);
            db.SaveChanges();
            return RedirectToAction("CbtData");
        }

        // ----HELPER METHODS-----
        // 1. Helper method to get information for all registered users
        public List<UserInformation> users_infor()
        {
            List<UserInformation> users = new List<UserInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var user_details = (from table_users in db.Users
                                join table_permissions in db.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                select new { db_user = table_users, db_permissions = table_permissions });
            foreach (var item in user_details)
            {
                users.Add(
                    new UserInformation
                    {
                        Id = item.db_user.Id,
                        Username = item.db_user.Username,
                        Fullname = item.db_user.Fullname,
                        Email = item.db_user.Email,
                        UserLevel = item.db_permissions.UserType
                    });
            }

            return users;

        }
        // 2. Helper method to get all information concerning vsla
        public List<VslaInformation> vsla_info()
        {
            List<VslaInformation> vsla_list = new List<VslaInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var vsla_details = (from data in db.Vslas select data);
            foreach (var item in vsla_details)
            {
                vsla_list.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.RegionId.ToString(), // change this back to integer
                    DateRegistered = item.DateRegistered.HasValue ? item.DateRegistered : System.DateTime.Today,
                    DateLinked = item.DateLinked.HasValue ? item.DateLinked : System.DateTime.Today,
                    PhysicalAddress = item.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = item.VslaPhoneMsisdn ?? "--",
                    GpsLocation = item.GpsLocation ?? "--",
                    ContactPerson = item.ContactPerson ?? "--",
                    PositionInVsla = item.PositionInVsla ?? "--",
                    PhoneNumber = item.PhoneNumber ?? "--",
                    CBT = "--",
                    Status = item.Status.ToString() ?? "--"
                });
            }
            return vsla_list;
        }
        // 3. Helper method to get the list of all CBTS that have been added to a system
        public List<CbtInformation> cbt_info()
        {
            List<CbtInformation> cbts = new List<CbtInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();

            var data = (from table_cbt in db.Cbt_info
                        join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                        join table_status in db.StatusTypes on table_cbt.Status equals table_status.Status_Id
                        select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status });

            foreach (var item in data)
            {
                cbts.Add(new CbtInformation
                {
                    Id = item.dt_cbt.Id,
                    Name = item.dt_cbt.Name,
                    Region = item.dt_region.RegionName,
                    PhoneNumber = item.dt_cbt.PhoneNumber,
                    Email = item.dt_cbt.Email,
                    Status = item.dt_status.CurrentStatus
                });
            }

            return cbts;
        }
        //   get a list of logs
        public List<LogsInformation> logs_list()
        {
            List<LogsInformation> logs = new List<LogsInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            var logs_info = (from database_logs in database.Audit_Log
                             join database_users in database.Users on
                                 database_logs.UserId equals database_users.Id
                             select new { db_logs = database_logs, db_users = database_users });

            foreach (var data in logs_info)
            {
                logs.Add(new LogsInformation
                {
                    LogId = data.db_logs.LogId,
                    userId = data.db_users.Username,
                    LogTime = data.db_logs.LogDate,
                    ActionPerformed = data.db_logs.ActionPerformed
                });
            }
            return logs;
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
        }
    }
}
