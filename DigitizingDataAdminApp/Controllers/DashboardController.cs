using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DigitizingDataAdminApp.Models;
using System.Web.Security;
using System.Text;
using System.Security.Cryptography;
using System.Data.Objects.SqlClient;
using System.Web.Helpers;
using System.IO;
namespace DigitizingDataAdminApp.Controllers
{
    public class DashboardController : Controller
    {

        // Initialize the user logging class
        ActivityLogging activityLogging = new ActivityLogging();

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
        /**
         * Get all information concerning registered information
         * */
        public ActionResult UsersData()
        {
            AllUsersInformation allUsers = new AllUsersInformation();
            List<UserInformation> singleUser = new List<UserInformation>();
            singleUser = usersInformation();
            allUsers.AllUsersList = singleUser;
            string action = "Viewed all users";
            activityLogging.logUserActivity(action);
            return View(allUsers);
        }
        /**
         * Get all information concerning VSLAs
         * */
        public ActionResult VslaData()
        {
            AllVslaInformation allVslas = new AllVslaInformation();
            List<VslaInformation> getVslaData = new List<VslaInformation>();
            getVslaData = getVslaInformation();
            allVslas.AllVslaList = getVslaData;
            string action = "Viewed all village lending and saving associations information";
            activityLogging.logUserActivity(action);
            return View(allVslas);

        }


        /**
         * Get all information concerned with CBTs
         * */
        public ActionResult CbtData()
        {
            AllCbtInformation allCbts = new AllCbtInformation();
            List<CbtInformation> getCbtData = new List<CbtInformation>();
            getCbtData = getCbtInformation();
            allCbts.AllCbtList = getCbtData;
            string action = "Viewed all commnity based trainers information";
            activityLogging.logUserActivity(action);
            return View(allCbts);
        }
        /**
         * Display all the log information
         * */
        public ActionResult LogsData()
        {
            AllLogsInformation loggedInformation = new AllLogsInformation();
            List<LogsInformation> loggedData = new List<LogsInformation>();
            loggedData = getAllLogInformation();
            loggedInformation.AllLogsList = loggedData;
            string action = "Viewed log information";
            activityLogging.logUserActivity(action);
            return View(loggedInformation);
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
            activityLogging.logUserActivity(action);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");

        }
        /**
         * Register a new user annd add them into the system
         * */
        public ActionResult AddUser()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            List<UserPermission> permission = database.UserPermissions.OrderBy(a => a.Level_Id).ToList();
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
            if (ModelState.IsValid && user != null)
            {
                PasswordHashing _password = new PasswordHashing();
                string _hashedPassword = _password.hashedPassword(user.Password);
                ledgerlinkEntities database = new ledgerlinkEntities();
                User _user = new User
                {
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Password = _hashedPassword,
                    Email = user.Email,
                    UserLevel = Level_Id
                };
                if (_user != null && _hashedPassword != null)
                {
                    database.Users.Add(_user);
                    database.SaveChanges();
                }
                else
                {
                    return RedirectToAction("AddUser");
                }

                ModelState.Clear();
                string action = "Added a new user called " + user.Username;
                activityLogging.logUserActivity(action);
                user = null;
                return RedirectToAction("UsersData");
            }
            return View();
        }

        /**
         * Edit a particular user's information.
         * */
        [HttpGet]
        public ActionResult EditUser(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var userDetails = (from table_users in db.Users
                               join table_permissions in db.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                               where table_users.Id == id
                               select new { db_user = table_users, db_permissions = table_permissions }).Single();
            List<UserPermission> user_levels = db.UserPermissions.OrderBy(a => a.UserType).ToList();
            SelectList user_types = new SelectList(user_levels, "Level_Id", "UserType", userDetails.db_user.UserLevel);
            UserInformation user_data = new UserInformation
            {
                Id = userDetails.db_user.Id,
                Username = userDetails.db_user.Username,
                Password = userDetails.db_user.Password,
                Fullname = userDetails.db_user.Fullname,
                Email = userDetails.db_user.Email,
                UserTypes = user_types,
            };
            return View(user_data);

        }
        [HttpPost]
        public ActionResult EditUser(UserInformation info, int id, int Level_Id)
        {
            if (ModelState.IsValid && info != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                var query = database.Users.Find(id);
                query.Username = info.Username;
                query.Password = info.Password;
                query.Fullname = info.Fullname;
                query.Email = info.Email;
                query.UserLevel = Level_Id;
                if (info.Username == null || info.Password == null || info.Password == null || info.Email == null || info.Fullname == null)
                {
                    return RedirectToAction("UsersData");
                }

                database.SaveChanges();
                string action = "Edited information for " + info.Fullname;
                activityLogging.logUserActivity(action);
                return RedirectToAction("UsersData");
            }
            return View();
        }
        /**
         * Delete a particular user form the system
         * */
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var userDetails = (from tb_users in database.Users
                               join table_permissions in database.UserPermissions on tb_users.UserLevel equals table_permissions.Level_Id
                               where tb_users.Id == id
                               select new { db_users = tb_users, db_permissions = table_permissions }).Single();

