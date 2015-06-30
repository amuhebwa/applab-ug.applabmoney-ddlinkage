using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web.Script.Serialization;

namespace DigitizingDataAdminWebService
{
    public class DigitizingDataRestfulWebService : IDigitizingDataRestfulWebService
    {
        /**
         * Login for the cbt
         */
        Constants constants = new Constants();
        public CBTLoginDetails loginCBT(String Username, String PassKey)
        {
            PasswordHash passwordHashing = new PasswordHash();
            string passkey = passwordHashing.hashedPassword(PassKey);

            ledgerlinkEntities database = new ledgerlinkEntities();
            var login = (from table_cbt in database.Cbt_info
                         where table_cbt.Username == Username && table_cbt.Passkey == passkey
                         select new { table_cbt }).SingleOrDefault();

            CBTLoginDetails loginResult = new CBTLoginDetails();


            if (login != null)
            {
                loginResult.CbtId = login.table_cbt.Id;
                loginResult.result = constants.successful;
                loginResult.Username = login.table_cbt.Username;
            }
            else
            {
                loginResult.CbtId = -1;
                loginResult.result = constants.unsuccessful;
                loginResult.Username = null;
            }
            return loginResult;
        }
        /**
         * Search for a given VSLA
         */
        public List<VslaDetails> searchForVsla(string VslaName)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vslaQuery = (from table_vsla in database.Vslas
                             join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                             join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                             where table_vsla.VslaName.Contains(VslaName)
                             select new { table_vsla, table_regions, table_cbt, table_status });

            List<VslaDetails> results = new List<VslaDetails>();
            foreach (var vsla in vslaQuery)
            {
                results.Add(new VslaDetails()
                {
                    VslaId = vsla.table_vsla.VslaId,
                    VslaCode = vsla.table_vsla.VslaCode,
                    VslaName = vsla.table_vsla.VslaName,
                    VslaPhoneMsisdn = string.IsNullOrEmpty(vsla.table_vsla.VslaPhoneMsisdn) ? "---" : vsla.table_vsla.VslaPhoneMsisdn.ToString(),
                    PhysicalAddress = string.IsNullOrEmpty(vsla.table_vsla.PhysicalAddress) ? "---" : vsla.table_vsla.PhysicalAddress.ToString(),
                    GpsLocation = string.IsNullOrEmpty(vsla.table_vsla.GpsLocation) ? "---" : vsla.table_vsla.GpsLocation.ToString(),
                    DateRegistered = string.IsNullOrEmpty(vsla.table_vsla.DateRegistered.ToString()) ? "---" : vsla.table_vsla.DateRegistered.ToString(),
                    DateLinked = string.IsNullOrEmpty(vsla.table_vsla.DateLinked.ToString()) ? "---" : vsla.table_vsla.DateLinked.ToString(),
                    RegionName = vsla.table_regions.RegionName,
                    ContactPerson = string.IsNullOrEmpty(vsla.table_vsla.ContactPerson) ? "---" : vsla.table_vsla.ContactPerson,
                    PositionInVsla = string.IsNullOrEmpty(vsla.table_vsla.PositionInVsla) ? "---" : vsla.table_vsla.PositionInVsla,
                    PhoneNumber = string.IsNullOrEmpty(vsla.table_vsla.PhoneNumber) ? "---" : vsla.table_vsla.PhoneNumber,
                    CbtName = vsla.table_cbt.Name,
                    Status = vsla.table_status.CurrentStatus
                });
            }

            return results;

        }

        /**
         * Create  a new VSLA
         */
        public string createNewVsla(string groupInfo, string phoneInfo, string locationInfo)
        {
            return "";
        }
        /**
         * Sample Testing method for creating  
         */
        public string createVslaTestMethod(Stream jsonStream)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            int getMaxId = database.Vslas.Max(x => x.VslaId) + 1;
            string getYear = DateTime.Now.Year.ToString().Substring(2);
            string generatedVslaCode = "VS" + getYear + getMaxId.ToString();

