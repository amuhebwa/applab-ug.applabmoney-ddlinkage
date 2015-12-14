using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
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
        public TechnicalTrainer validateTrainer(string username, string passkey)
        {
            TechnicalTrainer tt = new TechnicalTrainer();
            TechnicalTrainerRepo trainerRepo = new TechnicalTrainerRepo();
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
                        numberOfCycles = Convert.ToString(data.NumberOfCycles)
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
                     numberOfCycles = Convert.ToString(vslaData.NumberOfCycles)
                 };
                return result;
            }
            return null;
        }

        // add a new vsla
        public OperationResult addNewVsla(Stream jsonStream)
        {
            StreamReader reader = new StreamReader(jsonStream);
            string data = reader.ReadToEnd();
            VslaDetails request;
            String _vslaCode = string.Empty;
            OperationResult operationResult = null;
            try
            {
                operationResult = new OperationResult();
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
                vsla.NumberOfCycles = Convert.ToInt32(request.numberOfCycles);
                // cbt 
                DigitizingDataDomain.Model.TechnicalTrainer cbt = new DigitizingDataDomain.Model.TechnicalTrainer();
                cbt.Id = Convert.ToInt32(request.tTrainerId);
                vsla.CBT = cbt;

                // region id
                DigitizingDataDomain.Model.VslaRegion vslaRegion = new DigitizingDataDomain.Model.VslaRegion();
                vslaRegion.RegionId = Convert.ToInt32(request.RegionName);
                vsla.VslaRegion = vslaRegion;

                VslaRepo vslaRepo = new VslaRepo();
                Boolean result = vslaRepo.Insert(vsla);
                if (result)
                { // SUCCESS
                    operationResult.operation = "create";
                    operationResult.result = "1";
                    operationResult.VslaCode = _vslaCode;
                }
                else
                { // FAILED
                    operationResult.operation = null;
                    operationResult.result = "-1";
                    operationResult.VslaCode = null;
                }

            }
            catch (Exception e)
            {

            }
            return operationResult;
        }

        // generate vsla code based on the last 4 characters of unix time stamp and last 2 digits of the current year
        public string genVslaCode()
        {
            string year = DateTime.Now.Year.ToString().Substring(2);
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string unixTimeStamp = Convert.ToString(unixTimestamp).Substring(4);
            string vslaCode = "VS" + year + unixTimeStamp;
            return vslaCode;
        }

        // edit an existing vsla
        public OperationResult editVsla(Stream jsonStream)
        {
            StreamReader reader = new StreamReader(jsonStream);
            string data = reader.ReadToEnd();
            VslaDetails request;
            OperationResult operationResult = null;
            VslaRepo vslaRepo = null;
            try
            {
                operationResult = new OperationResult();
                vslaRepo = new VslaRepo();
                request = JsonConvert.DeserializeObject<VslaDetails>(data);

                // First check if the vsla exists
                int _vslaId = Convert.ToInt32(request.VslaId);
                DigitizingDataDomain.Model.Vsla vslaData = vslaRepo.FindVslaById(_vslaId);

                if (vslaData != null)
                {
                    DigitizingDataDomain.Model.Vsla vsla = new DigitizingDataDomain.Model.Vsla();
                    vslaData.VslaName = Convert.ToString(request.VslaName);
                    vslaData.PhoneNumber = Convert.ToString(request.repPhoneNumber);
                    vslaData.ContactPerson = Convert.ToString(request.representativeName);
                    vslaData.VslaPhoneMsisdn = Convert.ToString(request.grpPhoneNumber);
                    vslaData.PositionInVsla = Convert.ToString(request.representativePosition);
                    vslaData.PhysicalAddress = Convert.ToString(request.PhysicalAddress);
                    vslaData.GpsLocation = Convert.ToString(request.GpsLocation);
                    vslaData.GroupAccountNumber = Convert.ToString(request.GroupAccountNumber);
                    vslaData.NumberOfCycles = Convert.ToInt32(request.numberOfCycles);
                    // region id
                    DigitizingDataDomain.Model.VslaRegion vslaRegion = new DigitizingDataDomain.Model.VslaRegion();
                    vslaRegion.RegionId = Convert.ToInt32(request.RegionName);
                    vslaData.VslaRegion = vslaRegion;
                    Boolean updateResult = false;
                    if (vslaData.VslaId > 0)
                    {
                        updateResult = vslaRepo.Update(vslaData);
                        if (updateResult)
                        {
                            // if sucessfu, also add the support type
                            addSupportType(_vslaId, request.tTrainerId, request.GroupSupport);
                            // then construct a json feedback
                            operationResult.result = "1";
                            operationResult.operation = "edit";
                            operationResult.VslaCode = null;
                        }
                        else
                        {
                            operationResult.result = "-1";
                            operationResult.operation = null;
                            operationResult.VslaCode = null;

                        }

                    }

                }
            }
            catch (Exception e)
            {

            }
            return operationResult;
        }

        // add support type delivered to the group
        public void addSupportType(int vslaId, int trainerId, string supportType)
        {
            DateTime supportDate = DateTime.Now;
            GroupSupportType gst;
            GroupSupportRepo supportRepo = null;
            string success = string.Empty;

            DigitizingDataDomain.Model.GroupSupport groupSupport = null;
            try
            {
                gst = new GroupSupportType();
                supportRepo = new GroupSupportRepo();
                groupSupport = new DigitizingDataDomain.Model.GroupSupport();
                // vsla
                DigitizingDataDomain.Model.Vsla vsla = new DigitizingDataDomain.Model.Vsla();
                vsla.VslaId = Convert.ToInt32(vslaId);
                groupSupport.VslaId = vsla;
                // cbt
                DigitizingDataDomain.Model.TechnicalTrainer cbt = new DigitizingDataDomain.Model.TechnicalTrainer();
                cbt.Id = Convert.ToInt32(trainerId);
                groupSupport.TrainerId = cbt;
                // support date
                groupSupport.SupportDate = supportDate;
                // support type
                groupSupport.SupportType = supportType;
                // add support type attached to a group
                supportRepo.Insert(groupSupport);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
