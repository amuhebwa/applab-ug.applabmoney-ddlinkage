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
    /**
     * CBT Logs in
     */
    [DataContract]
    public class CBTLoginDetails 
    {
        [DataMember]
        public int result { get; set; } // 1 for success, 0 for failed login
        [DataMember]
        public int TechnicalTrainerId { get; set; } // User Id of the CBT
        [DataMember]
        public string Username { get; set; }
    }
    /**
     * Users field models
     */
    [DataContract]
    public class UsersDetails
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
    /**
     * VSLA fields model
     */
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
        public string VslaPhoneMsisdn { get; set; }
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
        public string GroupRepresentativeName { get; set; }
        [DataMember]
        public string GroupRepresentativePosition { get; set; }
        [DataMember]
        public string GroupRepresentativePhonenumber { get; set; }
        [DataMember]
        public int? TechnicalTrainerId { get; set; }
        [DataMember]
        public string TechnicalTTrainerName { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string GroupAccountNumber { get; set; }
    }

    /**
     *  
     */
    [DataContract]
    public class RegistrationResult {
        [DataMember]
        public string result { get; set; }
        [DataMember]
        public string VslaCode { get; set; } // returned on sucessful vsla registration
        [DataMember]
        public string operation { get; set; } // Creating/Editing
    }
}