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
        public string VslaCode { get; set; }
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
        public string ContactPerson { get; set; }
        [DataMember]
        public string PositionInVsla { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string CbtName { get; set; }
        [DataMember]
        public string Status { get; set; }
    }
}