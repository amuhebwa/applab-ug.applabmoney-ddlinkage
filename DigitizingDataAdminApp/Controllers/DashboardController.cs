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
using System.Text.RegularExpressions;
namespace DigitizingDataAdminApp.Controllers
{
    public class DashboardController : Controller
    {
        ActivityLoggingSystem activityLoggingSystem;
        ledgerlinkEntities database;
        public DashboardController()
        {
            activityLoggingSystem = new ActivityLoggingSystem();
            database = new ledgerlinkEntities();

        }

        [Authorize]
        public ActionResult Dashboard()
        {
            if (Session["UserId"] != null)
            {

                // Total VSLAs
                long totalVslas = database.Vslas.Select(x => x.VslaName).Count();
                // Total Members
                int femaleMMembers = (from db_members in database.Members
                                      where db_members.Gender == "Female"
                                      select new { db_members }).Count();
                int maleMembers = (from db_members in database.Members
                                   where db_members.Gender == "Male"
                                   select new { db_members }).Count();
                int totalMembers = (int)database.Members.Select(x => x.MemberId).Count();
                // Savings, Loans and repayments
                double totalSavings = (double)database.Meetings.Select(x => x.SumOfSavings).Sum();
                double totalLoans = (double)database.Meetings.Select(x => x.SumOfLoanIssues).Sum();
                double totalLoanRepayment = (double)database.Meetings.Select(x => x.SumOfLoanRepayments).Sum();
                // Attendance
                int totalPresent = (from db_present in database.Attendances
                                    where db_present.IsPresent == true
                                    select new { db_present }).Count();
                int totalAbsent = (from db_present in database.Attendances
                                   where db_present.IsPresent == false
                                   select new { db_present }).Count();
                // Total number of submissions
                int totalSubmissions = (int)database.DataSubmissions.Select(x => x.SourceVslaCode).Count();
                // Total meetings
                int totalMeeetings = (int)database.Meetings.Select(x => x.MeetingId).Count();
                DashboardData summary = new DashboardData
                {
                    femaleMembers = femaleMMembers,
                    maleMembers = maleMembers,
                    totalMembers = totalMembers,
                    totalAbsent = totalAbsent,
                    totalPresent = totalPresent,
                    totalLoanRepayment = totalLoanRepayment,
                    totalLoans = totalLoans,
                    totalSavings = totalSavings,
                    totalSubmissions = totalSubmissions,
                    totalMeeetings = totalMeeetings,
                    totalVslas = totalVslas


                };
                return View(summary);
            }
            else
            {
                return RedirectToAction("Index", "Index");
            }
        }
        /**
         * Generate Graphs for Summary Data Visualization
         * */
        // 1. Members by Gender
        public ActionResult showMembersByGender()
        {
            int femaleMMembers = (from db_members in database.Members
                                  where db_members.Gender == "Female"
                                  select new { db_members }).Count();
            int maleMembers = (from db_members in database.Members
                               where db_members.Gender == "Male"
                               select new { db_members }).Count();
            new Chart(width: 300, height: 300)
            .AddTitle("Members By Gender")
            .AddSeries(chartType: "pie",
                xValue: new[] { "Males", "Females" },
                yValues: new[] { maleMembers.ToString(), femaleMMembers.ToString() }
                ).AddLegend().Write("bmp");
            return null;
        }
        // 2. Attendance (Absent/Present)
        public ActionResult showAttendance()
        {
            long totalPresent = (from db_present in database.Attendances
                                 where db_present.IsPresent == true
                                 select new { db_present }).Count();
            long totalAbsent = (from db_present in database.Attendances
                                where db_present.IsPresent == false
                                select new { db_present }).Count();
            new Chart(width: 300, height: 300)
            .AddTitle("Overall Attendance")
            .AddSeries(chartType: "pie",
                xValue: new[] { "Present", "Absent" },
               yValues: new[] { totalPresent.ToString(), totalAbsent.ToString() }
               ).AddLegend().Write("bmp");
            return null;
        }
        // 3. Show total savings, loans given out and loan repayments
        public ActionResult showSavingsLoansAndRepayments()
        {
            double totalSavings = (double)database.Meetings.Select(x => x.SumOfSavings).Sum();
            double totalLoans = (double)database.Meetings.Select(x => x.SumOfLoanIssues).Sum();
            double totalLoanRepayment = (double)database.Meetings.Select(x => x.SumOfLoanRepayments).Sum();

            new Chart(width: 300, height: 300)
            .AddTitle("Financial Break down")
            .AddSeries(chartType: "column",
            xValue: new[] { "Total Savings", "Total Loans", "Loan Repayment" },
            yValues: new[] { totalSavings.ToString(), totalLoans.ToString(), totalLoanRepayment.ToString() })
            .Write("bmp");

            return null;
        }
        /**
         * Get all information concerning registered information
         * */
        public ActionResult SystemUsers()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            SystemUsersInformation allUsers = new SystemUsersInformation();
            List<UserInformation> singleUser = new List<UserInformation>();
            singleUser = usersInformation();
            allUsers.AllUsersList = singleUser;
            allUsers.SessionUserLevel = sessionUserLevel;
            allUsers.AccessLevel = getAccessPermissions();
            return View(allUsers);
        }
        /**
         * Get the user level permissions to populate in the drop down list 
         **/
        public SelectList getAccessPermissions()
        {
            List<UserPermission> permissions = new List<UserPermission>();
            permissions.Add(new UserPermission { Level_Id = 0, UserType = "- Select Access Level -" });
            var databasePermissions = database.UserPermissions.OrderBy(a => a.Level_Id);
            foreach (var permission in databasePermissions)
            {
                permissions.Add(new UserPermission
                {
                    Level_Id = permission.Level_Id,
                    UserType = permission.UserType
                });
            }
            SelectList acccessPermissions = new SelectList(permissions, "Level_Id", "UserType", 0);
            return acccessPermissions;
        }
        /**
         * Get all information concerning VSLA Groups
         * */
        public ActionResult VslaGroupInformation()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]); // Get the user level of the current session
            VslaGroupsInformation allGroups = new VslaGroupsInformation();
            List<VslaInformation> getVslaData = new List<VslaInformation>();
            getVslaData = getVslaInformation();
            allGroups.AllGroupsList = getVslaData;
            allGroups.sessionUserLevel = sessionUserLevel;
            allGroups.AllTechnicalTrainers = getAllTrainers();
            allGroups.VslaRegions = getVslaRegions();
            allGroups.StatusType = getStatusTypes();
            allGroups.groupSupportProvided = getSupportType();


            return View(allGroups);
        }
        // Get the group support modules that have been provided to the group by technical trainers
        public List<GroupSupportInfo> getSupportType()
        {
            var support = (from tb_support in database.GroupSupports
                           join tb_vsla in database.Vslas
                               on tb_support.VslaId equals tb_vsla.VslaId
                           join tb_trainers in database.TechnicalTrainers on
                               tb_support.TrainerId equals tb_trainers.Id
                           select new { tb_support, tb_vsla, tb_trainers });
            List<GroupSupportInfo> supportGiven = new List<GroupSupportInfo>();
            foreach (var sp in support)
            {
                supportGiven.Add(new GroupSupportInfo()
                {
                    GroupName = sp.tb_vsla.VslaName,
                    TrainerName = sp.tb_trainers.Name,
                    SupportType = sp.tb_support.SupportType,
                    supportDate = sp.tb_support.SupportDate
                });
            }
            return supportGiven;

        }


        /**
         * Get all information concerned with CTechnical Trainers
         * */
        public ActionResult TechnicalTrainers()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]); // Get the user level of the current session
            AllTrainersInformation allTrainers = new AllTrainersInformation();
            List<TrainerInformation> getTrainersData = new List<TrainerInformation>();
            getTrainersData = getCbtInformation();
            allTrainers.AllTrainersList = getTrainersData;
            allTrainers.SessionUserLevel = sessionUserLevel;
            allTrainers.VslaRegionsModel = getVslaRegions();
            allTrainers.StatusType = getStatusTypes();

            return View(allTrainers);
        }

        // Get the list of regions
        public SelectList getVslaRegions()
        {
            List<VslaRegion> allRegionsList = new List<VslaRegion>();
            allRegionsList.Add(new VslaRegion() { RegionId = 0, RegionName = "-Select Region-" });
            var databaseRegions = database.VslaRegions.OrderBy(a => a.RegionName);
            foreach (var region in databaseRegions)
            {
                allRegionsList.Add(new VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName", 0);
            return regionsList;
        }


        // List of status types ie active/inactive
        public SelectList getStatusTypes()
        {
            List<StatusType> statusOptions = new List<StatusType>();
            statusOptions.Add(new StatusType() { Status_Id = 0, CurrentStatus = "-Select Status-" });
            var databaseStatuses = database.StatusTypes.OrderBy(a => a.Status_Id);
            foreach (var status in databaseStatuses)
            {
                statusOptions.Add(new StatusType
                {
                    Status_Id = status.Status_Id,
                    CurrentStatus = status.CurrentStatus
                });
            }
            SelectList statusTypes = new SelectList(statusOptions, "Status_Id", "CurrentStatus", 0);
            return statusTypes;

        }
        // list of all Technical trainers
        public SelectList getAllTrainers()
        {
            List<TechnicalTrainer> trainers = new List<TechnicalTrainer>();
            trainers.Add(new TechnicalTrainer { Id = 0, Name = "-Select Trainer" });
            var database_trainers = database.TechnicalTrainers.OrderBy(a => a.Name);
            foreach (var trainer in database_trainers)
            {
                trainers.Add(new TechnicalTrainer
                {
                    Id = trainer.Id,
                    Name = trainer.Name
                });
            }
            SelectList allTrainers = new SelectList(trainers, "Id", "Name", 0);
            return allTrainers;

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
            String logString = Convert.ToString(Session["Username"]) + " Logged out";
            activityLoggingSystem.logActivity(logString, 0);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult AddUser(User user, int Level_Id)
        {
            PasswordHashing _password = new PasswordHashing();
            string _hashedPassword = _password.hashedPassword(user.Password.Trim()); // Hash the password
            if (Level_Id == 0)
            {
                ModelState.AddModelError("AccessLevel", "Please select Access Level");
                return Redirect(Url.Action("SystemUsers") + "#addusertab");
            }
            else
            { // All fields have been validated
                User _user = new User
                {
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Password = _hashedPassword,
                    Email = user.Email,
                    DateCreated = System.DateTime.Today,
                    UserLevel = Level_Id
                };
                database.Users.Add(_user);
                database.SaveChanges();
                ModelState.Clear();
                user = null;
                String logString = Convert.ToString(Session["Username"]) + " Added a new User";
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("SystemUsers");
            }
        }

        /**
         * Edit a particular user's information.
         * */
        [HttpGet]
        public ActionResult EditUser(int id)
        {
            var userDetails = (from table_users in database.Users
                               join table_permissions in database.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                               where table_users.Id == id
                               select new { db_user = table_users, db_permissions = table_permissions }).Single();

            // Get access levels
            List<UserPermission> permissions = new List<UserPermission>();
            var databasePermissions = database.UserPermissions.OrderBy(a => a.Level_Id);
            foreach (var permission in databasePermissions)
            {
                permissions.Add(new UserPermission
                {
                    Level_Id = permission.Level_Id,
                    UserType = permission.UserType
                });
            }
            SelectList acccessPermissions = new SelectList(permissions, "Level_Id", "UserType", userDetails.db_permissions.Level_Id);
            UserInformation user_data = new UserInformation
            {
                Id = userDetails.db_user.Id,
                Username = userDetails.db_user.Username,
                // Password = userDetails.db_user.Password,
                Fullname = userDetails.db_user.Fullname,
                Email = userDetails.db_user.Email,
                DateCreated = userDetails.db_user.DateCreated,
                AccessLevel = acccessPermissions,
            };
            return View(user_data);

        }
        [HttpPost]
        public ActionResult EditUser(UserInformation user, int id, int Level_Id)
        {
            PasswordHashing passwordHashing = new PasswordHashing();
            if (string.IsNullOrEmpty(user.Fullname))
            {
                ModelState.AddModelError("Fullname", "Fullname cannot be empty");
            }
            else if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Email cannot be empty");
            }
            else if (Level_Id == 0)
            {
                ModelState.AddModelError("AccessLevel", "Please select Access Level");
            }
            else
            {
                string hashedPassword = passwordHashing.hashedPassword(user.Password);
                int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
                var query = database.Users.Find(id);

                if (user.Password != null) { query.Password = hashedPassword; }
                query.UserLevel = sessionUserLevel == 1 ? Level_Id : 2; // If the user level == 2, don't allow them to change it
                query.Username = user.Username;
                query.Fullname = user.Fullname;
                query.Email = user.Email;
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Edited User with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("SystemUsers");
            }
            List<UserPermission> permissions = new List<UserPermission>();
            var databasePermissions = database.UserPermissions.OrderBy(a => a.Level_Id);
            foreach (var permission in databasePermissions)
            {
                permissions.Add(new UserPermission
                {
                    Level_Id = permission.Level_Id,
                    UserType = permission.UserType
                });
            }
            SelectList acccessPermissions = new SelectList(permissions, "Level_Id", "UserType", Level_Id);
            UserInformation user_data = new UserInformation
            {
                Id = id,
                Username = user.Username,
                Password = user.Password,
                Fullname = user.Fullname,
                Email = user.Email,
                AccessLevel = acccessPermissions,
            };
            return View(user_data);
        }
        /**
         * Delete a particular user form the system
         * */
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
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
                DateCreated = userDetails.db_users.DateCreated,
                UserLevel = userDetails.db_permissions.UserType
            };
            return View(userData);
        }

        [HttpPost]
        public ActionResult DeleteUser(User user, int id)
        {
            if (ModelState.IsValid && user != null)
            {
                user.Id = id;
                database.Users.Attach(user);
                database.Users.Remove(user);
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Deleted User with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("SystemUsers");
            }
            String logStringError = Convert.ToString(Session["Username"]) + "Failed to delete User with ID : " + Convert.ToString(id);
            activityLoggingSystem.logActivity(logStringError, 1);
            return View();
        }

        /**
         * Display all information for a particular user
         * */
        public ActionResult UserDetails(int id)
        {
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
                DateCreated = user_details.db_users.DateCreated,
                UserLevel = user_details.db_permissions.UserType
            };
            return View(userData);
        }
        /**
         * Display information for a particular VSLA
         * */
        public ActionResult VslaGroupDetails(int id)
        {
            var vsla_info = (from tb_vsla in database.Vslas
                             join tb_cbt in database.TechnicalTrainers on tb_vsla.CBT equals tb_cbt.Id
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
                DateRegistered = vsla_info.db_vsla.DateRegistered,
                DateLinked = vsla_info.db_vsla.DateLinked,
                PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla_info.db_vsla.ContactPerson ?? "--",
                PositionInVsla = vsla_info.db_vsla.PositionInVsla,
                PhoneNumber = vsla_info.db_vsla.PhoneNumber ?? "--",
                TechnicalTrainer = vsla_info.db_cbt.Name ?? "--",
                Status = vsla_info.db_status.CurrentStatus ?? "--",
                GroupAccountNumber = "A/C " + vsla_info.db_vsla.GroupAccountNumber ?? "--"
            };
            return View(vslaData);
        }
        /**
         * Edit a given VSLA
         * */
        [HttpGet]
        public ActionResult EditVslaGroup(int id)
        {
            VslaInformation vslaData = getGroupEditInformation(id);
            return View(vslaData);
        }
        /**
         * Edit details for a particular VSLA
         * */
        [HttpPost]
        public ActionResult EditVslaGroup(VslaInformation vslaGroup, int VslaId, int Id, int RegionId, int Status_Id)
        {
            if (string.IsNullOrEmpty(vslaGroup.VslaName))
            {
                ModelState.AddModelError("VslaName", "Please add a valid VSLA Name");
            }
            else if (RegionId == 0)
            {
                ModelState.AddModelError("RegionName", "Please select a region");
            }
            else if (string.IsNullOrEmpty(vslaGroup.DateRegistered.ToString()))
            {
                ModelState.AddModelError("DateRegistered", "Please Enter Valid Date Registered");
            }
            else if (string.IsNullOrEmpty(vslaGroup.DateLinked.ToString()))
            {
                ModelState.AddModelError("DateLinked", "ADate Linked cannot be null");
            }
            else if (string.IsNullOrEmpty(vslaGroup.PhysicalAddress))
            {
                ModelState.AddModelError("PhysicalAddress", " Please add a physical address");
            }
            else if (string.IsNullOrEmpty(vslaGroup.VslaPhoneMsisdn))
            {
                ModelState.AddModelError("VslaPhoneMsisdn", "Phone MSISDN cannot be empty");
            }
            else if (string.IsNullOrEmpty(vslaGroup.GpsLocation))
            {
                ModelState.AddModelError("GpsLocation", "Your GPS Location cannot be empty");
            }
            else if (string.IsNullOrEmpty(vslaGroup.ContactPerson))
            {
                ModelState.AddModelError("ContactPerson", "Please add a valid contact person");
            }
            else if (string.IsNullOrEmpty(vslaGroup.PositionInVsla))
            {
                ModelState.AddModelError("PositionInVsla", "Position cannot be left Empty");
            }
            else if (string.IsNullOrEmpty(vslaGroup.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Contact Person's Number is Empty");
            }
            else if (Id == 0)
            {
                ModelState.AddModelError("CbtModel", "Select Responsible CBT");
            }
            else if (Status_Id == 0)
            {
                ModelState.AddModelError("StatusType", "Select Status Type");
            }
            else if (string.IsNullOrEmpty(vslaGroup.GroupAccountNumber))
            {
                ModelState.AddModelError("GroupAccountNumber", "Add Group Account Number");
            }
            else
            {
                var query = database.Vslas.Find(VslaId);
                query.VslaName = vslaGroup.VslaName;
                query.VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn;
                query.GpsLocation = vslaGroup.GpsLocation;
                query.DateRegistered = vslaGroup.DateRegistered;
                query.DateLinked = vslaGroup.DateLinked;
                query.RegionId = (int)RegionId;
                query.ContactPerson = vslaGroup.ContactPerson;
                query.PositionInVsla = vslaGroup.PositionInVsla;
                query.PhoneNumber = vslaGroup.PhoneNumber;
                query.CBT = Id;
                query.Status = Status_Id;
                query.GroupAccountNumber = vslaGroup.GroupAccountNumber;
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Edited VSLA with ID : " + Convert.ToString(VslaId);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
            // If one of the validations fails, reload the form and repopulate the dropdown list
            VslaInformation vslaData = getGroupEditInformation(VslaId);
            return View(vslaData);

        }
        /**
         * Add a new Village savings and lending association (VSLA) to the system
         **/
        [HttpPost]
        public ActionResult AddVslaGroup(Vsla vslaGroup, int RegionId, int Id, int Status_Id)
        {
            if (RegionId == 0)
            {
                ModelState.AddModelError("RegionName", "Please select a region");
                return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
            }
            else if (Id == 0)
            {
                ModelState.AddModelError("TechnicalTrainer", "Select Trainer in charge");
                return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
            }
            else if (Status_Id == 0)
            {
                ModelState.AddModelError("Status", "Select the VSLA status");
                return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
            }
            else
            { //! All fields are valid
                /** Generate he VSLA code based on new  VSLA to be created abd the current year(yyyy) */
                int getMaxId = database.Vslas.Max(x => x.VslaId) + 1;
                string getYear = DateTime.Now.Year.ToString().Substring(2);
                string generatedVslaCode = "VS" + getYear + getMaxId.ToString();

                Vsla newVsla = new Vsla
                {
                    VslaCode = generatedVslaCode,
                    VslaName = vslaGroup.VslaName,
                    RegionId = RegionId,
                    DateRegistered = vslaGroup.DateRegistered.HasValue ? vslaGroup.DateRegistered : System.DateTime.Now,
                    DateLinked = vslaGroup.DateLinked.HasValue ? vslaGroup.DateLinked : System.DateTime.Now,
                    PhysicalAddress = vslaGroup.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn ?? "--",
                    GpsLocation = vslaGroup.GpsLocation ?? "--",
                    ContactPerson = vslaGroup.ContactPerson,
                    PositionInVsla = vslaGroup.PositionInVsla,
                    PhoneNumber = vslaGroup.PhoneNumber,
                    CBT = Id,
                    Status = Status_Id,
                    GroupAccountNumber = vslaGroup.GroupAccountNumber
                };
                database.Vslas.Add(newVsla);
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Added VSLA with ID : " + generatedVslaCode;
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
        }
        /**
         * Get the options for re-populating the edit VSLA form, in case of the forms fails
         **/
        public VslaInformation getGroupEditInformation(int id)
        {
            var vsla = (from tb_vsla in database.Vslas
                        join tb_cbt in database.TechnicalTrainers on tb_vsla.CBT equals tb_cbt.Id
                        where tb_vsla.VslaId == id
                        select new { db_vsla = tb_vsla, db_cbt = tb_cbt }).FirstOrDefault();

            // Get a list of all vsla regions to populate in the dropdown list
            List<VslaRegion> regions = new List<VslaRegion>();
            var databaseRegions = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
            foreach (var region in databaseRegions)
            {
                regions.Add(new VslaRegion
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList allRegions = new SelectList(regions, "RegionId", "RegionName", vsla.db_vsla.RegionId);
            // Get the list of all cbts to populate in the dropdown list
            List<TechnicalTrainer> cbts = new List<TechnicalTrainer>();
            var database_cbts = database.TechnicalTrainers.OrderBy(a => a.Name).ToList();
            foreach (var cbt in database_cbts)
            {
                cbts.Add(new TechnicalTrainer
                {
                    Id = cbt.Id,
                    Name = cbt.Name
                });
            }
            SelectList allTrainers = new SelectList(cbts, "Id", "Name", (int)vsla.db_vsla.CBT);

            // Get the status type ie active/inactive
            List<StatusType> statusTypes = new List<StatusType>();
            var databaseStatusTypes = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
            foreach (var statusType in databaseStatusTypes)
            {
                statusTypes.Add(new StatusType
                {
                    Status_Id = statusType.Status_Id,
                    CurrentStatus = statusType.CurrentStatus
                });
            }
            SelectList statusTypesList = new SelectList(statusTypes, "Status_Id", "CurrentStatus", vsla.db_cbt.Status);

            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vsla.db_vsla.VslaId,
                VslaCode = vsla.db_vsla.VslaCode ?? "--",
                VslaName = vsla.db_vsla.VslaName ?? "--",
                VslaRegions = allRegions,
                DateRegistered = vsla.db_vsla.DateRegistered.HasValue ? vsla.db_vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla.db_vsla.DateLinked.HasValue ? vsla.db_vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla.db_vsla.GpsLocation ?? "--",
                ContactPerson = vsla.db_vsla.ContactPerson,
                PositionInVsla = vsla.db_vsla.PositionInVsla,
                PhoneNumber = vsla.db_vsla.PhoneNumber,
                AllTechnicalTrainers = allTrainers,
                StatusType = statusTypesList,
                GroupAccountNumber = vsla.db_vsla.GroupAccountNumber
            };
            return vslaData;
        }
        /**
         * Delete a particular VSLA from the system
         * */
        [HttpGet]
        public ActionResult DeleteVslaGroup(int id)
        {
            var vslaInformation = (from table_vsla in database.Vslas
                                   join table_cbt in database.TechnicalTrainers on table_vsla.CBT equals table_cbt.Id
                                   join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                                   join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                                   where table_vsla.VslaId == id
                                   select new { db_vsla = table_vsla, db_cbt = table_cbt, db_regions = table_regions, db_status = table_status }).SingleOrDefault();

            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vslaInformation.db_vsla.VslaId,
                VslaCode = vslaInformation.db_vsla.VslaCode ?? "--",
                VslaName = vslaInformation.db_vsla.VslaName ?? "--",
                RegionId = vslaInformation.db_regions.RegionName,
                DateRegistered = vslaInformation.db_vsla.DateRegistered,
                DateLinked = vslaInformation.db_vsla.DateLinked,
                PhysicalAddress = vslaInformation.db_vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vslaInformation.db_vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vslaInformation.db_vsla.GpsLocation ?? "--",
                ContactPerson = vslaInformation.db_vsla.ContactPerson,
                PositionInVsla = vslaInformation.db_vsla.PositionInVsla,
                PhoneNumber = vslaInformation.db_vsla.PhoneNumber,
                TechnicalTrainer = vslaInformation.db_cbt.Name ?? "--",
                Status = vslaInformation.db_status.CurrentStatus,
                GroupAccountNumber = "A/C " + vslaInformation.db_vsla.GroupAccountNumber
            };
            return View(vslaData);
        }
        [HttpPost]
        public ActionResult DeleteVslaGroup(Vsla vslaGroup, int id)
        {
            if (ModelState.IsValid && vslaGroup != null)
            {
                vslaGroup.VslaId = id;
                database.Vslas.Attach(vslaGroup);
                database.Vslas.Remove(vslaGroup);
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Deleted VSLA with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
            return View();
        }
        /**
         * View all meetings attached to a particular VSLA
         * */
        public ActionResult VslaGroupMeetings(int id)
        {
            AllVslaMeetingInformation totalMeetings = new AllVslaMeetingInformation();
            List<VslaMeetingInformation> singleMeeting = new List<VslaMeetingInformation>();
            var vslaName = database.Vslas.Find(id);
            singleMeeting = getMeetingData(id);
            totalMeetings.allVslaMeetings = singleMeeting;
            totalMeetings.vslaName = vslaName.VslaName;
            totalMeetings.vslaId = id;
            return View(totalMeetings);
        }

        /**
         * Helper method to query the database all information for all meetings
         * */
        public List<VslaMeetingInformation> getMeetingData(int Id)
        {
            List<VslaMeetingInformation> allMeetings = new List<VslaMeetingInformation>();
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
            AllSingleMeetingInfo allInformation = new AllSingleMeetingInfo();
            List<SingleMeetingInfo> meetingsList = new List<SingleMeetingInfo>();

            // Get the date when the meeting was held
            var meetingDate = database.Meetings.Find(id);

            // Get the all the meeting details
            meetingsList = groupMeetingDetails(id);
            allInformation.allMeetingData = meetingsList;
            allInformation.meetingDate = meetingDate.MeetingDate;
            allInformation.vslaId = id;
            return View(allInformation);
        }

        /**
         * Helper class for getting information concerned with all meetings in the whole system
         * */
        public List<SingleMeetingInfo> groupMeetingDetails(int id)
        {
            List<SingleMeetingInfo> meetings = new List<SingleMeetingInfo>();
            var meeting = (from db_attendance in database.Attendances
                           join db_member in database.Members on db_attendance.MemberId equals db_member.MemberId
                           join db_savings in database.Savings on db_attendance.MemberId equals db_savings.MemberId
                           join db_loan in database.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
                           join db_fines in database.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
                           join db_loanRepayment in database.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
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
                meetings.Add(new SingleMeetingInfo
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
            return meetings;
        }

        /**
         * Export details of a single meeting to a csv file
         * */
        public void ExportSingleMeetingDetailsCSV(int id)
        {
            List<SingleMeetingInfo> meetings = new List<SingleMeetingInfo>();
            var meeting = (from db_attendance in database.Attendances
                           join db_member in database.Members on db_attendance.MemberId equals db_member.MemberId
                           join db_savings in database.Savings on db_attendance.MemberId equals db_savings.MemberId
                           join db_loan in database.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
                           join db_fines in database.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
                           join db_loanRepayment in database.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
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
                meetings.Add(new SingleMeetingInfo
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


        }

        /**
         *  View all members attached to a given vsla
         * */
        public ActionResult VslaGroupMembers(int id)
        {
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

            return allMembers;
        }

        /**
         * Export the list of members to a csv file
         * */
        public void ExportMembersToCSV(int id, string fileName)
        {
            List<VslaMembersInformation> allMembers = new List<VslaMembersInformation>();
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
        public ActionResult GroupMemberDetails(int id, int vslaId)
        {
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
                phoneNumber = member.dt_members.PhoneNo,
                vslaId = (int)vslaId

            };
            return View(memberInfo);
        }
        /**
         * Add a new community based trainer (CBT) to the system
         **/
        [HttpPost]
        public ActionResult AddTechnicalTrainer(TechnicalTrainer new_cbt, int RegionId, int Status_Id)
        {
            if (Status_Id == 0)
            {
                ModelState.AddModelError("Status", "Please select status");
                return Redirect(Url.Action("TechnicalTrainers") + "#addtrainer");
            }
            else if (RegionId == 0)
            {
                ModelState.AddModelError("Region", "Please select Region");
                return Redirect(Url.Action("TechnicalTrainers") + "#addtrainer");
            }
            else
            { // All are valid
                string fullName = new_cbt.FirstName + " " + new_cbt.LastName;
                TechnicalTrainer _cbt = new TechnicalTrainer
                {

                    Name = fullName,
                    FirstName = new_cbt.FirstName,
                    LastName = new_cbt.LastName,
                    Region = RegionId,
                    PhoneNumber = new_cbt.PhoneNumber,
                    Email = new_cbt.Email,
                    Status = Status_Id,
                    Username = new_cbt.Username,
                    Passkey = new_cbt.Passkey
                };

                database.TechnicalTrainers.Add(_cbt);
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Added a new CBT called : " + new_cbt.FirstName + " " + new_cbt.LastName;
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("TechnicalTrainers");
            }
        }

        /**
         * View all information for a particular CBT
         * */
        public ActionResult TrainersDetails(int id)
        {
            TrainerInformation cbtData = getCbtInformationForEditing(id);
            return View(cbtData);
        }

        /**
         * Edit information for a particular CBT
         * */
        [HttpGet]
        public ActionResult EditTrainerDetails(int id)
        {
            var allInformation = (from table_cbt in database.TechnicalTrainers
                                  join table_region in database.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, db_region = table_region }).Single();

            // Regions
            List<VslaRegion> allRegionsList = new List<VslaRegion>();
            var databaseRegions = database.VslaRegions.OrderBy(a => a.RegionName);
            foreach (var region in databaseRegions)
            {
                allRegionsList.Add(new VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName", allInformation.db_region.RegionId);

            // Status types
            List<StatusType> statusOptions = new List<StatusType>();
            var databaseStatuses = database.StatusTypes.OrderBy(a => a.Status_Id);
            foreach (var status in databaseStatuses)
            {
                statusOptions.Add(new StatusType
                {
                    Status_Id = status.Status_Id,
                    CurrentStatus = status.CurrentStatus
                });
            }
            SelectList statusTypes = new SelectList(statusOptions, "Status_Id", "CurrentStatus", allInformation.dt_cbt.Status);

            // Create a cbt object
            TrainerInformation cbtData = new TrainerInformation
            {
                Id = allInformation.dt_cbt.Id,
                FirstName = allInformation.dt_cbt.FirstName,
                LastName = allInformation.dt_cbt.LastName,
                VslaRegionsModel = regionsList,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                StatusType = statusTypes,
                Username = allInformation.dt_cbt.Username,
                Passkey = allInformation.dt_cbt.Passkey
            };
            return View(cbtData);
        }
        [HttpPost]
        public ActionResult EditTrainerDetails(TechnicalTrainer cbt, int id, int RegionId, int Status_Id)
        {
            Regex phoneRegex = new Regex(@"^([0-9\(\)\/\+ \-]*)$");
            if (string.IsNullOrEmpty(cbt.FirstName))
            {
                ModelState.AddModelError("FirstName", "Please Enter a valid First Name");
            }
            else if (string.IsNullOrEmpty(cbt.LastName))
            {
                ModelState.AddModelError("LastName", "Please Enter a valid Last Name");
            }
            else if (RegionId == 0)
            {
                ModelState.AddModelError("Region", "Please Select a region");
            }
            else if (string.IsNullOrEmpty(cbt.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Please Enter Valid Phone Number");
            }
            else if (!phoneRegex.IsMatch(cbt.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Please Enter only digits");
            }
            else if (cbt.PhoneNumber.ToString().Trim().Length > 20)
            {
                ModelState.AddModelError("PhoneNumber", "Max : 20 Characters");
            }
            else if (cbt.PhoneNumber.ToString().Trim().Length < 10)
            {
                ModelState.AddModelError("PhoneNumber", "Min : 10 Characters");
            }
            else if (string.IsNullOrEmpty(cbt.Email))
            {
                ModelState.AddModelError("Email", "Please Enter Valid Email Address");
            }

            else if (Status_Id == 0)
            {
                ModelState.AddModelError("Status", "Please select status");
            }
            else
            {
                string fullname = cbt.FirstName + " " + cbt.LastName;
                var query = database.TechnicalTrainers.Find(id);
                query.Name = fullname;
                query.FirstName = cbt.FirstName;
                query.LastName = cbt.LastName;
                query.Region = RegionId;
                query.PhoneNumber = cbt.PhoneNumber;
                query.Email = cbt.Email;
                query.Status = Status_Id;
                query.Username = cbt.Username;
                query.Passkey = cbt.Passkey;
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Edited CBT called : " + cbt.FirstName + " " + cbt.LastName;
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("TechnicalTrainers");
            }

            // In case validation fails, recreate the form with pre-populated data
            var allInformation = (from table_cbt in database.TechnicalTrainers
                                  join table_region in database.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, db_region = table_region }).Single();

            // Regions
            List<VslaRegion> allRegionsList = new List<VslaRegion>();
            var databaseRegions = database.VslaRegions.OrderBy(a => a.RegionName);
            foreach (var region in databaseRegions)
            {
                allRegionsList.Add(new VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName", allInformation.db_region.RegionId);

            // Status types
            List<StatusType> statusOptions = new List<StatusType>();
            var databaseStatuses = database.StatusTypes.OrderBy(a => a.Status_Id);
            foreach (var status in databaseStatuses)
            {
                statusOptions.Add(new StatusType
                {
                    Status_Id = status.Status_Id,
                    CurrentStatus = status.CurrentStatus
                });
            }
            SelectList statusTypes = new SelectList(statusOptions, "Status_Id", "CurrentStatus", allInformation.dt_cbt.Status);

            // Create a cbt object
            TrainerInformation cbtData = new TrainerInformation
            {
                Id = allInformation.dt_cbt.Id,
                Name = allInformation.dt_cbt.Name,
                VslaRegionsModel = regionsList,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                StatusType = statusTypes
            };
            return View(cbtData);
        }

        /**
         * Function to query the database and get Information related to a particular CBT for editing
         * */
        public TrainerInformation getCbtInformationForEditing(int id)
        {
            var allInformation = (from table_cbt in database.TechnicalTrainers
                                  join table_region in database.VslaRegions on table_cbt.Region equals table_region.RegionId
                                  join table_status in database.StatusTypes on table_cbt.Status equals table_status.Status_Id
                                  where table_cbt.Id == id
                                  select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status }).Single();
            TrainerInformation cbtData = new TrainerInformation
            {
                Id = allInformation.dt_cbt.Id,
                FirstName = allInformation.dt_cbt.FirstName,
                LastName = allInformation.dt_cbt.LastName,
                Region = allInformation.dt_region.RegionName,
                PhoneNumber = allInformation.dt_cbt.PhoneNumber,
                Email = allInformation.dt_cbt.Email,
                Status = allInformation.dt_status.CurrentStatus,
                Username = allInformation.dt_cbt.Username,
                Passkey = allInformation.dt_cbt.Passkey
            };
            return cbtData;
        }

        /**
         * Delete a particular CBT from the system
         * */
        [HttpGet]
        public ActionResult DeleteTrainer(int id)
        {
            TrainerInformation trainerData = getCbtInformationForEditing(id);
            return View(trainerData);
        }
        [HttpPost]
        public ActionResult DeleteTrainer(TechnicalTrainer trainer, int id)
        {
            if (ModelState.IsValid && trainer != null)
            {
                trainer.Id = id;
                database.TechnicalTrainers.Attach(trainer);
                database.TechnicalTrainers.Remove(trainer);
                database.SaveChanges();
                String logString = Convert.ToString(Session["Username"]) + " Deleted CBT ";
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("TechnicalTrainers");
            }
            return View();
        }

        /**
         * Helper method to get information for all registered users
         * */
        public List<UserInformation> usersInformation()
        {
            List<UserInformation> users = new List<UserInformation>();

            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            string sessionUsername = Convert.ToString(Session["Username"]);

            dynamic user_details = null;
            /**
             * Session Level 1 : admin
             * Session Level 2 : user               
             */
            if (sessionUserLevel == 1)
            {

                user_details = (from table_users in database.Users
                                join table_permissions in database.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                select new { db_user = table_users, db_permissions = table_permissions });
            }
            else
            {
                user_details = (from table_users in database.Users
                                join table_permissions in database.UserPermissions on table_users.UserLevel equals table_permissions.Level_Id
                                where table_users.UserLevel == sessionUserLevel && table_users.Username == sessionUsername
                                select new { db_user = table_users, db_permissions = table_permissions });
            }
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
            var vslaDetails = (from data in database.Vslas select data);
            foreach (var item in vslaDetails)
            {
                vslaList.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.RegionId.ToString(),
                    DateRegistered = item.DateRegistered,
                    DateLinked = item.DateLinked,
                    PhysicalAddress = item.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = item.VslaPhoneMsisdn ?? "--",
                    GpsLocation = item.GpsLocation ?? "--",
                    ContactPerson = item.ContactPerson ?? "--",
                    PositionInVsla = item.PositionInVsla ?? "--",
                    PhoneNumber = item.PhoneNumber ?? "--",
                    TechnicalTrainer = "--",
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
            var vslaDetails = (from data in database.Vslas select data);
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
                    TechnicalTrainer = "--",
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
                                           line.TechnicalTrainer,
                                           line.Status.Equals("1") ? "Active" : "Inactive"
                                           ));

            }
            Response.Write(stringWriter.ToString());

            Response.End();
        }

        /**
         * Helper method to get the list of all CBTS that have been added to a system
         * */
        public List<TrainerInformation> getCbtInformation()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            List<TrainerInformation> cbts = new List<TrainerInformation>();
            var data = (from table_cbt in database.TechnicalTrainers
                        join table_region in database.VslaRegions on table_cbt.Region equals table_region.RegionId
                        join table_status in database.StatusTypes on table_cbt.Status equals table_status.Status_Id
                        select new { dt_cbt = table_cbt, dt_region = table_region, dt_status = table_status });

            foreach (var item in data)
            {
                cbts.Add(new TrainerInformation
                {
                    Id = item.dt_cbt.Id,
                    Name = item.dt_cbt.Name,
                    FirstName = item.dt_cbt.FirstName,
                    LastName = item.dt_cbt.LastName,
                    Region = item.dt_region.RegionName,
                    PhoneNumber = item.dt_cbt.PhoneNumber,
                    Email = item.dt_cbt.Email,
                    Status = item.dt_status.CurrentStatus
                });
            }

            return cbts;
        }

        // Generating reports
        public ActionResult VslaReporting()
        {
            string dateString = @"29/07/2014";
            DateTime startDate = Convert.ToDateTime(dateString, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            var results = (from table_meetings in database.Meetings
                           join table_cycles in database.VslaCycles
                               on table_meetings.CycleId equals table_cycles.CycleId
                           join tables_vsla in database.Vslas on table_cycles.VslaId equals
                        tables_vsla.VslaId
                           where table_meetings.MeetingDate >= startDate
                           select new { table_meetings, table_cycles, tables_vsla }).OrderByDescending(id => id.table_meetings.MeetingId);
            List<WeeklyMeetingsData> summary = new List<WeeklyMeetingsData>();
            AllMeetingsData allMeetingsSummary = new AllMeetingsData();
            if (null != results)
            {
                foreach (var item in results)
                {
                    summary.Add(new WeeklyMeetingsData
                    {
                        meetingId = item.table_meetings.MeetingId,
                        cashExpenses = (long)item.table_meetings.CashExpenses,
                        cashFines = (long)item.table_meetings.CashFines,
                        cashFromBank = (long)item.table_meetings.CashFromBank,
                        cashFromBox = (long)item.table_meetings.CashFromBox,
                        cashSavedBank = (long)item.table_meetings.CashSavedBank,
                        cashSavedBox = (long)item.table_meetings.CashSavedBox,
                        cashWelfare = (long)item.table_meetings.CashWelfare,
                        dateSent = item.table_meetings.DateSent,
                        meetingDate = item.table_meetings.MeetingDate,
                        countOfMembersPresent = (int)item.table_meetings.CountOfMembersPresent,
                        sumOfSavings = (long)item.table_meetings.SumOfSavings,
                        sumOfLoansIssued = (long)item.table_meetings.SumOfLoanIssues,
                        sumOfLoanRepayments = (long)item.table_meetings.SumOfLoanRepayments,
                        vslaName = item.tables_vsla.VslaName,
                        vslaId = (int)item.table_cycles.VslaId,
                        VslaCode = item.tables_vsla.VslaCode
                    });

                }
            }
            allMeetingsSummary.meetingsSummary = summary;
            return View(allMeetingsSummary);
        }
        /**
         * Export weekly meetings summary to a csv file
         */
        public void ExportWeeklySummary()
        {
            string dateString = @"29/07/2014";
            DateTime startDate = Convert.ToDateTime(dateString, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            var results = (from table_meetings in database.Meetings
                           join table_cycles in database.VslaCycles
                               on table_meetings.CycleId equals table_cycles.CycleId
                           join tables_vsla in database.Vslas on table_cycles.VslaId equals
                        tables_vsla.VslaId
                           where table_meetings.MeetingDate >= startDate
                           select new { table_meetings, table_cycles, tables_vsla }).OrderByDescending(id => id.table_meetings.MeetingId);
            List<WeeklyMeetingsData> summary = new List<WeeklyMeetingsData>();
            foreach (var item in results)
            {
                summary.Add(new WeeklyMeetingsData
                {
                    meetingId = item.table_meetings.MeetingId,
                    cashExpenses = (long)item.table_meetings.CashExpenses,
                    cashFines = (long)item.table_meetings.CashFines,
                    cashFromBank = (long)item.table_meetings.CashFromBank,
                    cashFromBox = (long)item.table_meetings.CashFromBox,
                    cashSavedBank = (long)item.table_meetings.CashSavedBank,
                    cashSavedBox = (long)item.table_meetings.CashSavedBox,
                    cashWelfare = (long)item.table_meetings.CashWelfare,
                    dateSent = item.table_meetings.DateSent,
                    meetingDate = item.table_meetings.MeetingDate,
                    countOfMembersPresent = (int)item.table_meetings.CountOfMembersPresent,
                    sumOfSavings = (long)item.table_meetings.SumOfSavings,
                    sumOfLoansIssued = (long)item.table_meetings.SumOfLoanIssues,
                    sumOfLoanRepayments = (long)item.table_meetings.SumOfLoanRepayments,
                    vslaName = item.tables_vsla.VslaName,
                    vslaId = (int)item.table_cycles.VslaId,
                    VslaCode = item.tables_vsla.VslaCode
                });

            }
            StringWriter stringWriter = new StringWriter();
            stringWriter.WriteLine("\"VSLA Code\",\"VSLA Name\",\"Meeting Id\",\"Cash Expenses\",\"Cash Fines\",\"Cash From Bank\",\"Cash From Box\",\"Cash Saved Bank\",\"Cash Saved Box\",\"Date Sent\",\"Meeting Date \",\"Members Present\", \"Savings\",\"Loans Issued\", \"Loan Repayment\"");

            Response.ClearContent();
            String attachment = "attachment;filename=Weekly_Summary_Meetings.csv";
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "text/csv";

            foreach (var line in summary)
            {

                stringWriter.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\",\"{12}\",\"{13}\",\"{14}\"",
                                           line.VslaCode,
                                           line.vslaName,
                                           line.meetingId,
                                           line.cashExpenses,
                                           line.cashFines,
                                           line.cashFromBank,
                                           line.cashFromBox,
                                           line.cashSavedBank,
                                           line.cashSavedBox,
                                           line.formattedDateSent,
                                           line.formattedMeetingDate,
                                           line.countOfMembersPresent,
                                           line.sumOfSavings,
                                           line.sumOfLoansIssued,
                                           line.sumOfLoanRepayments
                                           ));

            }
            Response.Write(stringWriter.ToString());

            Response.End();
        }
    }
}
