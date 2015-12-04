using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
// using System.ServiceModel.Web;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web.Script.Serialization;
using DigitizingDataBizLayer.Repositories;
using DigitizingDataDomain.Model;

namespace DigitizingDataAdminWebService
{
    public class DigitizingDataRestfulWebService : IDigitizingDataRestfulWebService
    {
        // Validate Technical Trainer's username and passkey
        public TechnicalTrainerCtx validateTrainer(string username, string passkey)
        {
            TechnicalTrainerCtx tt = new TechnicalTrainerCtx();
            Cbt_infoRepo trainerRepo = new Cbt_infoRepo();
            var trainerData = trainerRepo.checkIfTrainerExists(username, passkey);
            if (trainerData != null)
            {
                tt.userName = trainerData.Username;
                tt.TrainerId = trainerData.Id;
                tt.resultId = 1;
            }
            else
            {
                tt.resultId = 0;
            }
            return tt;
        }

        // Search for a given vsla
        public List<VslaDetails> searchVsla(string vslaName)
        {
            VslaRepo vslaRepo = new VslaRepo();
            List<DigitizingDataDomain.Model.Vsla> vslaData = vslaRepo.FindVslaByName(vslaName);
            List<VslaDetails> result = new List<VslaDetails>();
            if (vslaData != null)
            {
                foreach (var data in vslaData)
                {
                    result.Add(new VslaDetails()
                    {
                        VslaId = data.VslaId,
                        VslaCode = data.VslaCode,
                        VslaName = data.VslaName,
                        grpPhoneNumber = data.GroupAccountNumber,
                        PhysicalAddress = data.PhysicalAddress,
                        GpsLocation = data.GpsLocation,
                        DateRegistered = string.IsNullOrEmpty(data.DateRegistered.ToString()) ? "--" : data.DateRegistered.ToString(),
                        DateLinked = string.IsNullOrEmpty(data.DateLinked.ToString()) ? "--" : data.DateLinked.ToString(),
                        RegionName = data.VslaRegion.RegionName,
                        representativeName = data.ContactPerson,
                        representativePosition = data.PositionInVsla,
                        repPhoneNumber = data.PhoneNumber,
                        tTrainerId = data.CBT.Id,
                        tTrainerName = data.CBT.Username,
                        GroupAccountNumber = data.GroupAccountNumber,
                    });
                }
            }
            return result;
        }

        // Get all details for a given vsla
        public VslaDetails vslaInformation(string vslaId)
        {
            int id = Convert.ToInt32(vslaId);
            VslaRepo vslaRepo = new VslaRepo();
            DigitizingDataDomain.Model.Vsla vslaData = vslaRepo.FindVslaById(id);

            if (vslaData != null)
            {

                VslaDetails result = new VslaDetails()
                 {
                     VslaId = vslaData.VslaId,
                     VslaCode = vslaData.VslaCode,
                     VslaName = vslaData.VslaName,
                     grpPhoneNumber = vslaData.GroupAccountNumber,
                     PhysicalAddress = vslaData.PhysicalAddress,
                     GpsLocation = vslaData.GpsLocation,
                     DateRegistered = string.IsNullOrEmpty(vslaData.DateRegistered.ToString()) ? "--" : vslaData.DateRegistered.ToString(),
                     DateLinked = string.IsNullOrEmpty(vslaData.DateLinked.ToString()) ? "--" : vslaData.DateLinked.ToString(),
                     RegionName = vslaData.VslaRegion.RegionName,
                     representativeName = vslaData.ContactPerson,
                     representativePosition = vslaData.PositionInVsla,
                     repPhoneNumber = vslaData.PhoneNumber,
                     tTrainerId = vslaData.CBT.Id,
                     tTrainerName = vslaData.CBT.Username,
                     GroupAccountNumber = vslaData.GroupAccountNumber,
                 };
                return result;
            }
            return null;
        }

