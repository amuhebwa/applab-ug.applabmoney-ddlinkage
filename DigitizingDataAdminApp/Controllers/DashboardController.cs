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
            var cbt_details = (from data in db.Cbt_info select data);
            foreach (var item in cbt_details)
            {
                cbts.Add(new CbtInformation
                {
                    Id = item.Id,
                    Name = item.Name,
                    Region = item.Region,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    Status = item.Status
                });
            }
            return cbts;
        }
    }
}