            UserInformation userData = new UserInformation
            {
                Id = userDetails.db_users.Id,
                Username = userDetails.db_users.Username,
                Password = userDetails.db_users.Password,
                Fullname = userDetails.db_users.Fullname,
                Email = userDetails.db_users.Email,
                UserLevel = userDetails.db_permissions.UserType
            };
            string action = "Deleted information for " + userDetails.db_users.Fullname;
            activityLogging.logUserActivity(action);
            return View(userData);
        }

        [HttpPost]
        public ActionResult DeleteUser(User user, int id)
        {
            if (ModelState.IsValid && user != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                user.Id = id;
                database.Users.Attach(user);
                database.Users.Remove(user);
                database.SaveChanges();
                return RedirectToAction("UsersData");
            }
            return View();
        }

        /**
         * Display all information for a particular user
         * */
        public ActionResult UserDetails(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();

            var user_details = (from tb_users in database.Users
                                join tb_permissions in database.UserPermissions on tb_users.UserLevel equals tb_permissions.Level_Id
                                where tb_users.Id == id
                                select new { db_users = tb_users, db_permissions = tb_permissions }).Single();


            UserInformation userData = new UserInformation
            {
                Id = user_details.db_users.Id,
                Username = user_details.db_users.Username,
                Password = user_details.db_users.Password,
                Fullname = user_details.db_users.Fullname,
                Email = user_details.db_users.Email,
                UserLevel = user_details.db_permissions.UserType
            };
            string action = "Viewed list of all users in the System";
            activityLogging.logUserActivity(action);
            return View(userData);
        }
        /**
         * Display information for a particular VSLA
         * */
        public ActionResult VslaDetails(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vsla_info = (from tb_vsla in database.Vslas
                             join tb_cbt in database.Cbt_info on tb_vsla.CBT equals tb_cbt.Id
                             join tb_regions in database.VslaRegions on tb_vsla.RegionId equals tb_regions.RegionId
                             join tb_status in database.StatusTypes on tb_vsla.Status equals tb_status.Status_Id
                             where tb_vsla.VslaId == id
                             select new { db_vsla = tb_vsla, db_cbt = tb_cbt, db_regions = tb_regions, db_status = tb_status }).Single();

            VslaInformation vslaData = new VslaInformation
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
                ContactPerson = vsla_info.db_vsla.ContactPerson ?? "--",
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber ?? "--",
                CBT = vsla_info.db_cbt.Name ?? "--",
                Status = vsla_info.db_status.CurrentStatus ?? "--"
            };
            string action = "Viewed all information for VSLA named " + vsla_info.db_vsla.VslaName;
            activityLogging.logUserActivity(action);
            return View(vslaData);
        }
        /**
         * Edit a given VSLA
         * */
        [HttpGet]
        public ActionResult EditVsla(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();

            var vsla_info = (from tb_vsla in database.Vslas
                             join tb_cbt in
                                 database.Cbt_info on tb_vsla.CBT equals tb_cbt.Id
                             where tb_vsla.VslaId == id
                             select new { db_vsla = tb_vsla, db_cbt = tb_cbt }).Single();

            // Get a list of all vsla regions to populate in the dropdown list
            List<VslaRegion> allVslaRegions = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList vslaRegionsList = new SelectList(allVslaRegions, "RegionId", "RegionName", vsla_info.db_vsla.RegionId);

            // Get the list of all cbts to populate in the dropdown list
            List<Cbt_info> AllCbtsList = database.Cbt_info.OrderBy(a => a.Name).ToList();
            SelectList cbtsList = new SelectList(AllCbtsList, "Id", "Name", vsla_info.db_vsla.CBT);

            // Get the status type ie active/inactive,
            List<StatusType> status = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList statusTypes = new SelectList(status, "Status_Id", "CurrentStatus", vsla_info.db_cbt.Status);

            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vsla_info.db_vsla.VslaId,
                VslaCode = vsla_info.db_vsla.VslaCode ?? "--",
                VslaName = vsla_info.db_vsla.VslaName ?? "--",
                VslaRegionsModel = vslaRegionsList,
                DateRegistered = vsla_info.db_vsla.DateRegistered.HasValue ? vsla_info.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla_info.db_vsla.DateLinked.HasValue ? vsla_info.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla_info.db_vsla.ContactPerson,
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber,
                CbtModel = cbtsList,
                StatusType = statusTypes
            };
            string action = "Edited information for VSLA named " + vsla_info.db_vsla.VslaName ?? "--";
            activityLogging.logUserActivity(action);
            return View(vslaData);
        }
        /**
         * Edit details for a particular VSLA
         * */
        [HttpPost]
        public ActionResult EditVsla(VslaInformation vsla, int VslaId, int Id, int RegionId, int Status_Id)
        {
            if (ModelState.IsValid && vsla != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
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
                if (vsla.VslaCode == null || vsla.VslaName == null || vsla.VslaPhoneMsisdn == null || vsla.GpsLocation == null || vsla.DateRegistered == null || vsla.DateLinked == null ||
                    vsla.ContactPerson == null || vsla.PositionInVsla == null || vsla.PhoneNumber == null)
                {
                    return RedirectToAction("VslaData");
                }
                database.SaveChanges();
                return RedirectToAction("VslaData");
            }
            return View();

        }
        /**
         * Add a new Village savings and lending association (VSLA) to the system
         * */
        public ActionResult AddVsla()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();

            // Get all cbts
            List<Cbt_info> AllCbtsList = database.Cbt_info.OrderBy(a => a.Name).ToList();
            SelectList cbtsList = new SelectList(AllCbtsList, "Id", "Name");

            // Get all vsla regions
            List<VslaRegion> allRegionsList = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regions_list = new SelectList(allRegionsList, "RegionId", "RegionName");

            // Get the status type ie active/inactive
            List<StatusType> status = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList statusList = new SelectList(status, "Status_Id", "CurrentStatus");

            VslaInformation vsla_list_options = new VslaInformation
            {
                VslaRegionsModel = regions_list,
                CbtModel = cbtsList,
                StatusType = statusList
            };
            return View(vsla_list_options);
        }
        [HttpPost]
        public ActionResult AddVsla(Vsla new_vsla, int RegionId, int Id, int Status_Id)
        {

            if (ModelState.IsValid && new_vsla != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                Vsla addedVsla = new Vsla
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
                if (addedVsla == null)
                {
                    RedirectToAction("AddVsla");
                }
                database.Vslas.Add(addedVsla);
                database.SaveChanges();
                string action = "Added new  VSLA named " + new_vsla.VslaName;
                activityLogging.logUserActivity(action);
                return RedirectToAction("VslaData");
            }
            //return View(new_vsla);
            return View();
        }
        /**
         * Delete a particular VSLA from the system
         * */
        [HttpGet]
        public ActionResult DeleteVsla(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vslaInformation = (from table_vsla in database.Vslas
                                   join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                                   join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                                   join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                                   where table_vsla.VslaId == id
                                   select new { db_vsla = table_vsla, db_cbt = table_cbt, db_regions = table_regions, db_status = table_status }).Single();

            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vslaInformation.db_vsla.VslaId,
                VslaCode = vslaInformation.db_vsla.VslaCode ?? "--",
                VslaName = vslaInformation.db_vsla.VslaName ?? "--",
                RegionId = vslaInformation.db_regions.RegionName,
                DateRegistered = vslaInformation.db_vsla.DateRegistered.HasValue ? vslaInformation.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vslaInformation.db_vsla.DateLinked.HasValue ? vslaInformation.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vslaInformation.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vslaInformation.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vslaInformation.db_vsla.GpsLocation ?? "--",
                ContactPerson = vslaInformation.db_vsla.ContactPerson,
                PositionInVsla = vslaInformation.db_vsla.PositionInVsla,
                PhoneNumber = vslaInformation.db_vsla.PhoneNumber,
                CBT = vslaInformation.db_cbt.Name ?? "--",
                Status = vslaInformation.db_status.CurrentStatus
            };
            string action = "Deleted all information for VSLA named " + vslaInformation.db_vsla.VslaName;
            activityLogging.logUserActivity(action);
            return View(vslaData);
        }
        [HttpPost]
        public ActionResult DeleteVsla(Vsla vsla, int id)
        {
            if (ModelState.IsValid && vsla != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                vsla.VslaId = id;
                database.Vslas.Attach(vsla);
                database.Vslas.Remove(vsla);
                database.SaveChanges();
                return RedirectToAction("VslaData");
            }
            return View();
        }
        /**
         * View all meetings attached to a particular VSLA
         * */
        public ActionResult VslaMeetings(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            AllVslaMeetingInformation totalMeetings = new AllVslaMeetingInformation();
            List<VslaMeetingInformation> singleMeeting = new List<VslaMeetingInformation>();
            // Get the vsla name
            var vslaName = database.Vslas.Find(id);
            // Get all meetings attached to a given vsla
            singleMeeting = getMeetingData(id);
            totalMeetings.allVslaMeetings = singleMeeting;
            totalMeetings.vslaName = vslaName.VslaName;
            totalMeetings.vslaId = id;
            string action = "Viewed information concerning vsla meetings";
            activityLogging.logUserActivity(action);
            return View(totalMeetings);
        }

        /**
         * Helper method to query the database all information for all meetings
         * */
        public List<VslaMeetingInformation> getMeetingData(int Id)
        {
            List<VslaMeetingInformation> allMeetings = new List<VslaMeetingInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            try
            {
                var meetings = (from db_meetings in database.Meetings
                                join db_cycles in database.VslaCycles on db_meetings.CycleId equals db_cycles.CycleId
                                join db_vsla in database.Vslas on db_cycles.VslaId equals db_vsla.VslaId
                                where db_vsla.VslaId == Id
                                select new { dt_meetings = db_meetings, dt_cycles = db_cycles, dt_vsla = db_vsla });
                foreach (var item in meetings)
                {
                    allMeetings.Add(new VslaMeetingInformation
                    {
                        MeetingId = item.dt_meetings.MeetingId,
                        cashFines = (long)item.dt_meetings.CashFines,
                        meetingDate = item.dt_meetings.MeetingDate,
                        membersPresent = int.Parse(item.dt_meetings.CountOfMembersPresent.ToString()),
                        totalSavings = (long)item.dt_meetings.SumOfSavings,
                        totalLoans = (long)item.dt_meetings.SumOfLoanIssues,
                        totalLoanRepayment = (long)item.dt_meetings.SumOfLoanRepayments
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return allMeetings;
        }
        /**
         * Export all VSLA meetings attached to a particular VSLA to CSV
         * */
        public void ExportVSLAMeetingsToCSV(int id, string fileName)
        {
            List<VslaMeetingInformation> allMeetings = new List<VslaMeetingInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            var meetings = (from db_meetings in database.Meetings
                            join db_cycles in database.VslaCycles on db_meetings.CycleId equals db_cycles.CycleId
                            join db_vsla in database.Vslas on db_cycles.VslaId equals db_vsla.VslaId
                            where db_vsla.VslaId == id
                            select new { dt_meetings = db_meetings, dt_cycles = db_cycles, dt_vsla = db_vsla });
            foreach (var item in meetings)
            {
                allMeetings.Add(new VslaMeetingInformation
                {
                    MeetingId = item.dt_meetings.MeetingId,
                    cashFines = (long)item.dt_meetings.CashFines,
                    meetingDate = item.dt_meetings.MeetingDate,
                    membersPresent = int.Parse(item.dt_meetings.CountOfMembersPresent.ToString()),
                    totalSavings = (long)item.dt_meetings.SumOfSavings,
                    totalLoans = (long)item.dt_meetings.SumOfLoanIssues,
                    totalLoanRepayment = (long)item.dt_meetings.SumOfLoanRepayments
                });
            }
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Meeting Date\",\"Members Present\",\"Fines\",\"Amount Saved\",\"Total Loans\",\"Loan Outstanding\"");

            Response.ClearContent();

            String attachment = "attachment;filename=" + fileName.Replace(" ", "_") + ".csv";

            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/csv";
            foreach (var line in allMeetings)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                           line.meetingDate.Value.ToShortDateString(),
                                           line.membersPresent,
                                           line.cashFines,
                                           line.totalSavings,
                                           line.totalLoans,
                                           line.totalLoanRepayment
                                           ));
            }

            Response.Write(sw.ToString());

            Response.End();


        }
        /**
         * Get details for a particular meeting held on a partuclar day by a VSLA
         * */
        public ActionResult SingleMeetingDetails(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            AllSingleMeetingProcedures allInformation = new AllSingleMeetingProcedures();
            List<SingleMeetingProcedures> meetingsList = new List<SingleMeetingProcedures>();

            // Get the date when the meeting was held
            var meetingDate = database.Meetings.Find(id);
            // Get the all the meeting details
            meetingsList = MeetingDetails(id);
            allInformation.allMeetingData = meetingsList;
            allInformation.meetingDate = meetingDate.MeetingDate;
            allInformation.vslaId = id;
            return View(allInformation);
        }
        /**
         * Helper class for getting information concerned with all meetings in the whole system
         * */
        public List<SingleMeetingProcedures> MeetingDetails(int id)
        {
            List<SingleMeetingProcedures> meetings = new List<SingleMeetingProcedures>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var meeting = (from db_attendance in db.Attendances
                           join db_member in db.Members on db_attendance.MemberId equals db_member.MemberId
                           join db_savings in db.Savings on db_attendance.MemberId equals db_savings.MemberId
                           join db_loan in db.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
                           join db_fines in db.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
                           join db_loanRepayment in db.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
                           where (db_attendance.MeetingId == id && db_savings.MeetingId == id)
                           from db_loansAttendance in joinedLoansAttendance.DefaultIfEmpty()
                           from db_finesAttendance in joinedFinesAttendance.DefaultIfEmpty()
                           from db_repaymentAttendance in joinedRepaymentAttendance.DefaultIfEmpty()
                           select new
                           {
                               db_attendance,
                               db_member,
                               db_savings,
                               loanNo = (db_loansAttendance.LoanId == null) ? 00 : db_loansAttendance.LoanNo,
                               loanAmount = (db_loansAttendance.PrincipalAmount == null) ? (decimal)0.00 : db_loansAttendance.PrincipalAmount,
                               amountInFines = (db_finesAttendance.Amount == null) ? (decimal)0.00 : db_finesAttendance.Amount,
                               loanRepaymentAmount = (db_repaymentAttendance.Amount == null) ? (decimal)0.00 : db_repaymentAttendance.Amount,
                               remainingBalanceOnLoan = (db_repaymentAttendance.BalanceAfter == null) ? (decimal)0.00 : db_repaymentAttendance.BalanceAfter


                           });
            foreach (var item in meeting)
            {
                meetings.Add(new SingleMeetingProcedures
                {
                    Id = item.db_attendance.AttendanceId,
                    memberId = item.db_member.MemberId,
                    memberName = item.db_member.Surname + " " + item.db_member.OtherNames,
                    isPresent = item.db_attendance.IsPresent.ToString(),
                    amountSaved = (long)item.db_savings.Amount,
                    loanNumber = (int)item.loanNo,
                    principleAmount = (long)item.loanAmount,
                    finedAmount = (long)item.amountInFines,
                    loanRepaymentAmount = (long)item.loanRepaymentAmount,
                    remainingBalanceOnLoan = (long)item.remainingBalanceOnLoan

                });
            }
            string action = "Viewed information for a single VSLA meeting";
            activityLogging.logUserActivity(action);
            return meetings;
        }
        /**
         * Export details of a single meeting to a csv file
         * */
        public void ExportSingleMeetingDetailsCSV(int id)
        {
            List<SingleMeetingProcedures> meetings = new List<SingleMeetingProcedures>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var meeting = (from db_attendance in db.Attendances
                           join db_member in db.Members on db_attendance.MemberId equals db_member.MemberId
                           join db_savings in db.Savings on db_attendance.MemberId equals db_savings.MemberId
                           join db_loan in db.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
                           join db_fines in db.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
                           join db_loanRepayment in db.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
                           where (db_attendance.MeetingId == id && db_savings.MeetingId == id)
                           from db_loansAttendance in joinedLoansAttendance.DefaultIfEmpty()
                           from db_finesAttendance in joinedFinesAttendance.DefaultIfEmpty()
                           from db_repaymentAttendance in joinedRepaymentAttendance.DefaultIfEmpty()
                           select new
                           {
                               db_attendance,
                               db_member,
                               db_savings,
                               loanNo = (db_loansAttendance.LoanId == null) ? 00 : db_loansAttendance.LoanNo,
                               loanAmount = (db_loansAttendance.PrincipalAmount == null) ? (decimal)0.00 : db_loansAttendance.PrincipalAmount,
                               amountInFines = (db_finesAttendance.Amount == null) ? (decimal)0.00 : db_finesAttendance.Amount,
                               loanRepaymentAmount = (db_repaymentAttendance.Amount == null) ? (decimal)0.00 : db_repaymentAttendance.Amount,
                               remainingBalanceOnLoan = (db_repaymentAttendance.BalanceAfter == null) ? (decimal)0.00 : db_repaymentAttendance.BalanceAfter


                           });
            foreach (var item in meeting)
            {
                meetings.Add(new SingleMeetingProcedures
                {
                    Id = item.db_attendance.AttendanceId,
                    memberId = item.db_member.MemberId,
                    memberName = item.db_member.Surname + " " + item.db_member.OtherNames,
                    isPresent = item.db_attendance.IsPresent.ToString(),
                    amountSaved = (long)item.db_savings.Amount,
                    loanNumber = (int)item.loanNo,
                    principleAmount = (long)item.loanAmount,
                    finedAmount = (long)item.amountInFines,
                    loanRepaymentAmount = (long)item.loanRepaymentAmount,
                    remainingBalanceOnLoan = (long)item.remainingBalanceOnLoan

                });
            }
            StringWriter sw = new StringWriter();
            sw.WriteLine("\"Member Name\",\"Attendance\",\"Amount Saved\",\"Loan Number\",\"Loan Taken\",\"Fines\",\"Loan Cleared\",\"Loan Outstanding\"");
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=meeting_details.csv");
            Response.ContentType = "text/csv";


            foreach (var line in meetings)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
                                           line.memberName,
                                           line.isPresent,
                                           line.amountSaved,
                                           line.loanNumber,
                                           line.principleAmount,
                                           line.finedAmount,
                                           line.loanRepaymentAmount,
                                           line.remainingBalanceOnLoan
                                           ));
            }

            Response.Write(sw.ToString());

            Response.End();


        } // EOF

        /**
         *  View all members attached to a given vsla
         * */
        public ActionResult VslaMembers(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            AllVslaMemberInformation memberData = new AllVslaMemberInformation();
            List<VslaMembersInformation> membersList = new List<VslaMembersInformation>();
            // Get the name of vsla
            var vslaName = database.Vslas.Find(id);
            // Get the list of all members
            membersList = getMembersData(id);
            memberData.allVslaMembers = membersList;
            memberData.vslaName = vslaName.VslaName;
            memberData.vslaId = id; // passing the id of the vsla on whch members are attached
            return View(memberData);
        }
        /**
         * Helper method to query the database and get a list of all members attached to a 
         * particular vsla
         * */
        public List<VslaMembersInformation> getMembersData(int id)
        {
            List<VslaMembersInformation> allMembers = new List<VslaMembersInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            var members = (from db_members in database.Members where db_members.VslaId == id select new { dt_members = db_members });
            foreach (var item in members)
            {
                allMembers.Add(new VslaMembersInformation
                {
                    memberId = item.dt_members.MemberId,
                    memberNumber = int.Parse(item.dt_members.MemberNo.ToString()),
                    cyclesCompleted = int.Parse(item.dt_members.CyclesCompleted.ToString()),
                    surname = item.dt_members.Surname,
                    otherNames = item.dt_members.OtherNames,
                    gender = item.dt_members.Gender,
                    occupation = item.dt_members.Occupation,
                });
            }
            string action = "Viewed information for all members for a selected vsla";
            activityLogging.logUserActivity(action);
            return allMembers;
        }
        /**
         * Export the list of members to a csv file
         * */
        public void ExportMembersToCSV(int id, string fileName)
        {
            List<VslaMembersInformation> allMembers = new List<VslaMembersInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            var members = (from db_members in database.Members where db_members.VslaId == id select new { dt_members = db_members });
            foreach (var item in members)
            {
                allMembers.Add(new VslaMembersInformation
                {
                    memberId = item.dt_members.MemberId,
                    memberNumber = int.Parse(item.dt_members.MemberNo.ToString()),
                    cyclesCompleted = int.Parse(item.dt_members.CyclesCompleted.ToString()),
                    surname = item.dt_members.Surname,
                    otherNames = item.dt_members.OtherNames,
                    gender = item.dt_members.Gender,
                    occupation = item.dt_members.Occupation,
                });
            }
            StringWriter sw = new StringWriter();

            sw.WriteLine("\"Member Number\",\"Cycles Completed\",\"Surname\",\"Other Names\",\"Gender\",\"Occupation\"");

            Response.ClearContent();

            String attachment = "attachment;filename=" + fileName.Replace(" ", "_") + ".csv";
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/csv";

            foreach (var line in allMembers)
            {
                sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\"",
                                           line.memberNumber,
                                           line.cyclesCompleted,
                                           line.surname,
                                           line.otherNames,
                                           line.gender,
                                           line.occupation
                                           ));
            }

            Response.Write(sw.ToString());

            Response.End();
        }
        /**
         * Get all information for a given user attached to a particular vsla
         * */
        public ActionResult MemberDetails(int id)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var member = (from db_members in database.Members where db_members.MemberId == id select new { dt_members = db_members }).Single();
            VslaMembersInformation memberInfo = new VslaMembersInformation
            {
                memberNumber = int.Parse(member.dt_members.MemberNo.ToString()),
                cyclesCompleted = int.Parse(member.dt_members.CyclesCompleted.ToString()),
                surname = member.dt_members.Surname,
                otherNames = member.dt_members.OtherNames,
                gender = member.dt_members.Gender,
                occupation = member.dt_members.Occupation,
                dateArchived = member.dt_members.DateArchived,
                DateOfBirth = member.dt_members.DateOfBirth,
                isActive = member.dt_members.IsActive.ToString(),
                isArchive = member.dt_members.IsArchived.ToString(),
                phoneNumber = member.dt_members.PhoneNo

            };
            string action = "Viewed information for vsla member called " + member.dt_members.Surname + " " + member.dt_members.OtherNames;
            activityLogging.logUserActivity(action);
            return View(memberInfo);
        }
        /**
         * Add a new community based trainer (CBT) to the system
         * */
        public ActionResult AddCbt()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            // Regions
            List<VslaRegion> allRegionsList = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName");
            // Status types
            List<StatusType> status = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList statusTypes = new SelectList(status, "Status_Id", "CurrentStatus");
            CbtInformation regionsSelector = new CbtInformation
            {
                VslaRegionsModel = regionsList,
                StatusType = statusTypes
            };
            return View(regionsSelector);
        }
        [HttpPost]
        public ActionResult AddCbt(Cbt_info new_cbt, int RegionId, int Status_Id)
        {
            if (ModelState.IsValid && new_cbt != null)
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
                if (cbx == null)
                {
                    RedirectToAction("AddCbt");
                }
                string action = "Added a new CBT called " + new_cbt.Name;
                activityLogging.logUserActivity(action);

                database.Cbt_info.Add(cbx);
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
            //return View(new_cbt);
            return View();
        }

        /**
         * View all information for a particular CBT
         * */
        public ActionResult CbtDetails(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var allInformation = (from table_cbt in db.Cbt_info
                                  join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  join table_status in db.StatusTypes on table_cbt.Status equals table_status.Status_Id
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status }).Single();
            CbtInformation cbtData = new CbtInformation
            {
                Id = allInformation.dt_cbt.Id,
                Name = allInformation.dt_cbt.Name,
                Region = allInformation.dt_region.RegionName,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                Status = allInformation.dt_status.CurrentStatus
            };
            string action = "Viewed CBT details for  " + allInformation.dt_cbt.Name;
            activityLogging.logUserActivity(action);
            return View(cbtData);
        }
        /**
         * Edit information for a particular CBT
         * */
        [HttpGet]
        public ActionResult EditCbt(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();

            var allInformation = (from table_cbt in db.Cbt_info
                                  join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, db_region = table_region }).Single();

            // all reegions
            List<VslaRegion> allRegions = db.VslaRegions.OrderBy(a => a.RegionName).ToList();
            SelectList regionsList = new SelectList(allRegions, "RegionId", "RegionName", allInformation.db_region.RegionId);

            // Get the status ie active/inactive
            List<StatusType> status = db.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            SelectList statusList = new SelectList(status, "Status_Id", "CurrentStatus", allInformation.dt_cbt.Status);

            // Create a cbt object
            CbtInformation cbtData = new CbtInformation
            {
                Id = allInformation.dt_cbt.Id,
                Name = allInformation.dt_cbt.Name,
                VslaRegionsModel = regionsList,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                StatusType = statusList
            };

            string action = "Edited CBT information for  " + allInformation.dt_cbt.Name;
            activityLogging.logUserActivity(action);
            return View(cbtData);
        }
        [HttpPost]
        public ActionResult EditCbt(Cbt_info cbt, int id, int RegionId, int Status_Id)
        {
            if (ModelState.IsValid && cbt != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                var query = database.Cbt_info.Find(id);
                query.Name = cbt.Name;
                query.Region = RegionId;
                query.PhoneNumber = cbt.PhoneNumber;
                query.Email = cbt.Email;
                query.Status = Status_Id;

                if (cbt.Name == null || cbt.PhoneNumber == null || cbt.Email == null)
                {
                    return RedirectToAction("CbtData");
                }
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
            return View();
        }
        /**
         * Delete a particular CBT from the system
         * */
        [HttpGet]
        public ActionResult DeleteCbt(int id)
        {
            ledgerlinkEntities db = new ledgerlinkEntities();
            var allInformation = (from table_cbt in db.Cbt_info
                                  join table_region in db.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  join table_status in db.StatusTypes on table_cbt.Status equals table_status.Status_Id
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status }).Single();
            CbtInformation cbtData = new CbtInformation
            {
                Id = allInformation.dt_cbt.Id,
                Name = allInformation.dt_cbt.Name,
                Region = allInformation.dt_region.RegionName,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                Status = allInformation.dt_status.CurrentStatus
            };
            string action = "Deleted CBT information for  " + allInformation.dt_cbt.Name;
            activityLogging.logUserActivity(action);
            return View(cbtData);
        }
        [HttpPost]
        public ActionResult DeleteCbt(Cbt_info cbt, int id)
        {
            if (ModelState.IsValid && cbt != null)
            {
                ledgerlinkEntities database = new ledgerlinkEntities();
                cbt.Id = id;
                database.Cbt_info.Attach(cbt);
                database.Cbt_info.Remove(cbt);
                database.SaveChanges();
                return RedirectToAction("CbtData");
            }
            return View();
        }

        /**
         * Helper method to get information for all registered users
         * */
        public List<UserInformation> usersInformation()
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

        /**
         * Helper method to get all information concerning vsla
         * */
        public List<VslaInformation> getVslaInformation()
        {
            List<VslaInformation> vslaList = new List<VslaInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var vslaDetails = (from data in db.Vslas select data);
            foreach (var item in vslaDetails)
            {
                vslaList.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.RegionId.ToString(),
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
            return vslaList;
        }
        /**
        * Export all VSLA data to a csv file
        * 
        * */
        public void ExportVSLAsToCSV()
        {
            List<VslaInformation> vslaList = new List<VslaInformation>();
            ledgerlinkEntities db = new ledgerlinkEntities();
            var vslaDetails = (from data in db.Vslas select data);
            foreach (var item in vslaDetails)
            {
                vslaList.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.RegionId.ToString(),
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
            StringWriter stringWriter = new StringWriter();
            stringWriter.WriteLine("\"VSLA Code\",\"VSLA Name\",\"Region\",\"Date Registered\",\"Date Linked\",\"Physical Address\",\"Phone MSISDN\",\"Contact Person\",\"Position in VSLA\",\"Phone Number\",\"CBT\",\"Status\"");

            Response.ClearContent();
            String attachment = "attachment;filename=all_VSLAs.csv";
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/csv";
            foreach (var line in vslaList)
            {

                stringWriter.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"",
                                           line.VslaCode,
                                           line.VslaName,
                                           line.RegionId,
                                           line.DateRegistered.Value.ToShortDateString(),
                                           line.DateLinked.Value.ToShortDateString(),
                                           line.PhysicalAddress,
                                           line.VslaPhoneMsisdn,
                                           line.ContactPerson,
                                           line.PositionInVsla,
                                           line.PhoneNumber,
                                           line.CBT,
                                           line.Status.Equals("1") ? "Active" : "Inactive"
                                           ));

            }
            Response.Write(stringWriter.ToString());

            Response.End();


        }

        /**
         * Helper method to get the list of all CBTS that have been added to a system
         * */
        public List<CbtInformation> getCbtInformation()
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

        /**
         * Get all the log information
         * */
        public List<LogsInformation> getAllLogInformation()
        {
            List<LogsInformation> logs = new List<LogsInformation>();
            ledgerlinkEntities database = new ledgerlinkEntities();
            var logsInformation = (from database_logs in database.Audit_Log
                                   join database_users in database.Users on
                                       database_logs.UserId equals database_users.Id
                                   select new { db_logs = database_logs, db_users = database_users });

            foreach (var data in logsInformation)
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

    }
}
