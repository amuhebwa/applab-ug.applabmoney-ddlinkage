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
using DigitizingDataBizLayer.Repositories;
using DigitizingDataDomain.Model;
namespace DigitizingDataAdminApp.Controllers
{
    public class DashboardController : Controller
    {
        ActivityLoggingSystem activityLoggingSystem;
        public DashboardController()
        {
            activityLoggingSystem = new ActivityLoggingSystem();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            if (Session["UserId"] != null)
            {
                VslaRepo vslaRepo = new VslaRepo();
                MemberRepo memberRepo = new MemberRepo();
                MeetingRepo meetingRepo = new MeetingRepo();
                AttendanceRepo attendanceRepo = new AttendanceRepo();
                DataSubmissionRepo dataSubmisssionRepo = new DataSubmissionRepo();
                // Total VSLAs
                long totalVslas = vslaRepo.countVslas();

                // Total/Male/Female Members
                int femaleMembers = (int)memberRepo.countFemaleMembers();
                int maleMembers = (int)memberRepo.countMaleMembers();
                int totalMembers = (maleMembers + femaleMembers);

                // Savings, Loans, Repayments
                double totalSavings = meetingRepo.findTotalSavings();
                double totalLoans = meetingRepo.findTotalLoans();
                double totalLoanRepayment = meetingRepo.findTotalLoanRepayment();

                // Attendance
                int totalPresent = (int)attendanceRepo.totalMembersPresent();
                int totalAbsent = (int)attendanceRepo.totalMembersAbsent();

                // Total Meetings
                int totalMeetings = (int)meetingRepo.totalMeetingsHeld();

                // Total Submission
                int totalSubmissions = (int)dataSubmisssionRepo.findTotalSubmissions();

                DashboardData summary = new DashboardData
                {
                    femaleMembers = femaleMembers,
                    maleMembers = maleMembers,
                    totalMembers = totalMembers,
                    totalAbsent = totalAbsent,
                    totalPresent = totalPresent,
                    totalLoanRepayment = totalLoanRepayment,
                    totalLoans = totalLoans,
                    totalSavings = totalSavings,
                    totalSubmissions = totalSubmissions,
                    totalMeeetings = totalMeetings,
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

        // Members by Gender
        public ActionResult showMembersByGender()
        {
            MemberRepo memberRepo = new MemberRepo();
            int femaleMembers = (int)memberRepo.countFemaleMembers();
            int maleMembers = (int)memberRepo.countMaleMembers();

            new Chart(width: 300, height: 300)
            .AddTitle("Members By Gender")
            .AddSeries(chartType: "pie",
                xValue: new[] { "Males", "Females" },
                yValues: new[] { maleMembers.ToString(), femaleMembers.ToString() }
                ).AddLegend().Write("bmp");
            return null;
        }
        // Attendance (Absent/Present)
        public ActionResult showAttendance()
        {
            AttendanceRepo attendanceRepo = new AttendanceRepo();
            int totalPresent = (int)attendanceRepo.totalMembersPresent();
            int totalAbsent = (int)attendanceRepo.totalMembersAbsent();

            new Chart(width: 300, height: 300)
            .AddTitle("Overall Attendance")
            .AddSeries(chartType: "pie",
                xValue: new[] { "Present", "Absent" },
               yValues: new[] { totalPresent.ToString(), totalAbsent.ToString() }
               ).AddLegend().Write("bmp");

            return null;
        }
        // Show total savings, loans given out and loan repayments
        public ActionResult showSavingsLoansAndRepayments()
        {
            MeetingRepo meetingRepo = new MeetingRepo();
            double totalSavings = meetingRepo.findTotalSavings();
            double totalLoans = meetingRepo.findTotalLoans();
            double totalLoanRepayment = meetingRepo.findTotalLoanRepayment();

            new Chart(width: 300, height: 300)
            .AddTitle("Financial Break down")
            .AddSeries(chartType: "column",
            xValue: new[] { "Total Savings", "Total Loans", "Loan Repayment" },
            yValues: new[] { totalSavings.ToString(), totalLoans.ToString(), totalLoanRepayment.ToString() })
            .Write("bmp");

            return null;
        }
        /**
         * ************ SYSTEM USERS *****************
         */
        public ActionResult SystemUsers()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);

            List<UserInformation> singleUser = usersInformation();
            SystemUsersInformation allUsers = new SystemUsersInformation();
            allUsers.AllUsersList = singleUser;
            allUsers.SessionUserLevel = sessionUserLevel;
            allUsers.AccessLevel = getAccessPermissions();
            return View(allUsers);
        }

        // Helper method to get information for all registered users
        public List<UserInformation> usersInformation()
        {
            UserRepo userRepo = new UserRepo();
            List<UserInformation> users = new List<UserInformation>();
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            string sessionUsername = Convert.ToString(Session["Username"]);

            // Session Level 1 : admin
            // Session Level 2 : user               
            List<Users> userDetails = null;

            if (sessionUserLevel == 1)
            { // ADMIN
                userDetails = userRepo.findAllUsers();
            }
            else
            { // USER
                userDetails = userRepo.findParticularUser(sessionUserLevel, sessionUsername);
            }

            foreach (var item in userDetails)
            {
                users.Add(
                    new UserInformation
                    {
                        Id = item.Id,
                        Username = item.Username,
                        Fullname = item.Fullname,
                        Email = item.Email,
                        UserLevel = item.UserLevel == 1 ? "Admin" : "User"
                    });
            }
            return users;
        }

        // Get the user level permissions to populate in the drop down list 
        public SelectList getAccessPermissions()
        {
            UserPermissionsRepo permissionsRepo = new UserPermissionsRepo();
            List<UserPermissions> permissions = new List<UserPermissions>();
            permissions.Add(new UserPermissions { Level_Id = 0, UserType = "- Select Access Level -" });

            List<UserPermissions> allPermissions = permissionsRepo.allUserPermissions();

            foreach (var permission in allPermissions)
            {
                permissions.Add(new UserPermissions
                {
                    Level_Id = permission.Level_Id,
                    UserType = permission.UserType
                });
            }
            SelectList acccessPermissions = new SelectList(permissions, "Level_Id", "UserType", 0);
            return acccessPermissions;
        }

        // Display all information for a particular user
        public ActionResult UserDetails(int id)
        {
            UserRepo userRepo = new UserRepo();
            Users userDetails = userRepo.findUserDetails(id);
            UserInformation userData = new UserInformation
            {
                Id = userDetails.Id,
                Username = userDetails.Username,
                Password = userDetails.Password,
                Fullname = userDetails.Fullname,
                Email = userDetails.Email,
                DateCreated = userDetails.DateCreated,
                UserLevel = userDetails.UserLevel == 1 ? "Admin" : "User"
            };
            return View(userData);
        }

        [HttpPost]
        public ActionResult AddUser(Users user, int Level_Id)
        {
            Boolean insertResult = false;
            PasswordHashing passwordHashing = new PasswordHashing();
            string _hashedPassword = passwordHashing.hashedPassword(user.Password.Trim());
            if (Level_Id == 0)
            {
                ModelState.AddModelError("AccessLevel", "Please select Access Level");
                return Redirect(Url.Action("SystemUsers") + "#addusertab");
            }
            else
            { // All fields have been validated
                Users addUser = new Users();
                addUser.Username = Convert.ToString(user.Username);
                addUser.Password = Convert.ToString(user.Password);
                addUser.Fullname = Convert.ToString(user.Fullname);
                addUser.Email = Convert.ToString(user.Email);
                addUser.DateCreated = System.DateTime.Today;
                addUser.UserLevel = Convert.ToInt32(Level_Id);

                UserRepo _userRepo = new UserRepo();
                insertResult = _userRepo.Insert(addUser);
                if (insertResult)
                { // TRUE
                    // log details
                    String logString = Convert.ToString(Session["Username"]) + " Added a new User";
                    activityLoggingSystem.logActivity(logString, 0);

                    return RedirectToAction("SystemUsers");
                }
                else
                {// FALSE
                    return Redirect(Url.Action("SystemUsers") + "#addusertab");
                }
            }
        }

        // Edit a particular user's information.   
        public ActionResult EditUser(int id)
        {
            UserInformation user_data = particularUserData(id);
            return View(user_data);
        }

        [HttpPost]
        public ActionResult EditUser(UserInformation user, int id, int Level_Id)
        {
            // Get the access permissions of the user current logged in 
            int permissionLevel = Convert.ToInt32(Session["UserLevel"]);
            if (string.IsNullOrEmpty(user.Username))
            {
                ModelState.AddModelError("Username", "Username cannot be empty");
                UserInformation user_data = particularUserData(id);
                return View(user_data);
            }
            if (string.IsNullOrEmpty(user.Fullname))
            {
                ModelState.AddModelError("Fullname", "Fullname cannot be empty");
                UserInformation user_data = particularUserData(id);
                return View(user_data);
            }
            else if (string.IsNullOrEmpty(user.Email))
            {
                ModelState.AddModelError("Email", "Email cannot be empty");
                UserInformation user_data = particularUserData(id);
                return View(user_data);
            }
            else if (Level_Id == 0)
            {
                ModelState.AddModelError("AccessLevel", "Please select Access Level");
                UserInformation user_data = particularUserData(id);
                return View(user_data);
            }
            else
            {
                // Hash the password, if there's a need to change one
                PasswordHashing passwordHashing = new PasswordHashing();
                string hashedPassword = string.Empty;
                if (user.Password != null)
                {
                    hashedPassword = passwordHashing.hashedPassword(user.Password);
                }
                // Get access to the user repo and checkif the user exists
                UserRepo _userRepo = new UserRepo();
                int _userId = Convert.ToInt32(user.Id);
                Users currentUser = _userRepo.findUserById(_userId);
                Boolean updateResult = false;
                if (currentUser != null)
                {
                    currentUser.Username = user.Username;

                    if (user.Password != null)
                    {
                        currentUser.Password = hashedPassword;
                    }
                    currentUser.Fullname = user.Fullname;
                    currentUser.Email = user.Email;
                    currentUser.UserLevel = permissionLevel == 1 ? Level_Id : 2;
                    if (currentUser.Id > 0)
                    {
                        updateResult = _userRepo.Update(currentUser);
                    }
                }

                if (updateResult)
                {
                    return RedirectToAction("SystemUsers");
                }
                else
                {
                    return View();
                }
            }
        }
        // Helper function to get a user's information base on their ID
        public UserInformation particularUserData(int id)
        {
            UserRepo userRepo = new UserRepo();
            UserPermissionsRepo permissionsRepo = new UserPermissionsRepo();
            Users userDetails = userRepo.findUserDetails(id);
            // Get access levels
            List<UserPermissions> permissions = new List<UserPermissions>();
            List<UserPermissions> allPermissions = permissionsRepo.allUserPermissions();
            foreach (var permission in allPermissions)
            {
                permissions.Add(new UserPermissions
                {
                    Level_Id = permission.Level_Id,
                    UserType = permission.UserType
                });
            }
            SelectList acccessPermissions = new SelectList(permissions, "Level_Id", "UserType", userDetails.UserLevel);
            UserInformation user_data = new UserInformation
            {
                Id = userDetails.Id,
                Username = userDetails.Username,
                Fullname = userDetails.Fullname,
                Email = userDetails.Email,
                DateCreated = userDetails.DateCreated,
                AccessLevel = acccessPermissions,
            };
            return user_data;
        }

        // Delete a particular user form the system
        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            UserRepo userRepo = new UserRepo();
            Users userDetails = userRepo.findUserDetails(id);
            UserInformation userData = new UserInformation
            {
                Id = userDetails.Id,
                Username = userDetails.Username,
                Password = userDetails.Password,
                Fullname = userDetails.Fullname,
                Email = userDetails.Email,
                DateCreated = userDetails.DateCreated,
                UserLevel = userDetails.UserLevel == 1 ? "Admin" : "User"
            };
            return View(userData);
        }