        // add a new vsla
        public String addNewVsla(Stream jsonStream)
        {
            StreamReader reader = new StreamReader(jsonStream);
            string data = reader.ReadToEnd();
            VslaDetails request;
            String _vslaCode = string.Empty;
            try
            {
                _vslaCode = genVslaCode();
                request = JsonConvert.DeserializeObject<VslaDetails>(data);
                DigitizingDataDomain.Model.Vsla vsla = new DigitizingDataDomain.Model.Vsla();
                vsla.VslaName = Convert.ToString(request.VslaName);
                vsla.VslaCode = Convert.ToString(_vslaCode);
                vsla.PhoneNumber = Convert.ToString(request.repPhoneNumber);
                vsla.ContactPerson = Convert.ToString(request.representativeName);
                vsla.VslaPhoneMsisdn = Convert.ToString(request.grpPhoneNumber);
                vsla.PositionInVsla = Convert.ToString(request.representativePosition);
                vsla.DateRegistered = DateTime.Now;
                vsla.DateLinked = DateTime.Now;
                vsla.PhysicalAddress = Convert.ToString(request.PhysicalAddress);
                vsla.GpsLocation = Convert.ToString(request.GpsLocation);
                vsla.GroupAccountNumber = Convert.ToString(request.GroupAccountNumber);
                vsla.Status = 1;
                // cbt 
                DigitizingDataDomain.Model.Cbt_info cbt = new DigitizingDataDomain.Model.Cbt_info();
                cbt.Id = Convert.ToInt32(request.tTrainerId);
                vsla.CBT = cbt;

                // region id
                DigitizingDataDomain.Model.VslaRegion vslaRegion = new DigitizingDataDomain.Model.VslaRegion();
                vslaRegion.RegionId = Convert.ToInt32(request.RegionName);
                vsla.VslaRegion = vslaRegion;

                VslaRepo vslaRepo = new VslaRepo();
                Boolean result = vslaRepo.Insert(vsla);
                return Convert.ToString(result);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }

        // generate vsla code vased on the last 4 characters of unix time stamp and last 2 digits of the current year
        public string genVslaCode()
        {
            string year = DateTime.Now.Year.ToString().Substring(2);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string unixTimeStamp = Convert.ToString(unixTimestamp).Substring(4);
            string vslaCode = "VS" + year + unixTimeStamp;
            return vslaCode;
        }

















       
        /**
         * Edit an existing VSLA
         **/
        public RegResult editExistingVsla(Stream jsonStreamObject)
        {
            StreamReader reader = new StreamReader(jsonStreamObject);
            ledgerlinkEntities database = new ledgerlinkEntities();
            string data = reader.ReadToEnd();
            string result = string.Empty;
            RegResult registrationResults = new RegResult();
            if (string.IsNullOrEmpty(data))
            {
                registrationResults.result = "-1";
                return registrationResults;
            }
            else
            {
                VslaDetails request = JsonConvert.DeserializeObject<VslaDetails>(data);
                if (null != request)
                {
                    int VslaId = request.VslaId;
                    var query = from vsla in database.Vslas where vsla.VslaId == VslaId select vsla;
                    foreach (var row in query)
                    {
                        row.VslaName = request.VslaName != null ? request.VslaName : "-NA-";
                        row.RegionId = request.RegionName != null ? Convert.ToInt32(request.RegionName) : 9;
                        row.PhysicalAddress = request.PhysicalAddress != null ? request.PhysicalAddress : "--";
                        row.VslaPhoneMsisdn = request.grpPhoneNumber != null ? request.grpPhoneNumber : "--";
                        row.GpsLocation = request.GpsLocation != null ? request.GpsLocation : "--";
                        row.ContactPerson = request.representativeName != null ? request.representativeName : "--";
                        row.PositionInVsla = request.representativePosition != null ? request.representativePosition : "--";
                        row.PhoneNumber = request.repPhoneNumber != null ? request.repPhoneNumber : "--";
                        row.GroupAccountNumber = request.GroupAccountNumber != null ? request.GroupAccountNumber : "0000000000";

                        // Then update the group training type
                        DateTime _dtime = DateTime.Today;
                        int _vslaId = VslaId;
                        int _trainerId = (Int32)request.tTrainerId;
                        String _supportType = request.GroupSupport != null ? request.GroupSupport : "--";
                        saveGroupSupportType(_dtime, _vslaId, _trainerId, _supportType);
                    }
                    // save the data to the database
                    try
                    {
                        database.SaveChanges();
                        registrationResults.result = "1";
                        registrationResults.operation = "edit";

                        // Then return the results
                        return registrationResults;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    registrationResults.result = "-1";
                    return registrationResults;
                }
            }
        }

        /**
         * Save group support to the database
         */
        public void saveGroupSupportType(DateTime _dtime, int _vslaId, int _trainerId, string _supportType)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            if (!string.IsNullOrEmpty(_supportType))
            {
                GroupSupport support = new GroupSupport
                {
                    SupportType = _supportType,
                    VslaId = _vslaId,
                    TrainerId = _trainerId,
                    SupportDate = _dtime
                };
                database.GroupSupports.Add(support);
                database.SaveChanges();
            }
        }
    }
}
