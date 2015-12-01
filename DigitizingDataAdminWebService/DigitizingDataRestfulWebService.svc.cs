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

















        /**
         * Create new VSLA
         **/
        public RegistrationResult createNewVsla(Stream jsonStream)
        {
            ledgerlinkEntities database = new ledgerlinkEntities();
            int getMaxId = database.Vslas.Max(x => x.VslaId) + 1;
            string getYear = DateTime.Now.Year.ToString().Substring(2);
            string generatedVslaCode = "VS" + getYear + getMaxId.ToString();
            String result = String.Empty;
            RegistrationResult registrationResults = new RegistrationResult();

            StreamReader reader = new StreamReader(jsonStream);
            string data = reader.ReadToEnd();
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
                    Vsla newVsla = new Vsla
                    {
                        VslaCode = generatedVslaCode,
                        VslaName = request.VslaName != null ? request.VslaName : "--",
                        RegionId = request.RegionName != null ? Convert.ToInt32(request.RegionName) : 9,
                        DateRegistered = Convert.ToDateTime(DateTime.Today),
                        DateLinked = Convert.ToDateTime(DateTime.Today),
                        PhysicalAddress = request.PhysicalAddress != null ? request.PhysicalAddress : "--",
                        VslaPhoneMsisdn = request.grpPhoneNumber != null ? request.grpPhoneNumber : "--",
                        GpsLocation = request.GpsLocation != null ? request.GpsLocation : "--",
                        ContactPerson = request.representativeName != null ? request.representativeName : "--",
                        PositionInVsla = request.representativePosition != null ? request.representativePosition : "--",
                        PhoneNumber = request.repPhoneNumber != null ? request.repPhoneNumber : "--",
                        CBT = request.tTrainerId != null ? request.tTrainerId : 1,
                        GroupAccountNumber = request.GroupAccountNumber != null ? request.GroupAccountNumber : "00000000",
                        Status = 1
                    };

                    database.Vslas.Add(newVsla);
                    database.SaveChanges();
                    registrationResults.result = "1";
                    registrationResults.VslaCode = generatedVslaCode;
                    registrationResults.operation = "create";
                    return registrationResults;
                }
                else
                {
                    registrationResults.result = "-1";
                    return registrationResults;
                }
            }

        }

        /**
         * Edit an existing VSLA
         **/
        public RegistrationResult editExistingVsla(Stream jsonStreamObject)
        {
            StreamReader reader = new StreamReader(jsonStreamObject);
            ledgerlinkEntities database = new ledgerlinkEntities();
            string data = reader.ReadToEnd();
            string result = string.Empty;
            RegistrationResult registrationResults = new RegistrationResult();
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

       
         /** Get VSLAs attached to a particular CBT
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
                        grpPhoneNumber = string.IsNullOrEmpty(vsla.table_vsla.VslaPhoneMsisdn) ? "---" : vsla.table_vsla.VslaPhoneMsisdn.ToString(),
                        PhysicalAddress = string.IsNullOrEmpty(vsla.table_vsla.PhysicalAddress) ? "---" : vsla.table_vsla.PhysicalAddress.ToString(),
                        GpsLocation = string.IsNullOrEmpty(vsla.table_vsla.GpsLocation) ? "---" : vsla.table_vsla.GpsLocation.ToString(),
                        DateRegistered = string.IsNullOrEmpty(vsla.table_vsla.DateRegistered.ToString()) ? "---" : vsla.table_vsla.DateRegistered.ToString(),
                        DateLinked = string.IsNullOrEmpty(vsla.table_vsla.DateLinked.ToString()) ? "---" : vsla.table_vsla.DateLinked.ToString(),
                        RegionName = vsla.table_regions.RegionName,
                        representativeName = string.IsNullOrEmpty(vsla.table_vsla.ContactPerson) ? "---" : vsla.table_vsla.ContactPerson,
                        representativePosition = string.IsNullOrEmpty(vsla.table_vsla.PositionInVsla) ? "---" : vsla.table_vsla.PositionInVsla,
                        repPhoneNumber = string.IsNullOrEmpty(vsla.table_vsla.PhoneNumber) ? "---" : vsla.table_vsla.PhoneNumber,
                        tTrainerName = vsla.table_cbt.Name,
                        Status = vsla.table_status.CurrentStatus,
                        GroupAccountNumber = string.IsNullOrEmpty(vsla.table_vsla.GroupAccountNumber) ? "--" : vsla.table_vsla.GroupAccountNumber,
                        tTrainerId = vsla.table_vsla.CBT

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