        [HttpPost]
        public ActionResult DeleteUser(Users user, int id)
        {
            Boolean deleteResult = false;
            String logString = string.Empty;
            if (ModelState.IsValid && user != null)
            {
                int _userId = Convert.ToInt32(user.Id);
                UserRepo _userRepo = new UserRepo();
                Users currentUser = _userRepo.findUserById(_userId);

                if (currentUser != null)
                {
                    deleteResult = _userRepo.Delete(currentUser);
                }
            }
            if (deleteResult)
            { //TRUE
                // Data log
                logString = Convert.ToString(Session["Username"]) + " Deleted User with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("SystemUsers");
            }
            else
            {// FALSE
                logString = Convert.ToString(Session["Username"]) + "Failed to delete User with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 1);
                return View();
            }
        }

        /**
         * ******************  TECHNICAL USERS  *******************
         */

        // All Technical Trainers
        public ActionResult TechnicalTrainers()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]); // Get the user level of the current session
            AllTrainersInformation allTrainers = new AllTrainersInformation();
            List<TrainerInformation> allrainersData = new List<TrainerInformation>();
            allrainersData = allTechTrainerInformation();
            allTrainers.AllTrainersList = allrainersData;
            allTrainers.SessionUserLevel = sessionUserLevel;
            allTrainers.VslaRegionsModel = getVslaRegions();
            allTrainers.StatusType = getStatusTypes();

            return View(allTrainers);
        }

        // Helper method to get the list of all Technical Trainers that have been added to a system
        public List<TrainerInformation> allTechTrainerInformation()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            List<TrainerInformation> trainerList = new List<TrainerInformation>();
            TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
            List<TechnicalTrainer> allTrainers = _technicalTrainerRepo.findAllTechnicalTrainers();

            foreach (var item in allTrainers)
            {
                trainerList.Add(new TrainerInformation
                {
                    Id = item.Id,
                    Name = item.Name,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Region = item.VslaRegion.RegionName,
                    PhoneNumber = item.PhoneNumber,
                    Email = item.Email,
                    Status = item.Status == "1" ? "Active" : "Inactive"
                });
            }

            return trainerList;
        }
        // List Of All Regions
        public SelectList getVslaRegions()
        {
            List<VslaRegion> regionsList = new List<VslaRegion>();
            regionsList.Add(new VslaRegion()
            {
                RegionId = 0,
                RegionName = "-Select Region-"
            });
            VslaRegionRepo _vslaRegionRepo = new VslaRegionRepo();
            List<VslaRegion> allRegions = _vslaRegionRepo.findAllRegions();
            foreach (var region in allRegions)
            {
                regionsList.Add(new VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList data = new SelectList(regionsList, "RegionId", "RegionName", 0);
            return data;
        }

        // List of status types ie active/inactive
        public SelectList getStatusTypes()
        {
            List<StatusType> statusOptions = new List<StatusType>();

            statusOptions.Add(new StatusType()
            {
                Status_Id = 0,
                CurrentStatus = "-Select Status-"
            });

            StatusTypeRepo _statusTypeRepo = new StatusTypeRepo();
            List<StatusType> allStatusTypes = _statusTypeRepo.findAllStatusType();
            foreach (var status in allStatusTypes)
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

        // List of all Technical trainers from list selector
        public SelectList getAllTrainers()
        {
            List<TechnicalTrainer> trainers = new List<TechnicalTrainer>();
            trainers.Add(new TechnicalTrainer { Id = 0, Name = "-Select Trainer" });
            TechnicalTrainerRepo _technicalTrinerRepo = new TechnicalTrainerRepo();
            List<TechnicalTrainer> allTainers = _technicalTrinerRepo.findAllTechnicalTrainers();
            foreach (var trainer in allTainers)
            {
                trainers.Add(new TechnicalTrainer
                {
                    Id = trainer.Id,
                    Name = trainer.Name
                });
            }
            SelectList result = new SelectList(trainers, "Id", "Name", 0);
            return result;
        }

        // View all information for a particular Technical Trainer
        public ActionResult TrainersDetails(int id)
        {
            TrainerInformation trainerData = findParticularTrainerDetails(id);
            return View(trainerData);
        }

        // Get all details for a particular trainer
        public TrainerInformation findParticularTrainerDetails(int id)
        {
            TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
            TechnicalTrainer trainer = _technicalTrainerRepo.findParticularTrainer(id);
            TrainerInformation trainerData = new TrainerInformation
            {
                Id = trainer.Id,
                FirstName = trainer.FirstName,
                LastName = trainer.LastName,
                Region = trainer.VslaRegion.RegionName,
                PhoneNumber = trainer.PhoneNumber,
                Email = trainer.Email,
                Status = trainer.Status == "1" ? "Active" : "Inactive",
                Username = trainer.Username,
                Passkey = trainer.Passkey
            };
            return trainerData;
        }

        /**
        * Add a new Technical Trainer to the system
        **/
        [HttpPost]
        public ActionResult AddTechnicalTrainer(TechnicalTrainer trainer, int regionId, int Status_Id)
        {
            if (Status_Id == 0)
            {
                ModelState.AddModelError("Status", "Please select status");
                return Redirect(Url.Action("TechnicalTrainers") + "#addtrainer");
            }
            else if (regionId == 0)
            {
                ModelState.AddModelError("Region", "Please select Region");
                return Redirect(Url.Action("TechnicalTrainers") + "#addtrainer");
            }
            else
            { // All are valid

                Boolean insertResult = false;
                TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();

                TechnicalTrainer newTrainer = new TechnicalTrainer();

                string fullName = trainer.FirstName + " " + trainer.LastName;
                newTrainer.Name = Convert.ToString(fullName);
                newTrainer.FirstName = Convert.ToString(trainer.FirstName);
                newTrainer.LastName = Convert.ToString(trainer.LastName);
                // Region
                VslaRegion vslaRegion = new VslaRegion();
                vslaRegion.RegionId = Convert.ToInt32(regionId);
                newTrainer.VslaRegion = vslaRegion;

                newTrainer.PhoneNumber = Convert.ToString(trainer.PhoneNumber);
                newTrainer.Email = Convert.ToString(trainer.Email);
                newTrainer.Status = Convert.ToString(Status_Id);
                newTrainer.Username = Convert.ToString(trainer.Username);
                newTrainer.Passkey = Convert.ToString(trainer.Passkey);

                insertResult = _technicalTrainerRepo.Insert(newTrainer);
                if (insertResult)
                { // TRUE
                    String logString = Convert.ToString(Session["Username"]) + " Added a new Technical Trainer called : " + trainer.FirstName + " " + trainer.LastName;
                    activityLoggingSystem.logActivity(logString, 0);
                    return RedirectToAction("TechnicalTrainers");
                }
                else
                { // FALSE
                    return View();
                }
            }
        }

        // Edit information for a particular Technical Trainer
        [HttpGet]
        public ActionResult EditTrainerDetails(int id)
        {
            TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
            int _trainerId = Convert.ToInt32(id);
            TechnicalTrainer allInformation = _technicalTrainerRepo.findParticularTrainer(_trainerId);

            // Regions
            List<VslaRegion> allRegionsList = new List<VslaRegion>();
            VslaRegionRepo _vslaRegionRepo = new VslaRegionRepo();
            List<VslaRegion> databaseRegions = _vslaRegionRepo.findAllRegions();
            foreach (var region in databaseRegions)
            {
                allRegionsList.Add(new VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName", allInformation.VslaRegion.RegionId);

            // Status types
            List<StatusType> statusOptions = new List<StatusType>();
            StatusTypeRepo _statusTypeRepo = new StatusTypeRepo();
            List<StatusType> databaseStatuses = _statusTypeRepo.findAllStatusType();
            foreach (var status in databaseStatuses)
            {
                statusOptions.Add(new StatusType
                {
                    Status_Id = status.Status_Id,
                    CurrentStatus = status.CurrentStatus
                });
            }
            SelectList statusTypes = new SelectList(statusOptions, "Status_Id", "CurrentStatus", allInformation.Status);

            // Create a a technical trainer object
            TrainerInformation trainerData = new TrainerInformation
            {
                Id = allInformation.Id,
                FirstName = allInformation.FirstName,
                LastName = allInformation.LastName,
                VslaRegionsModel = regionsList, // Drop down list for regions
                PhoneNumber = allInformation.PhoneNumber,
                Email = allInformation.Email,
                StatusType = statusTypes, // Drop down list for status types
                Username = allInformation.Username,
                Passkey = allInformation.Passkey
            };
            return View(trainerData);
        }

        [HttpPost]
        public ActionResult EditTrainerDetails(TechnicalTrainer trainer, int id, int regionId, int Status_Id)
        {
            Boolean updateResult = false;
            int _trainerId = Convert.ToInt32(trainer.Id);
            TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
            TechnicalTrainer currentTrainer = _technicalTrainerRepo.findParticularTrainer(_trainerId);
            if (currentTrainer != null)
            {
                string fullname = trainer.FirstName + " " + trainer.LastName;
                currentTrainer.Name = Convert.ToString(fullname);
                currentTrainer.FirstName = Convert.ToString(trainer.FirstName);
                currentTrainer.LastName = Convert.ToString(trainer.LastName);

                // region id
                VslaRegion vslaRegion = new VslaRegion();
                vslaRegion.RegionId = Convert.ToInt32(regionId);
                currentTrainer.VslaRegion = vslaRegion;

                currentTrainer.PhoneNumber = Convert.ToString(trainer.PhoneNumber);
                currentTrainer.Email = Convert.ToString(trainer.Email);
                currentTrainer.Status = Convert.ToString(Status_Id);
                currentTrainer.Username = Convert.ToString(trainer.Username);
                currentTrainer.Passkey = Convert.ToString(trainer.Passkey);

                updateResult = _technicalTrainerRepo.Update(currentTrainer);
            }
            if (updateResult)
            { // TRUE
                return RedirectToAction("TechnicalTrainers");
            }
            else
            { // FALSE
                return View();
            }
        }

        // Delete a particular Technical Trainer from the system
        [HttpGet]
        public ActionResult DeleteTrainer(int id)
        {
            TrainerInformation trainerData = findParticularTrainerDetails(id);
            return View(trainerData);
        }
        [HttpPost]
        public ActionResult DeleteTrainer(TechnicalTrainer trainer, int id)
        {
            Boolean deleteResult = false;
            if (ModelState.IsValid && trainer != null)
            {
                int _trainerId = Convert.ToInt32(trainer.Id);
                TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
                TechnicalTrainer currentTrainer = _technicalTrainerRepo.findParticularTrainer(_trainerId);
                if (currentTrainer != null)
                {
                    deleteResult = _technicalTrainerRepo.Delete(currentTrainer);
                }
            }
            if (deleteResult)
            { //TRUE
                String logString = Convert.ToString(Session["Username"]) + " Deleted Technical Trainer ";
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("TechnicalTrainers");
            }
            else
            { // FALSE
                return View();
            }

        }

        /**
         *************  VSLA GROUPS ***************
         */

        // Display All VSLA Groups in a list
        public ActionResult VslaGroupInformation()
        {
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            VslaGroupsInformation allGroups = new VslaGroupsInformation();
            List<VslaInformation> getVslaData = getVslaInformation();

            allGroups.AllGroupsList = getVslaData;
            allGroups.sessionUserLevel = sessionUserLevel;
            allGroups.AllTechnicalTrainers = getAllTrainers();
            allGroups.VslaRegions = getVslaRegions();
            allGroups.StatusType = getStatusTypes();
            allGroups.groupSupportProvided = getSupportType();
            return View(allGroups);
        }

        // Helper method to get all information concerning vsla
        public List<VslaInformation> getVslaInformation()
        {
            VslaRepo _vslaRepo = new VslaRepo();
            List<Vsla> vslaDetails = _vslaRepo.findAllVslas();
            List<VslaInformation> vslaData = new List<VslaInformation>();
            foreach (var item in vslaDetails)
            {
                vslaData.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.VslaRegion.RegionName != null ? item.VslaRegion.RegionName : "--",
                    DateRegistered = item.DateRegistered,
                    DateLinked = item.DateLinked,
                    PhysicalAddress = item.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = item.VslaPhoneMsisdn ?? "--",
                    GpsLocation = item.GpsLocation ?? "--",
                    ContactPerson = item.ContactPerson ?? "--",
                    PositionInVsla = item.PositionInVsla ?? "--",
                    PhoneNumber = item.PhoneNumber ?? "--",
                    TechnicalTrainer = item.CBT.Name,
                    Status = item.Status.ToString() ?? "--"
                });
            }
            return vslaData;
        }

        // Get the group support modules that have been provided to the group by technical trainers
        public List<GroupSupportInfo> getSupportType()
        {
            GroupSupportRepo _groupSupportRepo = new GroupSupportRepo();
            List<GroupSupport> support = _groupSupportRepo.findAllGroupSupport();
            List<GroupSupportInfo> supportGiven = new List<GroupSupportInfo>();
            foreach (var sp in support)
            {
                supportGiven.Add(new GroupSupportInfo()
                {
                    GroupName = sp.VslaId.VslaName,
                    TrainerName = sp.TrainerId.Name,
                    SupportType = sp.SupportType,
                    supportDate = sp.SupportDate
                });
            }
            return supportGiven;

        }

        // VSLA Group details for a particular group queried based on the group id
        public ActionResult VslaGroupDetails(int id)
        {
            VslaInformation vslaDetails = findVslaDetails(id);
            return View(vslaDetails);
        }
        // Helper function to find VSLA details
        public VslaInformation findVslaDetails(int id)
        {
            int vslaId = Convert.ToInt32(id);
            VslaRepo _vslaRepo = new VslaRepo();
            Vsla vslaDetails = _vslaRepo.FindVslaById(vslaId);
            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vslaDetails.VslaId,
                VslaCode = vslaDetails.VslaCode ?? "--",
                VslaName = vslaDetails.VslaName ?? "--",
                RegionId = vslaDetails.VslaRegion.RegionName,
                DateRegistered = vslaDetails.DateRegistered,
                DateLinked = vslaDetails.DateLinked,
                PhysicalAddress = vslaDetails.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vslaDetails.VslaPhoneMsisdn ?? "--",
                GpsLocation = vslaDetails.GpsLocation ?? "--",
                ContactPerson = vslaDetails.ContactPerson ?? "--",
                PositionInVsla = vslaDetails.PositionInVsla,
                PhoneNumber = vslaDetails.PhoneNumber ?? "--",
                TechnicalTrainer = vslaDetails.CBT.Name ?? "--",
                Status = vslaDetails.Status == 1 ? "Active" : "Inactive",
                GroupAccountNumber = "A/C " + vslaDetails.GroupAccountNumber ?? "--",
                NumberOfCycles = Convert.ToString(vslaDetails.NumberOfCycles)
            };
            return vslaData;
        }

        // Add a new VSLA Group
        [HttpPost]
        public ActionResult AddVslaGroup(Vsla vslaGroup, int RegionId, int Id, int Status_Id)
        {
            string _vslaCode = generateVslaCode();

            // Create new VSLA group object
            Vsla newVsla = new Vsla();
            newVsla.VslaCode = generateVslaCode();
            newVsla.VslaName = vslaGroup.VslaName;

            // region
            VslaRegion vslaRegion = new VslaRegion();
            vslaRegion.RegionId = Convert.ToInt32(RegionId);
            newVsla.VslaRegion = vslaRegion;

            newVsla.DateRegistered = vslaGroup.DateRegistered.HasValue ? vslaGroup.DateRegistered : System.DateTime.Now;
            newVsla.DateLinked = vslaGroup.DateLinked.HasValue ? vslaGroup.DateLinked : System.DateTime.Now;
            newVsla.PhysicalAddress = vslaGroup.PhysicalAddress ?? "--";
            newVsla.VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn ?? "--";
            newVsla.GpsLocation = vslaGroup.GpsLocation ?? "--";
            newVsla.ContactPerson = vslaGroup.ContactPerson;
            newVsla.PositionInVsla = vslaGroup.PositionInVsla;
            newVsla.PhoneNumber = vslaGroup.PhoneNumber;

            // technical trainer
            TechnicalTrainer _trainer = new TechnicalTrainer();
            _trainer.Id = Convert.ToInt32(Id);
            newVsla.CBT = _trainer;

            newVsla.Status = Status_Id;
            newVsla.GroupAccountNumber = vslaGroup.GroupAccountNumber;
            newVsla.NumberOfCycles = vslaGroup.NumberOfCycles;

            VslaRepo _vslaRepo = new VslaRepo();
            Boolean insertResult = false;
            insertResult = _vslaRepo.Insert(newVsla);
            if (insertResult)
            { // TRUE
                String logString = Convert.ToString(Session["Username"]) + " Added VSLA with ID : " + _vslaCode;
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
            else
            { // FALSE 
                return View();
            }
        }

        // Helper method to generate VSLA Code of the new Group being added
        public string generateVslaCode()
        {
            string year = DateTime.Now.Year.ToString().Substring(2);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string unixTimeStamp = Convert.ToString(unixTimestamp).Substring(4);
            string vslaCode = "VS" + year + unixTimeStamp;
            return vslaCode;
        }

        // Edit a given VSLA Group
        [HttpGet]
        public ActionResult EditVslaGroup(int id)
        {
            VslaInformation vslaData = vslaEditInformation(id);
            return View(vslaData);
        }

        [HttpPost]
        public ActionResult EditVslaGroup(VslaInformation vslaGroup, int VslaId, int Id, int RegionId, int Status_Id)
        {
            int _vslaId = Convert.ToInt32(vslaGroup.VslaId);
            VslaRepo _vslaRepo = new VslaRepo();
            Vsla currentVsla = _vslaRepo.FindVslaById(_vslaId);
            Boolean updateResult = false;
            if (currentVsla != null)
            {
                currentVsla.VslaName = vslaGroup.VslaName;
                currentVsla.VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn;
                currentVsla.GpsLocation = vslaGroup.GpsLocation;
                currentVsla.DateRegistered = vslaGroup.DateRegistered;
                currentVsla.DateLinked = vslaGroup.DateLinked;
                currentVsla.PhysicalAddress = vslaGroup.PhysicalAddress;

                // region
                VslaRegion vslaRegion = new VslaRegion();
                vslaRegion.RegionId = Convert.ToInt32(RegionId);
                currentVsla.VslaRegion = vslaRegion;

                currentVsla.ContactPerson = vslaGroup.ContactPerson;
                currentVsla.PositionInVsla = vslaGroup.PositionInVsla;
                currentVsla.PhoneNumber = vslaGroup.PhoneNumber;

                TechnicalTrainer _trainer = new TechnicalTrainer();
                _trainer.Id = Convert.ToInt32(Id);
                currentVsla.CBT = _trainer;

                currentVsla.Status = Status_Id;
                currentVsla.GroupAccountNumber = vslaGroup.GroupAccountNumber;
                currentVsla.NumberOfCycles = Convert.ToInt32(vslaGroup.NumberOfCycles);
                if (currentVsla.VslaId > 0)
                {
                    updateResult = _vslaRepo.Update(currentVsla);
                }
            }
            if (updateResult)
            { // TRUE
                String logString = Convert.ToString(Session["Username"]) + " Edited VSLA with ID : " + Convert.ToString(VslaId);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
            else
            { // FALSE
                return View();
            }
        }

        // Helper function for getting vsla details for editing, including select options
        public VslaInformation vslaEditInformation(int id)
        {
            VslaRepo _vslaRepo = new VslaRepo();
            int _vslaId = Convert.ToInt32(id);
            Vsla vsla = _vslaRepo.FindVslaById(_vslaId);

            // Get a list of all vsla regions to populate in the dropdown list
            VslaRegionRepo _vslaRegionRepo = new VslaRegionRepo();
            List<VslaRegion> regions = new List<VslaRegion>();
            List<VslaRegion> regionsData = _vslaRegionRepo.findAllRegions();
            foreach (var region in regionsData)
            {
                regions.Add(new VslaRegion
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(regions, "RegionId", "RegionName", vsla.VslaRegion.RegionId);

            // Get the list of all technical trainers to populate in the dropdown list
            TechnicalTrainerRepo _technicalTrainerRepo = new TechnicalTrainerRepo();
            List<TechnicalTrainer> _trainers = new List<TechnicalTrainer>();
            List<TechnicalTrainer> trainerData = _technicalTrainerRepo.findAllTechnicalTrainers();
            foreach (var item in trainerData)
            {
                _trainers.Add(new TechnicalTrainer
                {
                    Id = item.Id,
                    Name = item.Name
                });
            }
            SelectList allTrainersList = new SelectList(_trainers, "Id", "Name", (int)vsla.CBT.Id);
            // Get the status type ie active/inactive
            StatusTypeRepo _statusTypeRepo = new StatusTypeRepo();
            List<StatusType> statusTypes = new List<StatusType>();
            List<StatusType> alltatusTypes = _statusTypeRepo.findAllStatusType();
            foreach (var statusType in alltatusTypes)
            {
                statusTypes.Add(new StatusType
                {
                    Status_Id = statusType.Status_Id,
                    CurrentStatus = statusType.CurrentStatus
                });
            }
            SelectList statusTypesList = new SelectList(statusTypes, "Status_Id", "CurrentStatus", vsla.Status);
            VslaInformation vslaData = new VslaInformation
            {
                VslaId = vsla.VslaId,
                VslaCode = vsla.VslaCode ?? "--",
                VslaName = vsla.VslaName ?? "--",
                VslaRegions = regionsList,
                DateRegistered = vsla.DateRegistered.HasValue ? vsla.DateRegistered : System.DateTime.Now,
                DateLinked = vsla.DateLinked.HasValue ? vsla.DateLinked : System.DateTime.Now,
                PhysicalAddress = vsla.PhysicalAddress ?? "--",
                VslaPhoneMsisdn = vsla.VslaPhoneMsisdn ?? "--",
                GpsLocation = vsla.GpsLocation ?? "--",
                ContactPerson = vsla.ContactPerson,
                PositionInVsla = vsla.PositionInVsla,
                PhoneNumber = vsla.PhoneNumber,
                AllTechnicalTrainers = allTrainersList,
                StatusType = statusTypesList,
                GroupAccountNumber = vsla.GroupAccountNumber,
                NumberOfCycles = Convert.ToString(vsla.NumberOfCycles)
            };
            return vslaData;
        }

        // Delete a particular VSLA from the system
        [HttpGet]
        public ActionResult DeleteVslaGroup(int id)
        {
            VslaInformation vslaData = findVslaDetails(id);
            return View(vslaData);
        }

        [HttpPost]
        public ActionResult DeleteVslaGroup(Vsla vslaGroup, int id)
        {
            Boolean deleteResult = false;
            int _vslaId = Convert.ToInt32(id);
            if (ModelState.IsValid && vslaGroup != null)
            {
                VslaRepo _vslaRepo = new VslaRepo();
                Vsla deleteVsla = _vslaRepo.FindVslaById(_vslaId);

                if (deleteVsla != null)
                {
                    deleteResult = _vslaRepo.Delete(deleteVsla);
                }
            }
            if (deleteResult)
            { // TRUE 
                String logString = Convert.ToString(Session["Username"]) + " Deleted VSLA with ID : " + Convert.ToString(id);
                activityLoggingSystem.logActivity(logString, 0);
                return RedirectToAction("VslaGroupInformation");
            }
            else
            { // FALSE 
                return View();
            }
        }

        // Export all VSLA data to a csv file
        public void Export_Vslas()
        {
            VslaRepo _vslaRepo = new VslaRepo();
            List<Vsla> vslaDetails = _vslaRepo.findAllVslas();
            List<VslaInformation> vslaList = new List<VslaInformation>();
            foreach (var item in vslaDetails)
            {
                vslaList.Add(new VslaInformation
                {
                    VslaId = item.VslaId,
                    VslaCode = item.VslaCode ?? "--",
                    VslaName = item.VslaName ?? "--",
                    RegionId = item.VslaRegion.RegionName,
                    DateRegistered = item.DateRegistered.HasValue ? item.DateRegistered : System.DateTime.Today,
                    DateLinked = item.DateLinked.HasValue ? item.DateLinked : System.DateTime.Today,
                    PhysicalAddress = item.PhysicalAddress ?? "--",
                    VslaPhoneMsisdn = item.VslaPhoneMsisdn ?? "--",
                    GpsLocation = item.GpsLocation ?? "--",
                    ContactPerson = item.ContactPerson ?? "--",
                    PositionInVsla = item.PositionInVsla ?? "--",
                    PhoneNumber = item.PhoneNumber ?? "--",
                    TechnicalTrainer = item.CBT.Name,
                    Status = item.Status.ToString() ?? "--"
                });
            }
            StringWriter stringWriter = new StringWriter();
            stringWriter.WriteLine("\"VSLA Code\",\"VSLA Name\",\"Region\",\"Date Registered\",\"Date Linked\",\"Physical Address\",\"Phone MSISDN\",\"Contact Person\",\"Position in VSLA\",\"Phone Number\",\"Technical Trainer\",\"Status\"");
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

        // View all meetings attached to a particular VSLA
        public ActionResult VslaGroupMeetings(int id)
        {
            AllVslaMeetingInformation totalMeetings = new AllVslaMeetingInformation();
            List<VslaMeetingInformation> singleMeeting = findMeetingDataByVslaId(id);

            VslaRepo _vslaRepo = new VslaRepo();
            Vsla _vsla = _vslaRepo.FindVslaById(Convert.ToInt32(id));

            totalMeetings.allVslaMeetings = singleMeeting;
            totalMeetings.vslaName = _vsla.VslaName;
            totalMeetings.vslaId = id;
            return View(totalMeetings);
        }

        // Helper method
        public List<VslaMeetingInformation> findMeetingDataByVslaId(int Id)
        {
            List<VslaMeetingInformation> allMeetings = new List<VslaMeetingInformation>();
            MeetingRepo _meetingRepo = null;
            int _vslaId = Convert.ToInt32(Id);
            try
            {
                _meetingRepo = new MeetingRepo();
                List<Meeting> meetingData = _meetingRepo.findMeetingByVslaId(_vslaId);
                foreach (var item in meetingData)
                {
                    allMeetings.Add(new VslaMeetingInformation
                    {
                        MeetingId = item.MeetingId,
                        cashFines = (long)item.CashFines,
                        meetingDate = item.MeetingDate,
                        membersPresent = int.Parse(item.CountOfMembersPresent.ToString()),
                        totalSavings = (long)item.SumOfSavings,
                        totalLoans = (long)item.SumOfLoanIssues,
                        totalLoanRepayment = (long)item.SumOfLoanRepayments
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
            return allMeetings;
        }

        // Export all VSLA meetings attached to a particular VSLA to CSV
        public void Export_VSLAMeetings(int id, string fileName)
        {
            List<VslaMeetingInformation> allMeetings = findMeetingDataByVslaId(id);
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
        // Get details for a particular meeting held on a partuclar day 
        //public ActionResult SingleMeetingDetails(int id)
        //{
        //    AllSingleMeetingInfo allInformation = new AllSingleMeetingInfo();
        //    List<SingleMeetingInfo> meetingsList = new List<SingleMeetingInfo>();

        //    // Get the date when the meeting was held
        //    MeetingRepo _meetingRepo = new MeetingRepo();
        //    int _meetingId = Convert.ToInt32(id);
        //    Meeting meeting = _meetingRepo.findMeetingByMeetingId(_meetingId);

        //    // Get the all the meeting details
        //    meetingsList = groupMeetingDetails(_meetingId);
        //    allInformation.allMeetingData = meetingsList;
        //    allInformation.meetingDate = meeting.MeetingDate;
        //    allInformation.vslaId = id;
        //    return View(allInformation);
        //}

        //// Helper class for getting information concerned with all meetings in the whole system
        //public List<SingleMeetingInfo> groupMeetingDetails(int id)
        //{

        //    List<SingleMeetingInfo> meetings = new List<SingleMeetingInfo>();
        //    var meeting = (from db_attendance in database.Attendances
        //                   join db_member in database.Members on db_attendance.MemberId equals db_member.MemberId
        //                   join db_savings in database.Savings on db_attendance.MemberId equals db_savings.MemberId
        //                   join db_loan in database.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
        //                   join db_fines in database.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
        //                   join db_loanRepayment in database.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
        //                   where (db_attendance.MeetingId == id && db_savings.MeetingId == id)
        //                   from db_loansAttendance in joinedLoansAttendance.DefaultIfEmpty()
        //                   from db_finesAttendance in joinedFinesAttendance.DefaultIfEmpty()
        //                   from db_repaymentAttendance in joinedRepaymentAttendance.DefaultIfEmpty()
        //                   select new
        //                   {
        //                       db_attendance,
        //                       db_member,
        //                       db_savings,
        //                       loanNo = (db_loansAttendance.LoanId == null) ? 00 : db_loansAttendance.LoanNo,
        //                       loanAmount = (db_loansAttendance.PrincipalAmount == null) ? (decimal)0.00 : db_loansAttendance.PrincipalAmount,
        //                       amountInFines = (db_finesAttendance.Amount == null) ? (decimal)0.00 : db_finesAttendance.Amount,
        //                       loanRepaymentAmount = (db_repaymentAttendance.Amount == null) ? (decimal)0.00 : db_repaymentAttendance.Amount,
        //                       remainingBalanceOnLoan = (db_repaymentAttendance.BalanceAfter == null) ? (decimal)0.00 : db_repaymentAttendance.BalanceAfter

        //                   });
        //    foreach (var item in meeting)
        //    {
        //        meetings.Add(new SingleMeetingInfo
        //        {
        //            Id = item.db_attendance.AttendanceId,
        //            memberId = item.db_member.MemberId,
        //            memberName = item.db_member.Surname + " " + item.db_member.OtherNames,
        //            isPresent = item.db_attendance.IsPresent.ToString(),
        //            amountSaved = (long)item.db_savings.Amount,
        //            loanNumber = (int)item.loanNo,
        //            principleAmount = (long)item.loanAmount,
        //            finedAmount = (long)item.amountInFines,
        //            loanRepaymentAmount = (long)item.loanRepaymentAmount,
        //            remainingBalanceOnLoan = (long)item.remainingBalanceOnLoan,

        //        });
        //    }
        //    return meetings;
        //}

        ///**
        // * Export details of a single meeting to a csv file
        // * */
        //public void Export_MeetingDetails(int id)
        //{
        //    List<SingleMeetingInfo> meetings = new List<SingleMeetingInfo>();
        //    var meeting = (from db_attendance in database.Attendances
        //                   join db_member in database.Members on db_attendance.MemberId equals db_member.MemberId
        //                   join db_savings in database.Savings on db_attendance.MemberId equals db_savings.MemberId
        //                   join db_loan in database.LoanIssues on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loan.MeetingId, db_loan.MemberId } into joinedLoansAttendance
        //                   join db_fines in database.Fines on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_fines.MeetingId, db_fines.MemberId } into joinedFinesAttendance
        //                   join db_loanRepayment in database.LoanRepayments on new { db_attendance.MeetingId, db_attendance.MemberId } equals new { db_loanRepayment.MeetingId, db_loanRepayment.MemberId } into joinedRepaymentAttendance
        //                   where (db_attendance.MeetingId == id && db_savings.MeetingId == id)
        //                   from db_loansAttendance in joinedLoansAttendance.DefaultIfEmpty()
        //                   from db_finesAttendance in joinedFinesAttendance.DefaultIfEmpty()
        //                   from db_repaymentAttendance in joinedRepaymentAttendance.DefaultIfEmpty()
        //                   select new
        //                   {
        //                       db_attendance,
        //                       db_member,
        //                       db_savings,
        //                       loanNo = (db_loansAttendance.LoanId == null) ? 00 : db_loansAttendance.LoanNo,
        //                       loanAmount = (db_loansAttendance.PrincipalAmount == null) ? (decimal)0.00 : db_loansAttendance.PrincipalAmount,
        //                       amountInFines = (db_finesAttendance.Amount == null) ? (decimal)0.00 : db_finesAttendance.Amount,
        //                       loanRepaymentAmount = (db_repaymentAttendance.Amount == null) ? (decimal)0.00 : db_repaymentAttendance.Amount,
        //                       remainingBalanceOnLoan = (db_repaymentAttendance.BalanceAfter == null) ? (decimal)0.00 : db_repaymentAttendance.BalanceAfter
        //                   });
        //    foreach (var item in meeting)
        //    {
        //        meetings.Add(new SingleMeetingInfo
        //        {
        //            Id = item.db_attendance.AttendanceId,
        //            memberId = item.db_member.MemberId,
        //            memberName = item.db_member.Surname + " " + item.db_member.OtherNames,
        //            isPresent = item.db_attendance.IsPresent.ToString(),
        //            amountSaved = (long)item.db_savings.Amount,
        //            loanNumber = (int)item.loanNo,
        //            principleAmount = (long)item.loanAmount,
        //            finedAmount = (long)item.amountInFines,
        //            loanRepaymentAmount = (long)item.loanRepaymentAmount,
        //            remainingBalanceOnLoan = (long)item.remainingBalanceOnLoan

        //        });
        //    }
        //    StringWriter sw = new StringWriter();
        //    sw.WriteLine("\"Member Name\",\"Attendance\",\"Amount Saved\",\"Loan Number\",\"Loan Taken\",\"Fines\",\"Loan Cleared\",\"Loan Outstanding\"");
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", "attachment;filename=meeting_details.csv");
        //    Response.ContentType = "text/csv";

        //    foreach (var line in meetings)
        //    {
        //        sw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\"",
        //                                   line.memberName,
        //                                   line.isPresent,
        //                                   line.amountSaved,
        //                                   line.loanNumber,
        //                                   line.principleAmount,
        //                                   line.finedAmount,
        //                                   line.loanRepaymentAmount,
        //                                   line.remainingBalanceOnLoan
        //                                   ));
        //    }

        //    Response.Write(sw.ToString());
        //    Response.End();
        //}

        // View all members attached to a given vsla
        public ActionResult VslaGroupMembers(int id)
        {
            AllVslaMemberInformation memberData = new AllVslaMemberInformation();
            int _vslaId = Convert.ToInt32(id);
            List<VslaMembersInformation> membersList = getMembersData(_vslaId);

            // Get the name of vsla
            VslaRepo _vslaRepo = new VslaRepo();
            Vsla VslaData = _vslaRepo.FindVslaById(_vslaId);

            // Get the list of all members
            memberData.allVslaMembers = membersList;
            memberData.vslaName = VslaData.VslaName;
            memberData.vslaId = id; // passing the id of the vsla on whch members are attached
            return View(memberData);
        }

        // Export the list of members attached to a particular VSLA to a csv file
        public void Export_MembersDetails(int id, string fileName)
        {
            List<VslaMembersInformation> allMembers = getMembersData(Convert.ToInt32(id));
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

        // Helper method to get members attached to a particular VSLA Group
        public List<VslaMembersInformation> getMembersData(int id)
        {
            MemberRepo _memberRepo = new MemberRepo();
            List<VslaMembersInformation> allMembers = new List<VslaMembersInformation>();
            List<Member> membersList = _memberRepo.findAllMembersByVslaId(id);
            foreach (var item in membersList)
            {
                allMembers.Add(new VslaMembersInformation
                {
                    memberId = item.MemberId,
                    memberNumber = int.Parse(item.MemberNo.ToString()),
                    cyclesCompleted = int.Parse(item.CyclesCompleted.ToString()),
                    surname = item.Surname,
                    otherNames = item.OtherNames,
                    gender = item.Gender,
                    occupation = item.Occupation
                });
            }
            return allMembers;
        }

        // Get details for a particualar member attached to a particular vsla
        public ActionResult GroupMemberDetails(int id, int vslaId)
        {
            MemberRepo _memberRepo = new MemberRepo();
            Member memberDetails = _memberRepo.FindMemberById(Convert.ToInt32(id));

            VslaMembersInformation memberInfo = new VslaMembersInformation
            {
                memberNumber = int.Parse(memberDetails.MemberNo.ToString()),
                cyclesCompleted = int.Parse(memberDetails.CyclesCompleted.ToString()),
                surname = memberDetails.Surname,
                otherNames = memberDetails.OtherNames,
                gender = memberDetails.Gender,
                occupation = memberDetails.Occupation,
                dateArchived = memberDetails.DateArchived,
                DateOfBirth = memberDetails.DateOfBirth,
                isActive = memberDetails.IsActive.ToString(),
                isArchive = memberDetails.IsArchived.ToString(),
                phoneNumber = memberDetails.PhoneNo,
                vslaId = (int)vslaId
            };
            return View(memberInfo);
        }

        /**
         * ******** GENERATE REPORTS *********
         */

        // Get weekly meetings
        public List<WeeklyMeetingsData> queryWeeklyMeetings()
        {
            MeetingRepo meetingRepo = new MeetingRepo();
            List<WeeklyMeetingsData> meetingsData = new List<WeeklyMeetingsData>();
            string dateString = @"29/07/2014";
            DateTime startDate = Convert.ToDateTime(dateString, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            List<Meeting> weeklyMeetings = meetingRepo.findWeeklyMeetings(startDate);
            if (null != weeklyMeetings)
            {
                foreach (var item in weeklyMeetings)
                {
                    meetingsData.Add(new WeeklyMeetingsData
                    {
                        meetingId = item.MeetingId,
                        cashExpenses = (long)item.CashExpenses,
                        cashFines = (long)item.CashFines,
                        cashFromBank = (long)item.CashFromBank,
                        cashFromBox = (long)item.CashFromBox,
                        cashSavedBank = (long)item.CashSavedBank,
                        cashSavedBox = (long)item.CashSavedBox,
                        cashWelfare = (long)item.CashWelfare,
                        dateSent = item.DateSent,
                        meetingDate = item.MeetingDate,
                        countOfMembersPresent = (int)item.CountOfMembersPresent,
                        sumOfSavings = (long)item.SumOfSavings,
                        sumOfLoansIssued = (long)item.SumOfLoanIssues,
                        sumOfLoanRepayments = (long)item.SumOfLoanRepayments,
                        vslaName = item.VslaCycle.Vsla.VslaName,
                        vslaId = (int)item.VslaCycle.Vsla.VslaId,
                        VslaCode = item.VslaCycle.Vsla.VslaCode
                    });
                }
            }
            return meetingsData;
        }

        // Display weekly meetings summary report
        public ActionResult VslaReporting()
        {
            AllMeetingsData allMeetingsSummary = new AllMeetingsData();
            allMeetingsSummary.meetingsSummary = queryWeeklyMeetings();
            return View(allMeetingsSummary);
        }

        // Export weekly meetings summary to a csv file
        public void ExportWeeklySummary()
        {
            List<WeeklyMeetingsData> summary = queryWeeklyMeetings();
            StringWriter stringWriter = new StringWriter();
            stringWriter.WriteLine("\"VSLA Code\",\"VSLA Name\",\"Meeting Id\",\"Cash Expenses\",\"Cash Fines\",\"Cash From Bank\",\"Cash From Box\",\"Cash Saved Bank\",\"Cash Saved Box\",\"Date Sent\",\"Meeting Date \",\"Members Present\",\"Savings\",\"Loans Issued\",\"Loan Repayment\"");

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

        /**
         * Logout
         * Note : The user-activity logging function should be called
         * before logging FormAutentication.SignOut() ie before the user
         * session is destroyed
         **/
        public ActionResult Logout()
        {
            String logString = Convert.ToString(Session["Username"]) + " Logged out";
            activityLoggingSystem.logActivity(logString, 0);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}
