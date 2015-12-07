using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DigitizingDataAdminWebService
{
    public class UsersInformation
    {

    }
    [DataContract]
    public class DataContracts
    {
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Fullname { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public DateTime? DateCreated { get; set; }
        [DataMember]
        public int UserLevel { get; set; }

    }

    // TRAINER DATA
    [DataContract]
    public class TechnicalTrainer
    {
        [DataMember]
        public int resultId { get; set; } // 1-Sucess, 0-Failed
        [DataMember]
        public int TrainerId { get; set; }
        [DataMember]
        public string userName { get; set; }
    }

    // VSLA DATA
    [DataContract]
    public class VslaDetails
    {
        [DataMember]
        public int VslaId { get; set; }
        [DataMember]
        public string VslaCode { get; set; } // This is auto-generated
        [DataMember]
        public string VslaName { get; set; }
        [DataMember]
        public string grpPhoneNumber { get; set; }
        [DataMember]
        public string PhysicalAddress { get; set; }
        [DataMember]
        public string GpsLocation { get; set; }
        [DataMember]
        public string DateRegistered { get; set; }
        [DataMember]
        public string DateLinked { get; set; }
        [DataMember]
        public string RegionName { get; set; }
        [DataMember]
        public string representativeName { get; set; }
        [DataMember]
        public string representativePosition { get; set; }
        [DataMember]
        public string repPhoneNumber { get; set; }
        [DataMember]
        public int tTrainerId { get; set; }
        [DataMember]
        public string tTrainerName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string GroupAccountNumber { get; set; }
        [DataMember]
        public string GroupSupport { get; set; }
    }

    // REGISTRATION RESULT
    [DataContract]
    public class OperationResult
    {
        [DataMember]
        public string result { get; set; }
        [DataMember]
        public string VslaCode { get; set; } // returned on sucessful vsla registration
        [DataMember]
        public string operation { get; set; } // Creating/Editing
    }

    // SUPPRT TYPE
    [DataContract]
    public class GroupSupportType
    {
        [DataMember]
        public string SupportType { get; set; }
        [DataMember]
        public int VslaId { get; set; }
        [DataMember]
        public int TrainerId { get; set; }
        public DateTime SupportDate { get; set; }
    }
}