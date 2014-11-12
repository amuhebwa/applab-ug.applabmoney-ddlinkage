using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DigitizingDataBizLayer.Repositories;
using DigitizingDataDomain.Model;

namespace DigitizingDataWebService
{   
    public class DigitizingDataRestfulService : IDigitizingDataRestfulService
    {        
        public ActivateVslaForDdResponse ActivateVslaPhone(Stream jsonRequest)
        {

            /*
             POST http://localhost:1126//DigitizingDataRestfulService.svc/vslas/activate HTTP/1.1
             Content-Type: application/x-www-form-urlencoded
             Host: localhost:1126
             Content-Length: 128

             {"VslaCode":"TEST","PhoneImei":"34056867897634755","PassKey":"12345","SimImsi":"6474845","SimSerialNo":"64854934644","NetworkOperatorName":"MTN UGANDA","NetworkType":"HSDPA"}

             * */
            
            ActivateVslaForDdResponse response = new ActivateVslaForDdResponse();
            ActivateVslaForDdRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.IsActivated = false;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<ActivateVslaForDdRequest>(jsonString);
                if (null != request)
                {
                    response.VslaName = request.VslaCode + "-ACTIVATED";
                    response.PassKey = request.PassKey.Trim();
                    //response.IsActivated = true;

                    VslaRepo vslaRepo = new VslaRepo();
                    var vsla = vslaRepo.FindVslaByCode(request.VslaCode);
                    if(vsla != null)
                    {          
                        response.VslaName = vsla.VslaName;
                        response.PassKey = request.PassKey.Trim();

                        VslaDdActivation ddActivation = new VslaDdActivation();
                        ddActivation.Vsla = vsla;
                        ddActivation.PhoneImei01 = request.PhoneImei;
                        ddActivation.SimImsiNo01 = request.SimImsi;
                        ddActivation.SimSerialNo01 = request.SimSerialNo;
                        ddActivation.SimNetworkOperator01 = request.NetworkOperatorName;
                        ddActivation.PassKey = request.PassKey;
                        ddActivation.IsActive = true;
                        ddActivation.ActivationDate = DateTime.Now;

                        VslaDdActivationRepo activationRepo = new VslaDdActivationRepo();

                        //Search for the IMEI to see whether it is already activated
                        //TODO: come back and refine this IMEI and Phone Identity problem
                        var existingActivation = activationRepo.FindActivationByImei(ddActivation.PhoneImei01);
                        if (null == existingActivation)
                        {
                            if (activationRepo.Insert(ddActivation))
                            {
                                response.IsActivated = true;
                            }
                        }
                        else
                        {
                            //Just Declare that it is Activated
                            if (null == existingActivation.Vsla)
                            {
                                //Try to Update: first setup the ActivationId to activate
                                ddActivation.ActivationId = existingActivation.ActivationId;
                                if(activationRepo.Update(ddActivation))
                                {
                                    response.IsActivated = true;
                                }
                            }
                            //Otherwise deny the activation if the VslaId is for a different VSLA
                            else if(null != existingActivation.Vsla && existingActivation.Vsla.VslaId == vsla.VslaId)
                            {
                                response.IsActivated = true;                                
                            }
                            else
                            {
                                //If the VslaId of the existing Activation by this IMEI is different from the new VslaId derived from the submitting VSLACODE
                                response.IsActivated = false;
                            }
                        }
                        
                    }
                }                
            }
            catch (Exception ex)
            {
                response.IsActivated = false;
            }
            return response;
        }

