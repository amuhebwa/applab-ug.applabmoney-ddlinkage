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
namespace DigitizingDataAdminApp.Controllers
{
    public class DashboardController : Controller
    {
        ActivityLoggingSystem activityLoggingSystem;
        ledgerlinkEntities database;

        VslaRepo vslaRepo;
        MemberRepo memberRepo;
        MeetingRepo meetingRepo;
        AttendanceRepo attendanceRepo;
        DataSubmissionRepo submssionRepo;
        UserPermissionsRepo permissionsRepo;
        UserRepo userRepo;

        public DashboardController()
        {
            activityLoggingSystem = new ActivityLoggingSystem();
            database = new ledgerlinkEntities();

            userRepo = new UserRepo();
            vslaRepo = new VslaRepo();
            memberRepo = new MemberRepo();
            meetingRepo = new MeetingRepo();
            attendanceRepo = new AttendanceRepo();
            submssionRepo = new DataSubmissionRepo();
            permissionsRepo = new UserPermissionsRepo();


        }

        [Authorize]
        public ActionResult Dashboard()
        {
            if (Session["UserId"] != null)
            {
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
                int totalSubmissions = (int)submssionRepo.findTotalSubmissions();

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
            List<UserInformation> users = new List<UserInformation>();
            int sessionUserLevel = Convert.ToInt32(Session["UserLevel"]);
            string sessionUsername = Convert.ToString(Session["Username"]);

            // Session Level 1 : admin
            // Session Level 2 : user               
            List<DigitizingDataDomain.Model.Users> userDetails = null;

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
            List<UserPermission> permissions = new List<UserPermission>();
            permissions.Add(new UserPermission { Level_Id = 0, UserType = "- Select Access Level -" });

            List<DigitizingDataDomain.Model.UserPermissions> allPermissions = permissionsRepo.allUserPermissions();

            foreach (var permission in allPermissions)
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

        // Display all information for a particular user
        public ActionResult UserDetails(int id)
        {
            DigitizingDataDomain.Model.Users userDetails = userRepo.findUserDetails(id);
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
        public ActionResult AddUser(User user, int Level_Id)
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
                DigitizingDataDomain.Model.Users addUser = new DigitizingDataDomain.Model.Users();
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
                DigitizingDataDomain.Model.Users currentUser = _userRepo.findUserById(_userId);
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
            DigitizingDataDomain.Model.Users userDetails = userRepo.findUserDetails(id);
            // Get access levels
            List<UserPermission> permissions = new List<UserPermission>();
            List<DigitizingDataDomain.Model.UserPermissions> allPermissions = permissionsRepo.allUserPermissions();
            foreach (var permission in allPermissions)
            {
                permissions.Add(new UserPermission
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
            DigitizingDataDomain.Model.Users userDetails = userRepo.findUserDetails(id);
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
        public ActionResult DeleteUser(User user, int id)
        {
            Boolean deleteResult = false;
            String logString = string.Empty;
            if (ModelState.IsValid && user != null)
            {
                int _userId = Convert.ToInt32(user.Id);
                UserRepo _userRepo = new UserRepo();
                DigitizingDataDomain.Model.Users currentUser = _userRepo.findUserById(_userId);

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
            List<DigitizingDataDomain.Model.TechnicalTrainer> allTrainers = _technicalTrainerRepo.findAllTechnicalTrainers();

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
            List<DigitizingDataAdminApp.Models.VslaRegion> regionsList = new List<DigitizingDataAdminApp.Models.VslaRegion>();
            regionsList.Add(new DigitizingDataAdminApp.Models.VslaRegion()
            {
                RegionId = 0,
                RegionName = "-Select Region-"
            });
            VslaRegionRepo _vslaRegionRepo = new VslaRegionRepo();
            List<DigitizingDataDomain.Model.VslaRegion> allRegions = _vslaRegionRepo.findAllRegions();
            foreach (var region in allRegions)
            {
                regionsList.Add(new DigitizingDataAdminApp.Models.VslaRegion()
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
            List<DigitizingDataAdminApp.Models.StatusType> statusOptions = new List<DigitizingDataAdminApp.Models.StatusType>();

            statusOptions.Add(new DigitizingDataAdminApp.Models.StatusType()
            {
                Status_Id = 0,
                CurrentStatus = "-Select Status-"
            });

            StatusTypeRepo _statusTypeRepo = new StatusTypeRepo();
            List<DigitizingDataDomain.Model.StatusType> allStatusTypes = _statusTypeRepo.findAllStatusType();
            foreach (var status in allStatusTypes)
            {
                statusOptions.Add(new DigitizingDataAdminApp.Models.StatusType
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
            List<DigitizingDataAdminApp.Models.TechnicalTrainer> trainers = new List<DigitizingDataAdminApp.Models.TechnicalTrainer>();
            trainers.Add(new DigitizingDataAdminApp.Models.TechnicalTrainer { Id = 0, Name = "-Select Trainer" });
            TechnicalTrainerRepo _technicalTrinerRepo = new TechnicalTrainerRepo();
            List<DigitizingDataDomain.Model.TechnicalTrainer> allTainers = _technicalTrinerRepo.findAllTechnicalTrainers();
            foreach (var trainer in allTainers)
            {
                trainers.Add(new DigitizingDataAdminApp.Models.TechnicalTrainer
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
            DigitizingDataDomain.Model.TechnicalTrainer trainer = _technicalTrainerRepo.findParticularTrainer(id);
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

                DigitizingDataDomain.Model.TechnicalTrainer newTrainer = new DigitizingDataDomain.Model.TechnicalTrainer();

                string fullName = trainer.FirstName + " " + trainer.LastName;
                newTrainer.Name = Convert.ToString(fullName);
                newTrainer.FirstName = Convert.ToString(trainer.FirstName);
                newTrainer.LastName = Convert.ToString(trainer.LastName);
                // Region
                DigitizingDataDomain.Model.VslaRegion vslaRegion = new DigitizingDataDomain.Model.VslaRegion();
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
            DigitizingDataDomain.Model.TechnicalTrainer allInformation = _technicalTrainerRepo.findParticularTrainer(_trainerId);

            // Regions
            List<DigitizingDataAdminApp.Models.VslaRegion> allRegionsList = new List<DigitizingDataAdminApp.Models.VslaRegion>();
            VslaRegionRepo _vslaRegionRepo = new VslaRegionRepo();
            List<DigitizingDataDomain.Model.VslaRegion> databaseRegions = _vslaRegionRepo.findAllRegions();
            foreach (var region in databaseRegions)
            {
                allRegionsList.Add(new DigitizingDataAdminApp.Models.VslaRegion()
                {
                    RegionId = region.RegionId,
                    RegionName = region.RegionName
                });
            }
            SelectList regionsList = new SelectList(allRegionsList, "RegionId", "RegionName", allInformation.VslaRegion.RegionId);

            // Status types
            List<DigitizingDataAdminApp.Models.StatusType> statusOptions = new List<DigitizingDataAdminApp.Models.StatusType>();
            StatusTypeRepo _statusTypeRepo = new StatusTypeRepo();
            List<DigitizingDataDomain.Model.StatusType> databaseStatuses = _statusTypeRepo.findAllStatusType();
            foreach (var status in databaseStatuses)
            {
                statusOptions.Add(new DigitizingDataAdminApp.Models.StatusType
                {
                    Status_Id = status.Status_Id,
                    CurrentStatus = status.CurrentStatus
                });
            }
            SelectList statusTypes = new SelectList(statusOptions, "Status_Id", "CurrentStatus", allInformation.Status);

            // Create a cbt object
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
            DigitizingDataDomain.Model.TechnicalTrainer currentTrainer = _technicalTrainerRepo.findParticularTrainer(_trainerId);
            if (currentTrainer != null)
            {
                string fullname = trainer.FirstName + " " + trainer.LastName;
                currentTrainer.Name = Convert.ToString(fullname);
                currentTrainer.FirstName = Convert.ToString(trainer.FirstName);
                currentTrainer.LastName = Convert.ToString(trainer.LastName);

                // region id
                DigitizingDataDomain.Model.VslaRegion vslaRegion = new DigitizingDataDomain.Model.VslaRegion();
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
                DigitizingDataDomain.Model.TechnicalTrainer currentTrainer = _technicalTrainerRepo.findParticularTrainer(_trainerId);
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
            List<DigitizingDataDomain.Model.Vsla> vslaDetails = _vslaRepo.findAllVslas();
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
                    TechnicalTrainer = "--",
                    Status = item.Status.ToString() ?? "--"
                });
            }
            return vslaData;
        }

        // Get the group support modules that have been provided to the group by technical trainers
        public List<GroupSupportInfo> getSupportType()
        {
            GroupSupportRepo _groupSupportRepo = new GroupSupportRepo();
            List<DigitizingDataDomain.Model.GroupSupport> support = _groupSupportRepo.findAllGroupSupport();
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
















        /**
         * Display information for a particular VSLA
         * */
        //public ActionResult VslaGroupDetails(int id)
        //{
        //    var vsla_info = (from tb_vsla in database.Vslas
        //                     join tb_cbt in database.TechnicalTrainers on tb_vsla.CBT equals tb_cbt.Id
        //                     join tb_regions in database.VslaRegions on tb_vsla.RegionId equals tb_regions.RegionId
        //                     join tb_status in database.StatusTypes on tb_vsla.Status equals tb_status.Status_Id
        //                     where tb_vsla.VslaId == id
        //                     select new { db_vsla = tb_vsla, db_cbt = tb_cbt, db_regions = tb_regions, db_status = tb_status }).Single();

        //    VslaInformation vslaData = new VslaInformation
        //    {
        //        VslaId = vsla_info.db_vsla.VslaId,
        //        VslaCode = vsla_info.db_vsla.VslaCode ?? "--",
        //        VslaName = vsla_info.db_vsla.VslaName ?? "--",
        //        RegionId = vsla_info.db_regions.RegionName,
        //        DateRegistered = vsla_info.db_vsla.DateRegistered,
        //        DateLinked = vsla_info.db_vsla.DateLinked,
        //        PhysicalAddress = vsla_info.db_vsla.PhysicalAddress ?? "--",
        //        VslaPhoneMsisdn = vsla_info.db_vsla.VslaPhoneMsisdn ?? "--",
        //        GpsLocation = vsla_info.db_vsla.GpsLocation ?? "--",
        //        ContactPerson = vsla_info.db_vsla.ContactPerson ?? "--",
        //        PositionInVsla = vsla_info.db_vsla.PositionInVsla,
        //        PhoneNumber = vsla_info.db_vsla.PhoneNumber ?? "--",
        //        TechnicalTrainer = vsla_info.db_cbt.Name ?? "--",
        //        Status = vsla_info.db_status.CurrentStatus ?? "--",
        //        GroupAccountNumber = "A/C " + vsla_info.db_vsla.GroupAccountNumber ?? "--"
        //    };
        //    return View(vslaData);
        //}
        ///**
        // * Edit a given VSLA
        // * */
        //[HttpGet]
        ////public ActionResult EditVslaGroup(int id)
        ////{
        ////    VslaInformation vslaData = getGroupEditInformation(id);
        ////    return View(vslaData);
        ////}
        ///**
        // * Edit details for a particular VSLA
        // * */
        //[HttpPost]
        //public ActionResult EditVslaGroup(VslaInformation vslaGroup, int VslaId, int Id, int RegionId, int Status_Id)
        //{
        //    if (string.IsNullOrEmpty(vslaGroup.VslaName))
        //    {
        //        ModelState.AddModelError("VslaName", "Please add a valid VSLA Name");
        //    }
        //    else if (RegionId == 0)
        //    {
        //        ModelState.AddModelError("RegionName", "Please select a region");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.DateRegistered.ToString()))
        //    {
        //        ModelState.AddModelError("DateRegistered", "Please Enter Valid Date Registered");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.DateLinked.ToString()))
        //    {
        //        ModelState.AddModelError("DateLinked", "ADate Linked cannot be null");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.PhysicalAddress))
        //    {
        //        ModelState.AddModelError("PhysicalAddress", " Please add a physical address");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.VslaPhoneMsisdn))
        //    {
        //        ModelState.AddModelError("VslaPhoneMsisdn", "Phone MSISDN cannot be empty");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.GpsLocation))
        //    {
        //        ModelState.AddModelError("GpsLocation", "Your GPS Location cannot be empty");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.ContactPerson))
        //    {
        //        ModelState.AddModelError("ContactPerson", "Please add a valid contact person");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.PositionInVsla))
        //    {
        //        ModelState.AddModelError("PositionInVsla", "Position cannot be left Empty");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.PhoneNumber))
        //    {
        //        ModelState.AddModelError("PhoneNumber", "Contact Person's Number is Empty");
        //    }
        //    else if (Id == 0)
        //    {
        //        ModelState.AddModelError("CbtModel", "Select Responsible CBT");
        //    }
        //    else if (Status_Id == 0)
        //    {
        //        ModelState.AddModelError("StatusType", "Select Status Type");
        //    }
        //    else if (string.IsNullOrEmpty(vslaGroup.GroupAccountNumber))
        //    {
        //        ModelState.AddModelError("GroupAccountNumber", "Add Group Account Number");
        //    }
        //    else
        //    {
        //        var query = database.Vslas.Find(VslaId);
        //        query.VslaName = vslaGroup.VslaName;
        //        query.VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn;
        //        query.GpsLocation = vslaGroup.GpsLocation;
        //        query.DateRegistered = vslaGroup.DateRegistered;
        //        query.DateLinked = vslaGroup.DateLinked;
        //        query.RegionId = (int)RegionId;
        //        query.ContactPerson = vslaGroup.ContactPerson;
        //        query.PositionInVsla = vslaGroup.PositionInVsla;
        //        query.PhoneNumber = vslaGroup.PhoneNumber;
        //        query.CBT = Id;
        //        query.Status = Status_Id;
        //        query.GroupAccountNumber = vslaGroup.GroupAccountNumber;
        //        database.SaveChanges();
        //        String logString = Convert.ToString(Session["Username"]) + " Edited VSLA with ID : " + Convert.ToString(VslaId);
        //        activityLoggingSystem.logActivity(logString, 0);
        //        return RedirectToAction("VslaGroupInformation");
        //    }
        //    // If one of the validations fails, reload the form and repopulate the dropdown list
        //    VslaInformation vslaData = getGroupEditInformation(VslaId);
        //    return View(vslaData);

        //}
        /**
         * Add a new Village savings and lending association (VSLA) to the system
         **/
        [HttpPost]
        //public ActionResult AddVslaGroup(Vsla vslaGroup, int RegionId, int Id, int Status_Id)
        //{
        //    if (RegionId == 0)
        //    {
        //        ModelState.AddModelError("RegionName", "Please select a region");
        //        return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
        //    }
        //    else if (Id == 0)
        //    {
        //        ModelState.AddModelError("TechnicalTrainer", "Select Trainer in charge");
        //        return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
        //    }
        //    else if (Status_Id == 0)
        //    {
        //        ModelState.AddModelError("Status", "Select the VSLA status");
        //        return Redirect(Url.Action("VslaGroupInformation") + "#addnewgroup");
        //    }
        //    else
        //    { //! All fields are valid
        //        /** Generate he VSLA code based on new  VSLA to be created abd the current year(yyyy) */
        //        int getMaxId = database.Vslas.Max(x => x.VslaId) + 1;
        //        string getYear = DateTime.Now.Year.ToString().Substring(2);
        //        string generatedVslaCode = "VS" + getYear + getMaxId.ToString();

        //        Vsla newVsla = new Vsla
        //        {
        //            VslaCode = generatedVslaCode,
        //            VslaName = vslaGroup.VslaName,
        //            RegionId = RegionId,
        //            DateRegistered = vslaGroup.DateRegistered.HasValue ? vslaGroup.DateRegistered : System.DateTime.Now,
        //            DateLinked = vslaGroup.DateLinked.HasValue ? vslaGroup.DateLinked : System.DateTime.Now,
        //            PhysicalAddress = vslaGroup.PhysicalAddress ?? "--",
        //            VslaPhoneMsisdn = vslaGroup.VslaPhoneMsisdn ?? "--",
        //            GpsLocation = vslaGroup.GpsLocation ?? "--",
        //            ContactPerson = vslaGroup.ContactPerson,
        //            PositionInVsla = vslaGroup.PositionInVsla,
        //            PhoneNumber = vslaGroup.PhoneNumber,
        //            CBT = Id,
        //            Status = Status_Id,
        //            GroupAccountNumber = vslaGroup.GroupAccountNumber
        //        };
        //        database.Vslas.Add(newVsla);
        //        database.SaveChanges();
        //        String logString = Convert.ToString(Session["Username"]) + " Added VSLA with ID : " + generatedVslaCode;
        //        activityLoggingSystem.logActivity(logString, 0);
        //        return RedirectToAction("VslaGroupInformation");
        //    }
        //}
        /**
         * Get the options for re-populating the edit VSLA form, in case of the forms fails
         **/
        //public VslaInformation getGroupEditInformation(int id)
        //{
        //    var vsla = (from tb_vsla in database.Vslas
        //                join tb_cbt in database.TechnicalTrainers on tb_vsla.CBT equals tb_cbt.Id
        //                where tb_vsla.VslaId == id
        //                select new { db_vsla = tb_vsla, db_cbt = tb_cbt }).FirstOrDefault();

        //    // Get a list of all vsla regions to populate in the dropdown list
        //    List<VslaRegion> regions = new List<VslaRegion>();
        //    var databaseRegions = database.VslaRegions.OrderBy(a => a.RegionName).ToList();
        //    foreach (var region in databaseRegions)
        //    {
        //        regions.Add(new VslaRegion
        //        {
        //            RegionId = region.RegionId,
        //            RegionName = region.RegionName
        //        });
        //    }
        //    SelectList allRegions = new SelectList(regions, "RegionId", "RegionName", vsla.db_vsla.RegionId);
        //    // Get the list of all cbts to populate in the dropdown list
        //    List<TechnicalTrainer> cbts = new List<TechnicalTrainer>();
        //    var database_cbts = database.TechnicalTrainers.OrderBy(a => a.Name).ToList();
        //    foreach (var cbt in database_cbts)
        //    {
        //        cbts.Add(new TechnicalTrainer
        //        {
        //            Id = cbt.Id,
        //            Name = cbt.Name
        //        });
        //    }
        //    SelectList allTrainers = new SelectList(cbts, "Id", "Name", (int)vsla.db_vsla.CBT);

        //    // Get the status type ie active/inactive
        //    List<StatusType> statusTypes = new List<StatusType>();
        //    var databaseStatusTypes = database.StatusTypes.OrderBy(a => a.Status_Id).ToList();
        //    foreach (var statusType in databaseStatusTypes)
        //    {
        //        statusTypes.Add(new StatusType
        //        {
        //            Status_Id = statusType.Status_Id,
        //            CurrentStatus = statusType.CurrentStatus
        //        });
        //    }
        //    SelectList statusTypesList = new SelectList(statusTypes, "Status_Id", "CurrentStatus", vsla.db_cbt.Status);

        //    VslaInformation vslaData = new VslaInformation
        //    {
        //        VslaId = vsla.db_vsla.VslaId,
        //        VslaCode = vsla.db_vsla.VslaCode ?? "--",
        //        VslaName = vsla.db_vsla.VslaName ?? "--",
        //        VslaRegions = allRegions,
        //        DateRegistered = vsla.db_vsla.DateRegistered.HasValue ? vsla.db_vsla.DateRegistered : System.DateTime.Now,
        //        DateLinked = vsla.db_vsla.DateLinked.HasValue ? vsla.db_vsla.DateLinked : System.DateTime.Now,
        //        PhysicalAddress = vsla.db_vsla.PhysicalAddress ?? "--",
        //        VslaPhoneMsisdn = vsla.db_vsla.VslaPhoneMsisdn ?? "--",
        //        GpsLocation = vsla.db_vsla.GpsLocation ?? "--",
        //        ContactPerson = vsla.db_vsla.ContactPerson,
        //        PositionInVsla = vsla.db_vsla.PositionInVsla,
        //        PhoneNumber = vsla.db_vsla.PhoneNumber,
        //        AllTechnicalTrainers = allTrainers,
        //        StatusType = statusTypesList,
        //        GroupAccountNumber = vsla.db_vsla.GroupAccountNumber
        //    };
        //    return vslaData;
        //}
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
        //public ActionResult DeleteVslaGroup(Vsla vslaGroup, int id)
        //{
        //    if (ModelState.IsValid && vslaGroup != null)
        //    {
        //        vslaGroup.VslaId = id;
        //        database.Vslas.Attach(vslaGroup);
        //        database.Vslas.Remove(vslaGroup);
        //        database.SaveChanges();
        //        String logString = Convert.ToString(Session["Username"]) + " Deleted VSLA with ID : " + Convert.ToString(id);
        //        activityLoggingSystem.logActivity(logString, 0);
        //        return RedirectToAction("VslaGroupInformation");
        //    }
        //    return View();
        //}
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
         * ******** GENERATE REPORTS *********
         */

        // Get weekly meetings
        public List<WeeklyMeetingsData> queryWeeklyMeetings()
        {
            List<WeeklyMeetingsData> meetingsData = new List<WeeklyMeetingsData>();
            string dateString = @"29/07/2014";
            DateTime startDate = Convert.ToDateTime(dateString, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            List<DigitizingDataDomain.Model.Meeting> weeklyMeetings = meetingRepo.findWeeklyMeetings(startDate);
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
