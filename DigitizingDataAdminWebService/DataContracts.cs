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
        public int CbtId { get; set; } // User Id of the CBT
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
    /**
     *--- Data Submitted From the phone to register a new phone ---
     * 1. Group information
     */
    [DataContract]
    class GroupInformation
    {
        [DataMember]
        public string GroupName { get; set; }
        [DataMember]
        public string GroupPasskey { get; set; }
        [DataMember]
        public string ContactPerson { get; set; }
        [DataMember]
        public string PositionInVsla { get; set; }
        [DataMember]
        public string MemberPhoneNumber { get; set; }
        [DataMember]
        public string GroupBankAccount { get; set; }
    }
    /**
     * 2. Phone information
     */
    [DataContract]
    public class PhoneInformation
    {
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string PhoneImei01 { get; set; }
        [DataMember]
        public string PhoneImei02 { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public string RecepientName { get; set; }
        [DataMember]
        public string RecipientPost { get; set; }
        [DataMember]
        public string DateDelivered { get; set; }
    }
    /**
     * 3. Location information
     */
    [DataContract]
    public class LocationInformation
    {
        [DataMember]
        public string PhysicalAddress { get; set; }
        [DataMember]
        public string RegionName { get; set; }
        [DataMember]
        public string GpsLocation { get; set; }
    }
}