        public ActivateAdminUserResponse ActivateAdminUser(Stream jsonRequest)
        {
            ActivateAdminUserResponse response = new ActivateAdminUserResponse();
            ActivateAdminUserRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.ActivationStatus = -1;

            try
            {
                List<VslaInfo> vslas = new List<VslaInfo> {
                new VslaInfo { VslaCode = "V001", VslaId = 13, VslaName ="ABAKISA BAKHONYANA"},
                new VslaInfo { VslaCode = "V002", VslaId = 21, VslaName ="IGANGA FARMERS"},
                new VslaInfo { VslaCode = "V003", VslaId = 24, VslaName ="BUGIRI DAIRY FARMERS ASSOCIATION"},
                new VslaInfo { VslaCode = "V004", VslaId = 35, VslaName ="AMEN A"},
                new VslaInfo { VslaCode = "V007", VslaId = 39, VslaName ="CARE UGANDA STAFF"},
                new VslaInfo { VslaCode = "V008", VslaId = 13, VslaName ="GRAMEEN STAFF"}
            };

                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<ActivateAdminUserRequest>(jsonString);
                if (null != request)
                {

                    response.VslaList = vslas;
                    response.ActivationStatus = 0;
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public string ActivateAdminUser(string username, string securityToken)
        {
            return username + securityToken;
        }

        public string ActivateAdminUserGet(string username, string securityToken)
        {
            return username + securityToken;
        }

        public List<VslaInfo> GetVslas(string regionId)
        {
            List<VslaInfo> vslas = new List<VslaInfo>{
                new VslaInfo { VslaCode = "V001", VslaId = 13, VslaName ="ABAKISA BAKHONYANA"},
                new VslaInfo { VslaCode = "V002", VslaId = 21, VslaName ="IGANGA FARMERS"},
                new VslaInfo { VslaCode = "V003", VslaId = 24, VslaName ="BUGIRI DAIRY FARMERS ASSOCIATION"},
                new VslaInfo { VslaCode = "V004", VslaId = 35, VslaName ="AMEN A"},
                new VslaInfo { VslaCode = "V007", VslaId = 39, VslaName ="CARE UGANDA STAFF"},
                new VslaInfo { VslaCode = "V008", VslaId = 13, VslaName ="GRAMEEN STAFF"}
            };

            return vslas;
        }

        public DeliverVslaKitResponse DeliverVslaKit(Stream jsonRequest)
        {

            DeliverVslaKitResponse response = new DeliverVslaKitResponse();
            DeliverVslaKitRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.DeliveryStatus = -1;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<DeliverVslaKitRequest>(jsonString);
                if (null != request && request.VslaPhoneImei.Trim().Length > 0 && request.PhoneImei.Trim().Length > 0)
                {
                    response.DeliveryStatus = Convert.ToInt32(request.VslaPhoneImei.Substring(0, 3) + request.PhoneImei.Substring(0, 3));
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public CaptureGpsLocationResponse CaptureGpsLocation(Stream jsonRequest)
        {

            CaptureGpsLocationResponse response = new CaptureGpsLocationResponse();
            CaptureGpsLocationRequest request = null;
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.StatusCode = -1;            

            try
            {
                //Just for Creating the Database
                //DataSubmissionRepo repo = null;
                //repo = new DataSubmissionRepo();
                //var list = repo.FindAll();

                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                request = JsonConvert.DeserializeObject<CaptureGpsLocationRequest>(jsonString);
                if (null != request && request.GpsLocation.Trim().Length > 0 && request.PhoneImei.Trim().Length > 0)
                {
                    response.StatusCode = 0;
                }

            }
            catch (Exception ex)
            {

            }
            return response;
        }

        public SubmitVslaDataResponse SubmitVslaData(Stream jsonRequest)
        {
            SubmitVslaDataResponse response = new SubmitVslaDataResponse();
            StreamReader reader = null;
            string jsonString = string.Empty;
            response.StatusCode = -1;
            DataSubmission dataSubmission = null;
            DataSubmissionRepo repo = null;

            try
            {
                reader = new StreamReader(jsonRequest);
                jsonString = reader.ReadToEnd();
                dynamic obj = JObject.Parse(jsonString);                
                var headerInfo = obj.HeaderInfo;
                
                //Data Item: this will be an Enum
                string dataItem = string.Empty;

                //{"HeaderInfo":{"VslaCode":"V1004","PhoneImei":"356422050612411","NetworkOperator":"MTN-UGANDA","NetworkType":"EDGE","DataItem":"cycleInfo"}
                if (null != headerInfo)
                {
                    dataSubmission = new DataSubmission();
                    dataSubmission.SourceVslaCode = Convert.ToString(headerInfo.VslaCode);
                    dataSubmission.SourcePhoneImei = Convert.ToString(headerInfo.PhoneImei);
                    dataSubmission.SourceNetworkOperator = Convert.ToString(headerInfo.NetworkOperator);
                    dataSubmission.SourceNetworkType = Convert.ToString(headerInfo.NetworkType);
                    dataSubmission.SubmissionTimestamp = DateTime.Now;
                    dataSubmission.Data = jsonString;
                    dataItem = Convert.ToString(headerInfo.DataItem);
                }

                if(dataSubmission != null)
                {
                    repo = new DataSubmissionRepo();
                    //repo.FindAll();

                    bool retVal = repo.Insert(dataSubmission);
                    int retValDataProcessed = -1;
                    if(retVal)
                    {
                        //Process the data
                        if(dataItem.ToUpper() == "CYCLEINFO")
                        {
                            //Retrieve the Information about the VSLA CYCLE and populate it into the database
                            var vslaCycleData = obj.VslaCycle;
                            
                            //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                            retValDataProcessed = ProcessVslaCycleData(vslaCycleData, dataSubmission.SourceVslaCode);
                        }
                        else if (dataItem.ToUpper() == "MEETINGDETAILS")
                        {
                            //Retrieve the Information about the MEETING and populate it into the database
                            var meetingData = obj.Meeting;

                            //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                            retValDataProcessed = ProcessMeetingData(meetingData, dataSubmission.SourceVslaCode);
                        }
                        else if (dataItem.ToUpper() == "MEMBERS")
                        {
                            //Retrieve the Information about the MEETING and populate it into the database
                            var membersObj = obj.Members;
                            var recordCount = Convert.ToInt32(obj.MemberCount);

                            //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                            retValDataProcessed = ProcessMembersCollection(membersObj, recordCount, dataSubmission.SourceVslaCode);
                        }
                        else if (dataItem.ToUpper() == "ATTENDANCE")
                        {
                            //Retrieve the Information about the MEETING and populate it into the database
                            var attendancesObj = obj.Attendances;
                            var recordCount = Convert.ToInt32(obj.MembersCount);
                            var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                            //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                            retValDataProcessed = ProcessAttendancesCollection(attendancesObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                        }
                        else if (dataItem.ToUpper() == "SAVINGS")
                        {
                            //Retrieve the Information about the SAVINGS and populate it into the database
                            var savingsObj = obj.Savings;
                            var recordCount = Convert.ToInt32(obj.MembersCount);
                            var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                            //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                            retValDataProcessed = ProcessSavingsCollection(savingsObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                        }
                        else if (dataItem.ToUpper() == "LOANS")
                        {
                            //Retrieve the Information about the LOANS and populate it into the database
                            var loanIssuesObj = obj.Loans;
                            var recordCount = Convert.ToInt32(obj.MembersCount);
                            var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                            //Pass the dynamic obj retrieved from the JSON string representing a LoanIssues array
                            retValDataProcessed = ProcessLoanIssuesCollection(loanIssuesObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                        }
                        else if (dataItem.ToUpper() == "REPAYMENTS")
                        {
                            //Retrieve the Information about the REPAYMENTS and populate it into the database
                            var loanRepaymentsObj = obj.Repayments;
                            var recordCount = Convert.ToInt32(obj.MembersCount);
                            var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                            //Pass the dynamic obj retrieved from the JSON string representing a LoanRepayments array
                            retValDataProcessed = ProcessLoanRepaymentsCollection(loanRepaymentsObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                        }
                        else if(dataItem.ToUpper() == "FINES")
                        {
                            //Retrieve the Information about the FINES and populate it into the database
                            var finesObj = obj.Fines;
                            var recordCount = Convert.ToInt32(obj.MembersCount);
                            var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                            //Pass the dynamic obj retrieved from the JSON string representing a Fines array
                            retValDataProcessed = ProcessFinesCollection(finesObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                        }

                        //return the response code
                        response.StatusCode = retValDataProcessed;
                    }
                }
            }
            catch
            {
                response.StatusCode = -99;
            }

            return response;
        }

        private int ProcessVslaCycleData(dynamic vslaCycleObj, string vslaCode)
        {
            //"VslaCycle": {"CycleId": 1,"StartDate": "2013-06-03","EndDate": "2014-06-02","SharePrice": 500,"MaxShareQty": 5,"MaxStartShare": 0,"InterestRate": 10}
            
            VslaCycleRepo vslaCycleRepo = null;
            VslaRepo vslaRepo = null;

            try
            {
                if (vslaCycleObj == null)
                {
                    return -1;
                }

                //Otherwise, continue
                vslaRepo = new VslaRepo();
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if(targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                vslaCycleRepo = new VslaCycleRepo();
                VslaCycle vslaCycle = null;

                //Check whether this record exists
                var targetCycle = vslaCycleRepo.FindVslaCycleByIdEx(vslaCode, Convert.ToInt32(vslaCycleObj.CycleId));
                if (targetCycle == null)
                {
                    vslaCycle = new VslaCycle();
                }
                else
                {
                    vslaCycle = targetCycle;
                }
                vslaCycle.Vsla = targetVsla;
                vslaCycle.CycleIdEx = Convert.ToInt32(vslaCycleObj.CycleId);
                
                DateTime theDate = DateTime.Now;
                var strStartDate = Convert.ToString(vslaCycleObj.StartDate);
                if( DateTime.TryParse(strStartDate, out theDate))
                {
                    vslaCycle.StartDate = theDate;
                }

                var strEndDate = Convert.ToString(vslaCycleObj.EndDate);
                if (DateTime.TryParse(strEndDate, out theDate))
                {
                    vslaCycle.EndDate = theDate;
                }
                vslaCycle.SharePrice = Convert.ToDouble(vslaCycleObj.SharePrice);
                vslaCycle.MaxShareQuantity = Convert.ToInt32(vslaCycleObj.MaxShareQty);
                vslaCycle.MaxStartShare = Convert.ToDouble(vslaCycleObj.MaxStartShare);
                vslaCycle.InterestRate = Convert.ToDouble(vslaCycleObj.InterestRate);

                //Save the changes 
                bool retValSave = false;
                if(vslaCycle.CycleId > 0)
                {                    
                    retValSave = vslaCycleRepo.Update(vslaCycle);
                }
                else
                {
                    retValSave = vslaCycleRepo.Insert(vslaCycle);
                }

                if(retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessMembersCollection(dynamic membersObj, int recordCount, string vslaCode)
        {
            //"MemberCount":30,"Members":[{"MemberId":1,"MemberNo":1,"Surname":"Kotoyi","OtherNames":"Everlyn","Gender":"Female","DateOfBirth":"1971-11-12","Occupation":"Farmer","PhoneNumber":"771450619","CyclesCompleted":4,"IsActive":false,"IsArchived":false},{"MemberId":2,"MemberNo":2,"Surname":"Musamali","OtherNames":"Joseph","Gender":"Female","DateOfBirth":"1988-11-12","Occupation":"Farmer","PhoneNumber":"785327102","CyclesCompleted":4,"IsActive":false,"IsArchived":false}]}
            if (membersObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                foreach (var membObj in membersObj)
                {
                    var retVal = ProcessMemberData(membObj, targetVsla);
                    if(retVal == 0)
                    {
                        processedCount++;
                    }
                }

                //if (processedCount < recordCount)
                //{
                //    return -1;
                //}
                //else
                //{
                //    return 0;
                //}

                return 0;
            }
            catch(Exception ex)
            {
                return -99;
            }

        }

        private int ProcessMemberData(dynamic memberObj, Vsla targetVsla)
        {
            //"MemberCount":30,"Members":[{"MemberId":1,"MemberNo":1,"Surname":"Kotoyi","OtherNames":"Everlyn","Gender":"Female","DateOfBirth":"1971-11-12","Occupation":"Farmer","PhoneNumber":"771450619","CyclesCompleted":4,"IsActive":false,"IsArchived":false},{"MemberId":2,"MemberNo":2,"Surname":"Musamali","OtherNames":"Joseph","Gender":"Female","DateOfBirth":"1988-11-12","Occupation":"Farmer","PhoneNumber":"785327102","CyclesCompleted":4,"IsActive":false,"IsArchived":false}]}
            MemberRepo memberRepo = null;            

            try
            {
                if (memberObj == null)
                {
                    return -1;
                }

                //Otherwise, continue                
                memberRepo = new MemberRepo();
                Member member = null;

                //Check whether this record exists
                var targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(memberObj.MemberId));
                if (targetMember == null)
                {
                    member = new Member();
                }
                else
                {
                    member = targetMember;
                }
                member.Vsla = targetVsla;
                member.MemberIdEx = Convert.ToInt32(memberObj.MemberId);

                //{"MemberId":1,"MemberNo":1,"Surname":"Kotoyi","OtherNames":"Everlyn","Gender":"Female","DateOfBirth":"1971-11-12","Occupation":"Farmer","PhoneNumber":"771450619","CyclesCompleted":4,"IsActive":false,"IsArchived":false}
                member.MemberNo = Convert.ToInt32(memberObj.MemberNo);
                member.CyclesCompleted = Convert.ToInt32(memberObj.CyclesCompleted);
                member.DateArchived = DateTime.Now;
                member.IsArchived = Convert.ToBoolean(memberObj.IsArchived);
                
                DateTime theDate = DateTime.Now;
                var strDateOfBirth = Convert.ToString(memberObj.DateOfBirth);
                if (DateTime.TryParse(strDateOfBirth, out theDate))
                {
                    member.DateOfBirth = theDate;
                }

                member.Gender = Convert.ToString(memberObj.Gender);
                member.IsActive = Convert.ToBoolean(memberObj.IsActive);
                member.Occupation = Convert.ToString(memberObj.Occupation);
                member.OtherNames = Convert.ToString(memberObj.OtherNames);
                member.PhoneNo = Convert.ToString(memberObj.PhoneNo);
                member.Surname = Convert.ToString(memberObj.Surname);

                //TODO: Add Mid-Cycle Data to the Members Object
                //Total Savings, Outstanding Loan, Outstanding Fine

                //Save the changes 
                bool retValSave = false;
                if (member.MemberId > 0)
                {
                    retValSave = memberRepo.Update(member);
                }
                else
                {
                    retValSave = memberRepo.Insert(member);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessMeetingData(dynamic meetingObj, string vslaCode)
        {
            //The VSLA Code will help in knowing which VSLA Cycle to consider because the CycleId from phone is not unique across VSLAs
            //"Meeting":{"CycleId":1,"MeetingId":9,"MeetingDate":"2014-01-06","OpeningBalanceBox":0,"OpeningBalanceBank":0,"Fines":0,"MembersPresent":19,"Savings":70000,"LoansRepaid":306116,"LoansIssued":0,"ClosingBalanceBox":0,"ClosingBalanceBank":0,"IsCashBookBalanced":false,"IsDataSent":false}}
            MeetingRepo meetingRepo = null;
            VslaRepo vslaRepo = null;
            VslaCycleRepo vslaCycleRepo = null;

            try
            {
                if (meetingObj == null)
                {
                    return -1;
                }

                //Otherwise, continue
                vslaRepo = new VslaRepo();
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                vslaCycleRepo = new VslaCycleRepo();
                meetingRepo = new MeetingRepo();

                Meeting meeting = null;

                //Get the Cycle to which the Meeting belongs to
                VslaCycle targetCycle = vslaCycleRepo.FindVslaCycleByIdEx(vslaCode, Convert.ToInt32(meetingObj.CycleId));
                if (targetCycle == null)
                {
                    return -3; //Target VSLA CYCLE does not exist
                }

                //Otherwise continue: Check whether the Meeting exists
                var targetMeeting = meetingRepo.FindMeetingByIdEx(targetCycle.CycleId, Convert.ToInt32(meetingObj.MeetingId));
                if (targetMeeting == null)
                {
                    meeting = new Meeting();
                }
                else
                {
                    meeting = targetMeeting;
                }
                meeting.VslaCycle = targetCycle;

                //The MeetingIdEx is the MeetingId on the vsla phone
                meeting.MeetingIdEx = Convert.ToInt32(meetingObj.MeetingId);

                DateTime theDate = DateTime.Now;
                var strMeetingDate = Convert.ToString(meetingObj.MeetingDate);
                if (DateTime.TryParse(strMeetingDate, out theDate))
                {
                    meeting.MeetingDate = theDate;
                }
                
                meeting.CashFromBox = Convert.ToDouble(meetingObj.OpeningBalanceBox);
                meeting.CashFromBank = Convert.ToDouble(meetingObj.OpeningBalanceBank);
                meeting.CashFines = Convert.ToDouble(meetingObj.Fines);
                meeting.CashSavedBox = Convert.ToDouble(meetingObj.ClosingBalanceBox);
                meeting.CashSavedBank = Convert.ToDouble(meetingObj.ClosingBalanceBank);
                meeting.CashWelfare = 0D;
                meeting.CashExpenses = 0D;
                meeting.IsDataSent = Convert.ToBoolean(meetingObj.IsDataSent);
                meeting.IsCurrent = false;
                meeting.DateSent = DateTime.Now;

                //Summary Fields
                meeting.CountOfMembersPresent = Convert.ToInt32(meetingObj.MembersPresent);
                meeting.SumOfSavings = Convert.ToDouble(meetingObj.Savings);
                meeting.SumOfLoanIssues = Convert.ToDouble(meetingObj.LoansIssued);
                meeting.SumOfLoanRepayments = Convert.ToDouble(meetingObj.LoansRepaid);

                //Save the changes 
                bool retValSave = false;
                if (meeting.MeetingId > 0)
                {
                    retValSave = meetingRepo.Update(meeting);
                }
                else
                {
                    retValSave = meetingRepo.Insert(meeting);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessAttendancesCollection(dynamic attendancesObj, int recordCount, string vslaCode, int meetingIdEx)
        {
            //I may consider passing the VSLACycleID to reduce on the depth of subqueries
            //"MeetingId":9,"MembersCount":30,"Attendances":[{"AttendanceId":211,"MemberId":1,"IsPresentFlg":0,"Comments":null},{"AttendanceId":212,"MemberId":2,"IsPresentFlg":1,"Comments":null},{"AttendanceId":213,"MemberId":3,"IsPresentFlg":1,"Comments":null},{"AttendanceId":214,"MemberId":4,"IsPresentFlg":0,"Comments":null},{"AttendanceId":215,"MemberId":5,"IsPresentFlg":1,"Comments":null},{"AttendanceId":216,"MemberId":6,"IsPresentFlg":1,"Comments":null},{"AttendanceId":217,"MemberId":7,"IsPresentFlg":1,"Comments":null},{"AttendanceId":218,"MemberId":8,"IsPresentFlg":1,"Comments":null},{"AttendanceId":219,"MemberId":9,"IsPresentFlg":1,"Comments":null},{"AttendanceId":220,"MemberId":10,"IsPresentFlg":0,"Comments":null},{"AttendanceId":221,"MemberId":11,"IsPresentFlg":0,"Comments":null},{"AttendanceId":222,"MemberId":12,"IsPresentFlg":1,"Comments":null},{"AttendanceId":223,"MemberId":13,"IsPresentFlg":1,"Comments":null},{"AttendanceId":224,"MemberId":14,"IsPresentFlg":1,"Comments":null},{"AttendanceId":225,"MemberId":15,"IsPresentFlg":1,"Comments":null},{"AttendanceId":226,"MemberId":16,"IsPresentFlg":1,"Comments":null},{"AttendanceId":227,"MemberId":17,"IsPresentFlg":0,"Comments":null},{"AttendanceId":228,"MemberId":18,"IsPresentFlg":0,"Comments":null},{"AttendanceId":229,"MemberId":19,"IsPresentFlg":0,"Comments":null},{"AttendanceId":230,"MemberId":20,"IsPresentFlg":1,"Comments":null},{"AttendanceId":231,"MemberId":21,"IsPresentFlg":1,"Comments":null},{"AttendanceId":232,"MemberId":22,"IsPresentFlg":0,"Comments":null},{"AttendanceId":233,"MemberId":23,"IsPresentFlg":1,"Comments":null},{"AttendanceId":234,"MemberId":24,"IsPresentFlg":0,"Comments":null},{"AttendanceId":235,"MemberId":25,"IsPresentFlg":1,"Comments":null},{"AttendanceId":236,"MemberId":26,"IsPresentFlg":1,"Comments":null},{"AttendanceId":237,"MemberId":27,"IsPresentFlg":0,"Comments":null},{"AttendanceId":238,"MemberId":28,"IsPresentFlg":1,"Comments":null},{"AttendanceId":239,"MemberId":29,"IsPresentFlg":1,"Comments":null},{"AttendanceId":240,"MemberId":30,"IsPresentFlg":0,"Comments":null}]}
            if (attendancesObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            MeetingRepo meetingRepo = null;

            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                meetingRepo = new MeetingRepo();

                //Retrieve the Target VSLA
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                //Retrieve the target Meeting
                var targetMeeting = meetingRepo.FindMeetingByIdEx(vslaCode, Convert.ToInt32(meetingIdEx));
                if (targetMeeting == null)
                {
                    return -3; //Target Meeting does not exist
                }

                foreach (var attendanceObj in attendancesObj)
                {
                    var retVal = ProcessAttendanceData(attendanceObj, targetMeeting, targetVsla);
                    if (retVal == 0)
                    {
                        processedCount++;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -99;
            }

        }

        private int ProcessAttendanceData(dynamic attendanceObj, Meeting targetMeeting, Vsla targetVsla)
        {
            //"MeetingId":9,"MembersCount":30,"Attendances":[{"AttendanceId":211,"MemberId":1,"IsPresentFlg":0,"Comments":null},{"AttendanceId":212,"MemberId":2,"IsPresentFlg":1,"Comments":null}]}
            AttendanceRepo attendanceRepo = null;
            MemberRepo memberRepo = null;
            Member targetMember = null;

            try
            {
                if (attendanceObj == null)
                {
                    return -1;
                }

                if (null == targetMeeting || null == targetVsla)
                {
                    return -2; //Target Meeting or VSLA is missing
                }

                //Get the target member
                memberRepo = new MemberRepo();
                targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(attendanceObj.MemberId));
                if (null == targetMember)
                {
                    return -3; //Target Member not Found
                }

                //Otherwise, continue                
                attendanceRepo = new AttendanceRepo();
                Attendance attendance = null;


                //Check whether this record exists
                var targetAttendance = attendanceRepo.FindAttendanceByIdEx(targetMeeting.MeetingId, Convert.ToInt32(attendanceObj.AttendanceId));
                if (targetAttendance == null)
                {
                    attendance = new Attendance();
                }
                else
                {
                    attendance = targetAttendance;
                }
                attendance.Meeting = targetMeeting;
                attendance.AttendanceIdEx = Convert.ToInt32(attendanceObj.AttendanceId);
                attendance.Comments = Convert.ToString(attendanceObj.Comments);
                attendance.Member = targetMember;

                //Ensure that 1 is interpreted as TRUE
                int isPresentFlg = Convert.ToInt32(attendanceObj.IsPresentFlg);
                attendance.IsPresent = (isPresentFlg == 1) ? true : false;

                //Save the changes 
                bool retValSave = false;
                if (attendance.AttendanceId > 0)
                {
                    retValSave = attendanceRepo.Update(attendance);
                }
                else
                {
                    retValSave = attendanceRepo.Insert(attendance);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessSavingsCollection(dynamic savingsObj, int recordCount, string vslaCode, int meetingIdEx)
        {
            //"MeetingId":9,"MembersCount":30,"Savings":[{"SavingId":213,"MemberId":1,"Amount":2500},{"SavingId":214,"MemberId":2,"Amount":2500},{"SavingId":215,"MemberId":3,"Amount":2500},{"SavingId":216,"MemberId":4,"Amount":1000},{"SavingId":217,"MemberId":5,"Amount":2500},{"SavingId":218,"MemberId":6,"Amount":2500},{"SavingId":219,"MemberId":7,"Amount":2500},{"SavingId":220,"MemberId":8,"Amount":2500},{"SavingId":221,"MemberId":9,"Amount":2500},{"SavingId":222,"MemberId":10,"Amount":2500},{"SavingId":223,"MemberId":11,"Amount":2000},{"SavingId":224,"MemberId":12,"Amount":2000},{"SavingId":225,"MemberId":13,"Amount":2000},{"SavingId":226,"MemberId":14,"Amount":2500},{"SavingId":227,"MemberId":15,"Amount":2500},{"SavingId":228,"MemberId":16,"Amount":2500},{"SavingId":229,"MemberId":17,"Amount":2500},{"SavingId":230,"MemberId":18,"Amount":2500},{"SavingId":231,"MemberId":19,"Amount":1000},{"SavingId":232,"MemberId":20,"Amount":2500},{"SavingId":233,"MemberId":21,"Amount":2500},{"SavingId":234,"MemberId":22,"Amount":2500},{"SavingId":235,"MemberId":23,"Amount":2000},{"SavingId":236,"MemberId":24,"Amount":2500},{"SavingId":237,"MemberId":25,"Amount":2500},{"SavingId":238,"MemberId":26,"Amount":2500},{"SavingId":239,"MemberId":27,"Amount":2500},{"SavingId":240,"MemberId":28,"Amount":2500},{"SavingId":241,"MemberId":29,"Amount":2500},{"SavingId":242,"MemberId":30,"Amount":2500}]}
            if (savingsObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            MeetingRepo meetingRepo = null;

            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                meetingRepo = new MeetingRepo();

                //Retrieve the Target VSLA
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                //Retrieve the target Meeting
                var targetMeeting = meetingRepo.FindMeetingByIdEx(vslaCode, Convert.ToInt32(meetingIdEx));
                if (targetMeeting == null)
                {
                    return -3; //Target Meeting does not exist
                }

                foreach (var savingObj in savingsObj)
                {
                    var retVal = ProcessSavingData(savingObj, targetMeeting, targetVsla);
                    if (retVal == 0)
                    {
                        processedCount++;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessSavingData(dynamic savingObj, Meeting targetMeeting, Vsla targetVsla)
        {
            //"MeetingId":9,"MembersCount":30,"Savings":[{"SavingId":213,"MemberId":1,"Amount":2500},{"SavingId":214,"MemberId":2,"Amount":2500},{"SavingId":215,"MemberId":3,"Amount":2500},{"SavingId":216,"MemberId":4,"Amount":1000},{"SavingId":217,"MemberId":5,"Amount":2500},{"SavingId":218,"MemberId":6,"Amount":2500},{"SavingId":219,"MemberId":7,"Amount":2500},{"SavingId":220,"MemberId":8,"Amount":2500},{"SavingId":221,"MemberId":9,"Amount":2500},{"SavingId":222,"MemberId":10,"Amount":2500},{"SavingId":223,"MemberId":11,"Amount":2000},{"SavingId":224,"MemberId":12,"Amount":2000},{"SavingId":225,"MemberId":13,"Amount":2000},{"SavingId":226,"MemberId":14,"Amount":2500},{"SavingId":227,"MemberId":15,"Amount":2500},{"SavingId":228,"MemberId":16,"Amount":2500},{"SavingId":229,"MemberId":17,"Amount":2500},{"SavingId":230,"MemberId":18,"Amount":2500},{"SavingId":231,"MemberId":19,"Amount":1000},{"SavingId":232,"MemberId":20,"Amount":2500},{"SavingId":233,"MemberId":21,"Amount":2500},{"SavingId":234,"MemberId":22,"Amount":2500},{"SavingId":235,"MemberId":23,"Amount":2000},{"SavingId":236,"MemberId":24,"Amount":2500},{"SavingId":237,"MemberId":25,"Amount":2500},{"SavingId":238,"MemberId":26,"Amount":2500},{"SavingId":239,"MemberId":27,"Amount":2500},{"SavingId":240,"MemberId":28,"Amount":2500},{"SavingId":241,"MemberId":29,"Amount":2500},{"SavingId":242,"MemberId":30,"Amount":2500}]}
            SavingRepo savingRepo = null;
            MemberRepo memberRepo = null;
            Member targetMember = null;

            try
            {
                if (savingObj == null)
                {
                    return -1;
                }

                if (null == targetMeeting || null == targetVsla)
                {
                    return -2; //Target Meeting or VSLA is missing
                }

                //Get the target member
                memberRepo = new MemberRepo();
                targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(savingObj.MemberId));
                if (null == targetMember)
                {
                    return -3; //Target Member not Found
                }

                //Otherwise, continue                
                savingRepo = new SavingRepo();
                Saving saving = null;


                //Check whether this record exists
                var targetSaving = savingRepo.FindSavingByIdEx(targetMeeting.MeetingId, Convert.ToInt32(savingObj.SavingId));
                if (targetSaving == null)
                {
                    saving = new Saving();
                }
                else
                {
                    saving = targetSaving;
                }
                saving.Meeting = targetMeeting;
                saving.SavingIdEx = Convert.ToInt32(savingObj.SavingId);
                saving.Amount = Convert.ToDouble(savingObj.Amount);
                saving.Member = targetMember;
                                
                //Save the changes 
                bool retValSave = false;
                if (saving.SavingId > 0)
                {
                    retValSave = savingRepo.Update(saving);
                }
                else
                {
                    retValSave = savingRepo.Insert(saving);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessLoanIssuesCollection(dynamic loanIssuesObj, int recordCount, string vslaCode, int meetingIdEx)
        {
            //"MeetingId":9,"MembersCount":1,"Loans":[{"MemberId":15,"LoanId":28,"LoanNo":112,"PrincipalAmount":30000,"InterestAmount":3000,"TotalRepaid":0,"LoanBalance":33000,"DateDue":"2014-01-02","Comments":null,"DateCleared":null,"IsCleared":false,"IsDefaulted":false,"IsWrittenOff":false}]}"
            if (loanIssuesObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            MeetingRepo meetingRepo = null;

            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                meetingRepo = new MeetingRepo();

                //Retrieve the Target VSLA
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                //Retrieve the target Meeting
                var targetMeeting = meetingRepo.FindMeetingByIdEx(vslaCode, Convert.ToInt32(meetingIdEx));
                if (targetMeeting == null)
                {
                    return -3; //Target Meeting does not exist
                }

                foreach (var loanIssueObj in loanIssuesObj)
                {
                    var retVal = ProcessLoanIssueData(loanIssueObj, targetMeeting, targetVsla);
                    if (retVal == 0)
                    {
                        processedCount++;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessLoanIssueData(dynamic loanIssueObj, Meeting targetMeeting, Vsla targetVsla)
        {
            //"MeetingId":9,"MembersCount":1,"Loans":[{"MemberId":15,"LoanId":28,"LoanNo":112,"PrincipalAmount":30000,"InterestAmount":3000,"TotalRepaid":0,"LoanBalance":33000,"DateDue":"2014-01-02","Comments":null,"DateCleared":null,"IsCleared":false,"IsDefaulted":false,"IsWrittenOff":false}]}"
            LoanIssueRepo loanIssueRepo = null;
            MemberRepo memberRepo = null;
            Member targetMember = null;

            try
            {
                if (loanIssueObj == null)
                {
                    return -1;
                }

                if (null == targetMeeting || null == targetVsla)
                {
                    return -2; //Target Meeting or VSLA is missing
                }

                //Get the target member
                memberRepo = new MemberRepo();
                targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(loanIssueObj.MemberId));
                if (null == targetMember)
                {
                    return -3; //Target Member not Found
                }

                //Otherwise, continue                
                loanIssueRepo = new LoanIssueRepo();
                LoanIssue loanIssue = null;


                //Check whether this record exists
                var targetLoanIssue = loanIssueRepo.FindLoanIssueByIdEx(targetMeeting.MeetingId, Convert.ToInt32(loanIssueObj.LoanId));
                if (targetLoanIssue == null)
                {
                    loanIssue = new LoanIssue();
                }
                else
                {
                    loanIssue = targetLoanIssue;
                }
                loanIssue.Meeting = targetMeeting;
                loanIssue.Member = targetMember;
                loanIssue.LoanIdEx = Convert.ToInt32(loanIssueObj.LoanId);
                loanIssue.PrincipalAmount = Convert.ToDouble(loanIssueObj.PrincipalAmount);
                loanIssue.LoanNo = Convert.ToInt32(loanIssueObj.LoanNo);
                loanIssue.TotalRepaid = Convert.ToDouble(loanIssueObj.TotalRepaid);
                loanIssue.Balance = Convert.ToDouble(loanIssueObj.LoanBalance);
                loanIssue.InterestAmount = Convert.ToDouble(loanIssueObj.InterestAmount);
                DateTime theDate = DateTime.Now;
                var strDateDue = Convert.ToString(loanIssueObj.DateDue);
                if (DateTime.TryParse(strDateDue, out theDate))
                {
                    loanIssue.DateDue = theDate;
                }

                theDate = DateTime.Now;
                var strDateCleared = Convert.ToString(loanIssueObj.DateCleared);
                if (DateTime.TryParse(strDateCleared, out theDate))
                {
                    loanIssue.DateCleared = theDate;
                }
                loanIssue.Comments = Convert.ToString(loanIssueObj.Comments);

                //Ensure that 1 is interpreted as TRUE
                int isClearedFlg = Convert.ToInt32(loanIssueObj.IsCleared);
                loanIssue.IsCleared = (isClearedFlg == 1) ? true : false;

                //Ensure that 1 is interpreted as TRUE
                int isDefaultedFlg = Convert.ToInt32(loanIssueObj.IsDefaulted);
                loanIssue.IsDefaulted = (isDefaultedFlg == 1) ? true : false;

                //Ensure that 1 is interpreted as TRUE
                int isWrittenOffFlg = Convert.ToInt32(loanIssueObj.IsWrittenOff);
                loanIssue.IsWrittenOff = (isWrittenOffFlg == 1) ? true : false;
                                
                //Save the changes 
                bool retValSave = false;
                if (loanIssue.LoanId > 0)
                {
                    retValSave = loanIssueRepo.Update(loanIssue);
                }
                else
                {
                    retValSave = loanIssueRepo.Insert(loanIssue);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessLoanRepaymentsCollection(dynamic loanRepaymentsObj, int recordCount, string vslaCode, int meetingIdEx)
        {
            //"MeetingId":9,"MembersCount":10,"Repayments":[{"RepaymentId":23,"MemberId":6,"LoanId":39,"Amount":22000,"BalanceBefore":187000,"BalanceAfter":165000,"InterestAmount":16500,"RolloverAmount":181500,"Comments":"","LastDateDue":"2014-01-23","NextDateDue":"2014-02-06"},
            if (loanRepaymentsObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            MeetingRepo meetingRepo = null;

            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                meetingRepo = new MeetingRepo();

                //Retrieve the Target VSLA
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                //Retrieve the target Meeting
                var targetMeeting = meetingRepo.FindMeetingByIdEx(vslaCode, Convert.ToInt32(meetingIdEx));
                if (targetMeeting == null)
                {
                    return -3; //Target Meeting does not exist
                }

                foreach (var loanRepaymentObj in loanRepaymentsObj)
                {
                    var retVal = ProcessLoanRepaymentData(loanRepaymentObj, targetMeeting, targetVsla);
                    if (retVal == 0)
                    {
                        processedCount++;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessLoanRepaymentData(dynamic loanRepaymentObj, Meeting targetMeeting, Vsla targetVsla)
        {
            //"MeetingId":9,"MembersCount":10,"Repayments":[{"RepaymentId":23,"MemberId":6,"LoanId":39,"Amount":22000,"BalanceBefore":187000,"BalanceAfter":165000,"InterestAmount":16500,"RolloverAmount":181500,"Comments":"","LastDateDue":"2014-01-23","NextDateDue":"2014-02-06"},
            LoanIssueRepo loanIssueRepo = null;
            LoanRepaymentRepo loanRepaymentRepo = null;
            MemberRepo memberRepo = null;
            Member targetMember = null;
            LoanIssue targetLoanIssue = null;

            try
            {
                if (loanRepaymentObj == null)
                {
                    return -1;
                }

                if (null == targetMeeting || null == targetVsla)
                {
                    return -2; //Target Meeting or VSLA is missing
                }

                //Get the target member
                memberRepo = new MemberRepo();
                targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(loanRepaymentObj.MemberId));
                if (null == targetMember)
                {
                    return -3; //Target Member not Found
                }

                //Get the target Loan: By the way here with the LoanId, the Member, Cycle & VSLA details can be found on the Loan Object                
                loanIssueRepo = new LoanIssueRepo();
                targetLoanIssue = loanIssueRepo.FindLoanIssueByMemberAndLoadIdEx(targetMember.MemberId, Convert.ToInt32(loanRepaymentObj.LoanId));
                if (null == targetLoanIssue)
                {
                    return -4; //Target Loan not Found
                }

                //Check whether this record exists
                loanRepaymentRepo = new LoanRepaymentRepo();
                LoanRepayment loanRepayment = null;

                var targetLoanRepayment = loanRepaymentRepo.FindLoanRepaymentByIdEx(targetMeeting.MeetingId, Convert.ToInt32(loanRepaymentObj.RepaymentId));
                if (targetLoanRepayment == null)
                {
                    loanRepayment = new LoanRepayment();
                }
                else
                {
                    loanRepayment = targetLoanRepayment;
                }
                loanRepayment.Meeting = targetMeeting;
                loanRepayment.Member = targetMember;
                loanRepayment.LoanIssue = targetLoanIssue;
                loanRepayment.RepaymentIdEx = Convert.ToInt32(loanRepaymentObj.RepaymentId);
                loanRepayment.Amount = Convert.ToDouble(loanRepaymentObj.Amount);
                loanRepayment.BalanceBefore = Convert.ToDouble(loanRepaymentObj.BalanceBefore);
                loanRepayment.BalanceAfter = Convert.ToDouble(loanRepaymentObj.BalanceAfter);
                loanRepayment.InterestAmount = Convert.ToDouble(loanRepaymentObj.InterestAmount);
                loanRepayment.RolloverAmount = Convert.ToDouble(loanRepaymentObj.RolloverAmount);
                DateTime theDate = DateTime.Now;
                var strLastDateDue = Convert.ToString(loanRepaymentObj.LastDateDue);
                if (DateTime.TryParse(strLastDateDue, out theDate))
                {
                    loanRepayment.LastDateDue = theDate;
                }

                theDate = DateTime.Now;
                var strNextDateDue = Convert.ToString(loanRepaymentObj.NextDateDue);
                if (DateTime.TryParse(strNextDateDue, out theDate))
                {
                    loanRepayment.NextDateDue = theDate;
                }
                
                loanRepayment.Comments = Convert.ToString(loanRepaymentObj.Comments);
                
                //Save the changes 
                bool retValSave = false;
                if (loanRepayment.RepaymentId > 0)
                {
                    retValSave = loanRepaymentRepo.Update(loanRepayment);
                }
                else
                {
                    retValSave = loanRepaymentRepo.Insert(loanRepayment);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }


        private int ProcessFinesCollection(dynamic finesObj, int recordCount, string vslaCode, int meetingIdEx)
        {
            
            if (finesObj == null)
            {
                return -1;
            }

            VslaRepo vslaRepo = null;
            MeetingRepo meetingRepo = null;

            int processedCount = 0;

            try
            {
                //Otherwise, continue
                vslaRepo = new VslaRepo();
                meetingRepo = new MeetingRepo();

                //Retrieve the Target VSLA
                Vsla targetVsla = vslaRepo.FindVslaByCode(vslaCode);
                if (targetVsla == null)
                {
                    return -2; //Target VSLA Does not exist
                }

                //Retrieve the target Meeting
                var targetMeeting = meetingRepo.FindMeetingByIdEx(vslaCode, Convert.ToInt32(meetingIdEx));
                if (targetMeeting == null)
                {
                    return -3; //Target Meeting does not exist
                }

                foreach (var fineObj in finesObj)
                {
                    var retVal = ProcessFineData(finesObj, targetMeeting, targetVsla);
                    if (retVal == 0)
                    {
                        processedCount++;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                return -99;
            }
        }

        private int ProcessFineData(dynamic fineObj, Meeting targetMeeting, Vsla targetVsla)
        {
            
            FineRepo fineRepo = null;
            MemberRepo memberRepo = null;
            Member targetMember = null;
            MeetingRepo meetingRepo = null;

            try
            {
                meetingRepo = new MeetingRepo();

                if (fineObj == null)
                {
                    return -1;
                }

                if (null == targetMeeting || null == targetVsla)
                {
                    return -2; //Target Meeting or VSLA is missing
                }

                //Get the target member
                memberRepo = new MemberRepo();
                targetMember = memberRepo.FindMemberByIdEx(targetVsla.VslaId, Convert.ToInt32(fineObj.MemberId));
                if (null == targetMember)
                {
                    return -3; //Target Member not Found
                }

                //Otherwise, continue                
                fineRepo = new FineRepo();
                Fine fine = null;


                //Check whether this record exists: Use the IssuedInMeetingId
                //var targetFine = fineRepo.FindFineByIdEx(targetMeeting.MeetingId, Convert.ToInt32(fineObj.FineId));
                var targetFine = fineRepo.FindFineByIdEx(Convert.ToInt32(fineObj.IssuedInMeetingId), Convert.ToInt32(fineObj.FineId));
                if (targetFine == null)
                {
                    fine = new Fine();
                }
                else
                {
                    fine = targetFine;
                }
                fine.IssuedInMeeting = meetingRepo.FindMeetingByIdEx(targetVsla.VslaCode, Convert.ToInt32(fineObj.IssuedInMeetingId));
                fine.PaidInMeeting = meetingRepo.FindMeetingByIdEx(targetVsla.VslaCode, Convert.ToInt32(fineObj.PaidInMeetingId));
                fine.Member = targetMember;
                fine.FineIdEx = Convert.ToInt32(fineObj.FineId);
                fine.Amount = Convert.ToDouble(fineObj.Amount);
                DateTime theDate = DateTime.Now;
                var strExpectedDate = Convert.ToString(fineObj.ExpectedDate);
                if (DateTime.TryParse(strExpectedDate, out theDate))
                {
                    fine.ExpectedDate = theDate;
                }

                theDate = DateTime.Now;
                var strDateCleared = Convert.ToString(fineObj.DateCleared);
                if (DateTime.TryParse(strDateCleared, out theDate))
                {
                    fine.DateCleared = theDate;
                }
                //fine.FineTypeName = Convert.ToString(fineObj.FineTypeName);

                //Ensure that 1 is interpreted as TRUE
                int isClearedFlg = Convert.ToInt32(fineObj.IsCleared);
                fine.IsCleared = (isClearedFlg == 1) ? true : false;


                //Save the changes 
                bool retValSave = false;
                if (fine.FineId > 0)
                {
                    retValSave = fineRepo.Update(fine);
                }
                else
                {
                    retValSave = fineRepo.Insert(fine);
                }

                if (retValSave)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -99;
            }
        }


        //This is an internal procedure that will be used to reprocess data submissions within the database
        //From the logs
        public SubmitVslaDataResponse ReProcessSubmissions(string username, string securityToken)
        {
            SubmitVslaDataResponse response = new SubmitVslaDataResponse();            
            string jsonString = string.Empty;
            response.StatusCode = -1;
            DataSubmission dataSubmission = null;
            DataSubmissionRepo repo = null;

            try
            {
                //Authentication
                string computedToken = DateTime.Today.ToString("yyMM");
                if(username != "SYSTEM" || securityToken != computedToken)
                {
                    response.StatusCode = -11;                    
                    return response;
                }
                //Retrieve the Data Submissions existing in the database
                //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "Retrieving Data from Database...");
                repo = new DataSubmissionRepo();
                List<DataSubmission> dataSubmissions = null;
                dataSubmissions = repo.RetrieveSubmissions();

                //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "Looping through the returned data...");
                int failedResubmissions = 0;
                foreach( var submission in dataSubmissions)
                {
                    try
                    {
                        //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "Processing submission: retrieving JSON string...");
                        jsonString = submission.Data;   //Get the raw Submission JSON request
                        dynamic obj = JObject.Parse(jsonString);

                        if (obj == null)
                        {
                            failedResubmissions++;
                            continue;
                        }
                        var headerInfo = obj.HeaderInfo;

                        //Data Item: this will be an Enum
                        string dataItem = string.Empty;

                        //{"HeaderInfo":{"VslaCode":"V1004","PhoneImei":"356422050612411","NetworkOperator":"MTN-UGANDA","NetworkType":"EDGE","DataItem":"cycleInfo"}
                        //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "Processing the HeaderInfo...");
                        if (null != headerInfo)
                        {
                            dataSubmission = new DataSubmission();
                            dataSubmission.SourceVslaCode = Convert.ToString(headerInfo.VslaCode);
                            dataSubmission.SourcePhoneImei = Convert.ToString(headerInfo.PhoneImei);
                            dataSubmission.SourceNetworkOperator = Convert.ToString(headerInfo.NetworkOperator);
                            dataSubmission.SourceNetworkType = Convert.ToString(headerInfo.NetworkType);
                            dataSubmission.SubmissionTimestamp = DateTime.Now;
                            dataSubmission.Data = jsonString;
                            dataItem = Convert.ToString(headerInfo.DataItem);
                        }

                        //If DataItem is null then skip
                        if(string.IsNullOrWhiteSpace(dataItem))
                        {
                            //Skip this Resubmission
                            continue;
                        }
                        //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "JSON String: " + Environment.NewLine + jsonString);

                        //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "Doing the Reprocessing...");
                        if (dataSubmission != null)
                        {
                            //Dont save the raw data again, since this is a resubmission
                            //Process the data
                            int retValDataProcessed = -1;

                            //DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("INFO", "About to process Data Item: [" + dataItem + "...");
                            if (dataItem.ToUpper() == "CYCLEINFO")
                            {
                                //Retrieve the Information about the VSLA CYCLE and populate it into the database
                                var vslaCycleData = obj.VslaCycle;

                                //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                                retValDataProcessed = ProcessVslaCycleData(vslaCycleData, dataSubmission.SourceVslaCode);
                            }
                            else if (dataItem.ToUpper() == "MEETINGDETAILS")
                            {
                                //Retrieve the Information about the MEETING and populate it into the database
                                var meetingData = obj.Meeting;

                                //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                                retValDataProcessed = ProcessMeetingData(meetingData, dataSubmission.SourceVslaCode);
                            }
                            else if (dataItem.ToUpper() == "MEMBERS")
                            {
                                //Retrieve the Information about the MEETING and populate it into the database
                                var membersObj = obj.Members;
                                var recordCount = Convert.ToInt32(obj.MemberCount);

                                //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                                retValDataProcessed = ProcessMembersCollection(membersObj, recordCount, dataSubmission.SourceVslaCode);
                            }
                            else if (dataItem.ToUpper() == "ATTENDANCE")
                            {
                                //Retrieve the Information about the MEETING and populate it into the database
                                var attendancesObj = obj.Attendances;
                                var recordCount = Convert.ToInt32(obj.MembersCount);
                                var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                                //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                                retValDataProcessed = ProcessAttendancesCollection(attendancesObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                            }
                            else if (dataItem.ToUpper() == "SAVINGS")
                            {
                                //Retrieve the Information about the SAVINGS and populate it into the database
                                var savingsObj = obj.Savings;
                                var recordCount = Convert.ToInt32(obj.MembersCount);
                                var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                                //Pass the dynamic obj retrieved from the JSON string representing a VslaCycle
                                retValDataProcessed = ProcessSavingsCollection(savingsObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                            }
                            else if (dataItem.ToUpper() == "LOANS")
                            {
                                //Retrieve the Information about the LOANS and populate it into the database
                                var loanIssuesObj = obj.Loans;
                                var recordCount = Convert.ToInt32(obj.MembersCount);
                                var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                                //Pass the dynamic obj retrieved from the JSON string representing a LoanIssues array
                                retValDataProcessed = ProcessLoanIssuesCollection(loanIssuesObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                            }
                            else if (dataItem.ToUpper() == "REPAYMENTS")
                            {
                                //Retrieve the Information about the REPAYMENTS and populate it into the database
                                var loanRepaymentsObj = obj.Repayments;
                                var recordCount = Convert.ToInt32(obj.MembersCount);
                                var meetingIdEx = Convert.ToInt32(obj.MeetingId);

                                //Pass the dynamic obj retrieved from the JSON string representing a LoanRepayments array
                                retValDataProcessed = ProcessLoanRepaymentsCollection(loanRepaymentsObj, recordCount, dataSubmission.SourceVslaCode, meetingIdEx);
                            }

                            //return the response code
                            response.StatusCode = retValDataProcessed;
                        }
                    }
                    catch(Exception exReprocess)
                    {
                        DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("Errors", exReprocess.Message + Environment.NewLine + exReprocess.StackTrace);
                        failedResubmissions++;
                    }
                }                
            }
            catch(Exception ex)
            {
                DigitizingDataBizLayer.Helpers.AppGlobals.LogToFileServer("Errors", ex.Message + Environment.NewLine + ex.StackTrace);
                response.StatusCode = -99;
            }

            return response;
        }

        public HealthStatsResponse GetHealthStats(string username, string securityToken)
        {
            HealthStatsResponse response = new HealthStatsResponse();
            VslaDdActivationRepo ddActivationRepo = null;
            VslaRepo vslaRepo = null;
            VslaActivationStats vslaActivationStats = null;
            VslaDataSubmissionStats vslaDataSubmissionStats = null;
            DataSubmissionRepo dataSubmissionRepo = null;
            DataSubmission dataSubmission = null;

            string jsonString = string.Empty;
            response.StatusCode = -1;

            //Authentication
            string computedToken = DateTime.Today.ToString("yyMM");
            if (username != "SYSTEM" || securityToken != computedToken)
            {
                //Failed Authentication
                response.StatusCode = -11;
                return response;
            }

            try
            {
                //VSLA Activations Stats
                vslaActivationStats = new VslaActivationStats();
                vslaRepo = new VslaRepo();
                var registeredVslas = vslaRepo.FindAll();
                vslaActivationStats.CountOfRegisteredVslas = registeredVslas.Count();

                ddActivationRepo = new VslaDdActivationRepo();
                var activatedVslas = ddActivationRepo.FindAll();
                vslaActivationStats.CountOfActivatedVslas = activatedVslas.Count();

                //Get the Most Recent Activation
                if (activatedVslas.Count() > 0)
                {
                    var orderedActivations = activatedVslas.OrderByDescending(a => a.ActivationId);
                    var recentDdVslaActivation = orderedActivations.ElementAt(0);

                    if(recentDdVslaActivation != null && recentDdVslaActivation.Vsla != null)
                    {
                        var theVsla = vslaRepo.FindVslaById(recentDdVslaActivation.Vsla.VslaId);
                        vslaActivationStats.LastActivatedVsla = theVsla.VslaCodeAndName;
                        vslaActivationStats.LastActivationTimestamp = recentDdVslaActivation.ActivationDate.GetValueOrDefault();
                    }
                }


                //Data Submission Stats
                vslaDataSubmissionStats = new VslaDataSubmissionStats();
                dataSubmissionRepo = new DataSubmissionRepo();
                //TODO: Change this to Server Side Query instead of pulling all the records to the client
                vslaDataSubmissionStats.CountOfSubmittedMeetings = dataSubmissionRepo.RetrieveSubmissions().Count();
                var mostRecentSubmission = dataSubmissionRepo.GetMostRecentDataSubmission();
                if(null != mostRecentSubmission)
                {
                    vslaDataSubmissionStats.LastSubmissionTimestamp = mostRecentSubmission.SubmissionTimestamp.GetValueOrDefault();
                    vslaDataSubmissionStats.LastSubmittedVsla = mostRecentSubmission.SourceVslaCode;
                }

                response.VslaDataSubmissionStats = vslaDataSubmissionStats;
                response.VslaActivationStats = vslaActivationStats;

                //Return final status code
                response.StatusCode = 0;
            }
            catch
            {
                //System Error
                response.StatusCode = -99;
            }

            return response;
        }
    }
}