using System;
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
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            return View();
        }
        // Action Result for user details
        public ActionResult UsersData() {
            AllUsersInformation all_users = new AllUsersInformation();
            List<UserInformation> single_user = new List<UserInformation>();
            single_user = users_infor();
            all_users.AllUsersList = single_user;
            return View(all_users);
        }
        // Action Result for vsla details
        public ActionResult VslaData() {
            AllVslaInformation all_vsla = new AllVslaInformation();
            List<VslaInformation> single_vsla = new List<VslaInformation>();
            single_vsla = vsla_info();
            all_vsla.AllVslaList = single_vsla;
            return View(all_vsla);
            
        }
        // Action Result for CBT Data
        public ActionResult CbtData() {
            AllCbtInformation all_cbt = new AllCbtInformation();
            List<CbtInformation> single_cbt = new List<CbtInformation>();
            single_cbt = cbt_info();
            all_cbt.AllCbtList = single_cbt;

            return View(all_cbt);
        }
        // Audit user logs
        public ActionResult AuditLogs() {
            return View();
        }
        // Logout
        public ActionResult Logout() {
            
            return View();
        }
        // Function to add a new user to the system
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                using (ledgerlinkEntities db = new ledgerlinkEntities())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    ModelState.Clear();
                    user = null;
                    ViewBag.Message = "New User has been added";
                }
            }
            //return View(user);
            return RedirectToAction("UsersData");
        }
        // Function to edit the user
        [HttpGet]
        public ActionResult EditUser(int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var usr = db.Users.Find(id);
            UserInformation user_data = new UserInformation
            {
                Id = usr.Id,
                Username = usr.Username,
                Password = usr.Password,
                Fullname = usr.Fullname,
                Email = usr.Email,
                UserLevel = usr.UserLevel
            };
            return View(user_data);
        
        }
        [HttpPost]
        public ActionResult EditUser(UserInformation info, int id) {
            using (ledgerlinkEntities database = new ledgerlinkEntities())
            {
                var query = database.Users.Find(id);
                query.Username = info.Username;
                query.Password = info.Password;
                query.Fullname = info.Fullname;
                query.Email = info.Email;
                query.UserLevel = info.UserLevel;
                database.SaveChanges();
                return RedirectToAction("UsersData");
            }
        }
        // Delete a user
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var usr = db.Users.Find(id);
            UserInformation user_data = new UserInformation
            {
                Id = usr.Id,
                Username = usr.Username,
                Password = usr.Password,
                Fullname = usr.Fullname,
                Email = usr.Email,
                UserLevel = usr.UserLevel
            };
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
        public ActionResult UserDetails(int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var usr = db.Users.Find(id);
            UserInformation user_data = new UserInformation
            {
                Id = usr.Id,
                Username = usr.Username,
                Password = usr.Password,
                Fullname = usr.Fullname,
                Email = usr.Email,
                UserLevel = usr.UserLevel
            };
            return View(user_data);
        }
        // Get the VSLA detailed information
    public ActionResult VslaDetails(int id) {
        ledgerlinkEntities database = new ledgerlinkEntities();
        var vsla = database.Vslas.Find(id);
        VslaInformation vsla_data = new VslaInformation {
            VslaId = vsla.VslaId,
            VslaCode = vsla.VslaCode ?? "--",
            VslaName = vsla.VslaName ?? "--",
            RegionId = vsla.RegionId.ToString(), // change this back to integer
            DateRegistered = vsla.DateRegistered.HasValue ? vsla.DateRegistered : System.DateTime.Today,
            DateLinked = vsla.DateLinked.HasValue ? vsla.DateLinked : System.DateTime.Today,
            PhysicalAddress = vsla.PhysicalAddress ?? "--",
            VslaPhoneMsisdn = vsla.VslaPhoneMsisdn ?? "--",
            GpsLocation = vsla.GpsLocation ?? "--",
            ContactPerson = vsla.ContactPerson ?? "--",
            PositionInVsla = vsla.PositionInVsla ?? "--",
            PhoneNumber = vsla.PhoneNumber ?? "--",
            CBT = vsla.CBT.HasValue ? (int)vsla.CBT : 00,
            Status = vsla.Status ?? "--"

        };
        return View(vsla_data);
    }
        // Edit VSLA
        [HttpGet]
    public ActionResult EditVsla(int id) {
        ledgerlinkEntities database = new ledgerlinkEntities();
        var vsla = database.Vslas.Find(id);
        VslaInformation vsla_data = new VslaInformation
        {
            VslaCode = vsla.VslaCode ?? "--",
            VslaName = vsla.VslaName ?? "--",
            RegionId = vsla.RegionId.ToString(), // change this back to integer
            DateRegistered = vsla.DateRegistered.HasValue ? vsla.DateRegistered : System.DateTime.Today,
            DateLinked = vsla.DateLinked.HasValue ? vsla.DateLinked : System.DateTime.Today,
            PhysicalAddress = vsla.PhysicalAddress ?? "--",
            VslaPhoneMsisdn = vsla.VslaPhoneMsisdn ?? "--",
            GpsLocation = vsla.GpsLocation ?? "--",
            ContactPerson = vsla.ContactPerson ?? "--",
            PositionInVsla = vsla.PositionInVsla ?? "--",
            PhoneNumber = vsla.PhoneNumber ?? "--",
            CBT = vsla.CBT.HasValue ? (int)vsla.CBT : 00,
            Status = vsla.Status ?? "--"

        };
        return View(vsla_data);
    }
        [HttpPost]
        public ActionResult EditVsla(VslaInformation vsla, int id) {
            using (ledgerlinkEntities database = new ledgerlinkEntities())
            {
                var query = database.Vslas.Find(id);
                query.VslaCode = vsla.VslaCode;
                query.VslaName = vsla.VslaName;
                query.VslaPhoneMsisdn = vsla.VslaPhoneMsisdn;
                query.GpsLocation = vsla.GpsLocation;
                query.DateRegistered = vsla.DateRegistered;
                query.DateLinked = vsla.DateLinked;
                query.RegionId = int.Parse(vsla.RegionId);
                query.ContactPerson = vsla.ContactPerson;
                query.PositionInVsla = vsla.PositionInVsla;
                query.PhoneNumber = vsla.PhoneNumber;
                query.CBT = vsla.CBT;
                query.Status = vsla.Status;

                database.SaveChanges();
                return RedirectToAction("VslaData");
            }
        }
        // Method for adding a new vsla
        public ActionResult AddVsla() {
            return View();
        }
        [HttpPost]
        public ActionResult AddVsla(Vsla new_vsla)
        {
            if (ModelState.IsValid)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                database.Vslas.Add(new_vsla);
                database.SaveChanges();
                return RedirectToAction("VslaData");
            }
            return View(new_vsla);
        }
        // Delete a VSLA
        [HttpGet]
        public ActionResult DeleteVsla(int id) {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vsla = database.Vslas.Find(id);
            VslaInformation vsla_data = new VslaInformation
            {
                VslaId = vsla.VslaId,
                VslaCode = vsla.VslaCode ?? "--",
                VslaName = vsla.VslaName ?? "--",
                RegionId = vsla.RegionId.ToString(), // change this back to integer
                DateRegistered = vsla.DateRegistered.HasValue ? vsla.DateRegistered : System.DateTime.Today,
                DateLinked = vsla.DateLinked.HasValue ? vsla.DateLinked : System.DateTime.Today,
                PhysicalAddress = vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla.GpsLocation ?? "--",
                ContactPerson = vsla.ContactPerson ?? "--",
                PositionInVsla = vsla.PositionInVsla ?? "--",
                PhoneNumber = vsla.PhoneNumber ?? "--",
                CBT = vsla.CBT.HasValue ? (int)vsla.CBT : 00,
                Status = vsla.Status ?? "--"

            };
            return View(vsla_data);
        }
        [HttpPost]
        public ActionResult DeleteVsla(Vsla vsla, int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();
            vsla.VslaId = id;
            db.Vslas.Attach(vsla);
            db.Vslas.Remove(vsla);
            db.SaveChanges();
            return RedirectToAction("VslaData");
        }
        // ---- CBT Methods
        // Add a new CBT
        public ActionResult AddCbt() {
            return View();
        }
        [HttpPost]
        public ActionResult AddCbt(Cbt_info new_cbt) {
            if (ModelState.IsValid)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                database.Cbt_info.Add(new_cbt);
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
            return View(new_cbt);
        }
        // View individual cbt information
        public ActionResult CbtDetails(int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region
                                       in db.VslaRegions on table_cbt.Region equals table_region.RegionId where table_cbt.Id == id
                                   select new { dt_cbt = table_cbt, db_region = table_region }).Single();
            CbtInformation cbt_data = new CbtInformation {
                Id = all_information.dt_cbt.Id,
                Name = all_information.dt_cbt.Name,
                Region = all_information.db_region.RegionName,
                PhoneNumber = all_information.dt_cbt.PhoneNumber,
                Email = all_information.dt_cbt.Email,
                Status = all_information.dt_cbt.Status
            };

           // return View(cbt_data);
            return View(cbt_data);
        }
        // Edit CBT information
        [HttpGet]
        public ActionResult EditCbt(int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();

            // select query for a particular cbt
            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region
                                       in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                   where table_cbt.Id == id
                                   select new { dt_cbt = table_cbt, db_region = table_region }).Single();

            // Get all cbt to populate in the dropdown list
            List<VslaRegion> all_regions = db.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regions_list = new SelectList(all_regions, "RegionId", "RegionName", all_information.db_region.RegionId);

            // Create a cbt object
            CbtInformation cbt_data = new CbtInformation
            {
                Id = all_information.dt_cbt.Id,
                Name = all_information.dt_cbt.Name,
                VslaRegionsModel = regions_list,
                PhoneNumber = all_information.dt_cbt.PhoneNumber,
                Email = all_information.dt_cbt.Email,
                Status = all_information.dt_cbt.Status
            };
            return View(cbt_data);
        }
        [HttpPost]
        public ActionResult EditCbt(Cbt_info cbt, int id, int RegionId)
        {
            using (ledgerlinkEntities database = new ledgerlinkEntities()) {
                var query = database.Cbt_info.Find(id);
                query.Name = cbt.Name;
                query.Region = RegionId;
                query.PhoneNumber = cbt.PhoneNumber;
                query.Email = cbt.Email;
                query.Status = cbt.Status;
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
           
        }
       // Delete CBT information
        [HttpGet]
        public ActionResult DeleteCbt(int id) {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var cbt = db.Cbt_info.Find(id);
            CbtInformation cbt_data = new CbtInformation
            {
                Id = cbt.Id,
                Name = cbt.Name,
                Region = "place holder", // here, delete the foreign key, which is int. So, parse the string to integer
                PhoneNumber = cbt.PhoneNumber,
                Email = cbt.Email,
                Status = cbt.Status
            };
            return View(cbt_data);
        }
        [HttpPost]
        public ActionResult DeleteCbt(Cbt_info cbt, int id) {
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
            var user_details = (from data in db.Users select data);
            foreach (var item in user_details)
            {
                users.Add(
                    new UserInformation
                    {
                        Id = item.Id,
                        Username = item.Username,
                        Fullname = item.Fullname,
                        Email = item.Email,
                        UserLevel = item.UserLevel
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
                    CBT = item.CBT.HasValue ? (int)item.CBT : 11,
                    Status = item.Status ?? "--"
                });
            }
            return vsla_list;
        }
        // 3. Helper method to get the list of all CBTS that have been added to a system
        public List<CbtInformation> cbt_info()
        {
            List<CbtInformation> cbts = new List<CbtInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();

            // selecting from multiple tables
            var all_information = (from table_cbt in db.Cbt_info
                                   join table_region
                                       in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                   select new { dt_cbt = table_cbt, db_region = table_region });
            foreach (var item in all_information) {
                cbts.Add(new CbtInformation 
                {
                Id = item.dt_cbt.Id,
                Name = item.dt_cbt.Name,
                Region = item.db_region.RegionName,
                PhoneNumber = item.dt_cbt.PhoneNumber,
                Email = item.dt_cbt.Email,
                Status = item.dt_cbt.Status
                });
            }

            return cbts;
        }
    }
}