            string _sampleString = "{\"VslaName\": \"Admin Web services Test\",\"VslaPhoneMsisdn\": \"123456789\",\"PhysicalAddress\": \"Near the mango tree\",\"GpsLocation\": \"4567.566, 746666\",\"DateRegistered\": \"2015-06-24T00:00:00.000Z\",\"DateLinked\": \"2015-07-12T00:00:00.000Z\",\"RegionId\": 8,\"ContactPerson\": \"Julius Matovu\",\"PositionInVsla\": \"Mobile Secretary\",\"PhoneNumber\": \"0774561760\",\"CBT\": 8,\"Status\": 2}";
            VslaDetails request = JsonConvert.DeserializeObject<VslaDetails>(_sampleString);
            //if (null != request)
            //{
            //    Vsla newVsla = new Vsla
            //    {
            //        VslaCode = generatedVslaCode,
            //        VslaName = request.VslaName,
            //        RegionId = 9,
            //        DateRegistered = Convert.ToDateTime(request.DateRegistered),
            //        DateLinked = Convert.ToDateTime(request.DateLinked),
            //        PhysicalAddress = request.PhysicalAddress ?? "--",
            //        VslaPhoneMsisdn = request.VslaPhoneMsisdn ?? "--",
            //        GpsLocation = request.GpsLocation ?? "--",
            //        ContactPerson = request.ContactPerson,
            //        PositionInVsla = request.PositionInVsla,
            //        PhoneNumber = request.PhoneNumber,
            //        CBT = 9,
            //        Status = 2

            //    };
            //    database.Vslas.Add(newVsla);
            //    database.SaveChanges();
            //    // string result = "VSLA Name is " + request.VslaName;
            //    return "Done. New group has been added to the DB";
            //}
            //else
            //{
            //    string result = "Group could not be added to the DB";
            //    return result;
            //}
            StreamReader reader = new StreamReader(jsonStream);
            string data = reader.ReadToEnd();
            if (string.IsNullOrEmpty(data))
            {
                return "Nothing to show";
            }
            else {
                return data;
            }
        }

        /**
         * Get all users
         */
        public List<UsersDetails> getRegisteredUsers()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            List<UsersDetails> results = new List<UsersDetails>();
            foreach (var user in database.Users)
            {
                results.Add(new UsersDetails()
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Fullname = user.Fullname,
                    // Password = user.Password,
                    Email = user.Email,
                    DateCreated = user.DateCreated,
                    UserLevel = user.UserLevel
                });
            }
            return results;
        }
        /**
         * Get the list of all VSLAs
         */
        public List<VslaDetails> getAllVslas()
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vslaQuery = (from table_vsla in database.Vslas
                             join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                             join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                             select new { table_vsla, table_regions, table_cbt, table_status });
            if (vslaQuery != null)
            {
                List<VslaDetails> results = new List<VslaDetails>();
                foreach (var vsla in vslaQuery)
                {
                    results.Add(new VslaDetails()
                    {
                        VslaId = vsla.table_vsla.VslaId,
                        VslaCode = vsla.table_vsla.VslaCode,
                        VslaName = vsla.table_vsla.VslaName,
                        VslaPhoneMsisdn = string.IsNullOrEmpty(vsla.table_vsla.VslaPhoneMsisdn) ? "---" : vsla.table_vsla.VslaPhoneMsisdn.ToString(),
                        PhysicalAddress = string.IsNullOrEmpty(vsla.table_vsla.PhysicalAddress) ? "---" : vsla.table_vsla.PhysicalAddress.ToString(),
                        GpsLocation = string.IsNullOrEmpty(vsla.table_vsla.GpsLocation) ? "---" : vsla.table_vsla.GpsLocation.ToString(),
                        DateRegistered = string.IsNullOrEmpty(vsla.table_vsla.DateRegistered.ToString()) ? "---" : vsla.table_vsla.DateRegistered.ToString(),
                        DateLinked = string.IsNullOrEmpty(vsla.table_vsla.DateLinked.ToString()) ? "---" : vsla.table_vsla.DateLinked.ToString(),
                        RegionName = vsla.table_regions.RegionName,
                        ContactPerson = string.IsNullOrEmpty(vsla.table_vsla.ContactPerson) ? "---" : vsla.table_vsla.ContactPerson,
                        PositionInVsla = string.IsNullOrEmpty(vsla.table_vsla.PositionInVsla) ? "---" : vsla.table_vsla.PositionInVsla,
                        PhoneNumber = string.IsNullOrEmpty(vsla.table_vsla.PhoneNumber) ? "---" : vsla.table_vsla.PhoneNumber,
                        CbtName = vsla.table_cbt.Name,
                        Status = vsla.table_status.CurrentStatus
                    });
                }

                return results;
            }
            else
            {
                return null;
            }
        }
        /**
         * Get VSLAs attached to a particular CBT
         */
        public List<VslaDetails> getVslaForParticularCBT(string id)
        {
            int cbtId = Convert.ToInt32(id);
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vslaQuery = (from table_vsla in database.Vslas
                             join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                             join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                             join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                             where table_vsla.CBT == cbtId
                             select new { table_vsla, table_regions, table_cbt, table_status });
            if (vslaQuery != null)
            {
                List<VslaDetails> results = new List<VslaDetails>();
                foreach (var vsla in vslaQuery)
                {
                    results.Add(new VslaDetails()
                    {
                        VslaId = vsla.table_vsla.VslaId,
                        VslaCode = vsla.table_vsla.VslaCode,
                        VslaName = vsla.table_vsla.VslaName,
                        VslaPhoneMsisdn = string.IsNullOrEmpty(vsla.table_vsla.VslaPhoneMsisdn) ? "---" : vsla.table_vsla.VslaPhoneMsisdn.ToString(),
                        PhysicalAddress = string.IsNullOrEmpty(vsla.table_vsla.PhysicalAddress) ? "---" : vsla.table_vsla.PhysicalAddress.ToString(),
                        GpsLocation = string.IsNullOrEmpty(vsla.table_vsla.GpsLocation) ? "---" : vsla.table_vsla.GpsLocation.ToString(),
                        DateRegistered = string.IsNullOrEmpty(vsla.table_vsla.DateRegistered.ToString()) ? "---" : vsla.table_vsla.DateRegistered.ToString(),
                        DateLinked = string.IsNullOrEmpty(vsla.table_vsla.DateLinked.ToString()) ? "---" : vsla.table_vsla.DateLinked.ToString(),
                        RegionName = vsla.table_regions.RegionName,
                        ContactPerson = string.IsNullOrEmpty(vsla.table_vsla.ContactPerson) ? "---" : vsla.table_vsla.ContactPerson,
                        PositionInVsla = string.IsNullOrEmpty(vsla.table_vsla.PositionInVsla) ? "---" : vsla.table_vsla.PositionInVsla,
                        PhoneNumber = string.IsNullOrEmpty(vsla.table_vsla.PhoneNumber) ? "---" : vsla.table_vsla.PhoneNumber,
                        CbtName = vsla.table_cbt.Name,
                        Status = vsla.table_status.CurrentStatus
                    });
                }
                return results;
            }
            else
            {
                return null;
            }

        }
        //Get all information attached to a single vsla
        public VslaDetails getSingleVslaDetails(string VslaId)
        {
            int _VslaId = Convert.ToInt32(VslaId);
            ledgerlinkEntities database = new ledgerlinkEntities();
            var vslaData = (from table_vsla in database.Vslas
                            join table_regions in database.VslaRegions on table_vsla.RegionId equals table_regions.RegionId
                            join table_cbt in database.Cbt_info on table_vsla.CBT equals table_cbt.Id
                            join table_status in database.StatusTypes on table_vsla.Status equals table_status.Status_Id
                            where table_vsla.VslaId == _VslaId
                            select new { table_vsla, table_regions, table_cbt, table_status }).SingleOrDefault();
            if (vslaData != null)
            {
                VslaDetails results = new VslaDetails
                {
                    VslaId = vslaData.table_vsla.VslaId,
                    VslaCode = vslaData.table_vsla.VslaCode,
                    VslaName = vslaData.table_vsla.VslaName,
                    VslaPhoneMsisdn = string.IsNullOrEmpty(vslaData.table_vsla.VslaPhoneMsisdn) ? "---" : vslaData.table_vsla.VslaPhoneMsisdn.ToString(),
                    PhysicalAddress = string.IsNullOrEmpty(vslaData.table_vsla.PhysicalAddress) ? "---" : vslaData.table_vsla.PhysicalAddress.ToString(),
                    GpsLocation = string.IsNullOrEmpty(vslaData.table_vsla.GpsLocation) ? "---" : vslaData.table_vsla.GpsLocation.ToString(),
                    DateRegistered = string.IsNullOrEmpty(vslaData.table_vsla.DateRegistered.ToString()) ? "---" : vslaData.table_vsla.DateRegistered.ToString(),
                    DateLinked = string.IsNullOrEmpty(vslaData.table_vsla.DateLinked.ToString()) ? "---" : vslaData.table_vsla.DateLinked.ToString(),
                    RegionName = vslaData.table_regions.RegionName,
                    ContactPerson = string.IsNullOrEmpty(vslaData.table_vsla.ContactPerson) ? "---" : vslaData.table_vsla.ContactPerson,
                    PositionInVsla = string.IsNullOrEmpty(vslaData.table_vsla.PositionInVsla) ? "---" : vslaData.table_vsla.PositionInVsla,
                    PhoneNumber = string.IsNullOrEmpty(vslaData.table_vsla.PhoneNumber) ? "---" : vslaData.table_vsla.PhoneNumber,
                    CbtName = vslaData.table_cbt.Name,
                    Status = vslaData.table_status.CurrentStatus


                };
                return results;
            }
            else
            {
                return null;
            }

        }
    }
}
