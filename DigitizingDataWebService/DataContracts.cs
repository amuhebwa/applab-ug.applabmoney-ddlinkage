using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

[DataContract]
public class ActivateVslaForDdRequest
{
    [DataMember]
    public string VslaCode { get; set; }
    [DataMember]
    public string PassKey { get; set; }
    [DataMember]
    public string PhoneImei { get; set; }
    [DataMember]
    public string SimImsi { get; set; }
    [DataMember]
    public string SimSerialNo { get; set; }
    [DataMember]
    public string NetworkOperatorName { get; set; }
    [DataMember]
    public string NetworkType { get; set; }
}

[DataContract]
public class ActivateVslaForDdResponse
{
    [DataMember]
    public bool IsActivated { get; set; }
    [DataMember]
    public string VslaName { get; set; }
    [DataMember]
    public string PassKey { get; set; }
}

[DataContract]
public class VslaInfo
{
    [DataMember]
    public int VslaId { get; set; }
    [DataMember]
    public string VslaCode { get; set; }
    [DataMember]
    public string VslaName { get; set; }    
}

[DataContract]
public class ActivateAdminUserResponse
{
    [DataMember]
    public int ActivationStatus { get; set; }
    [DataMember]
    public List<VslaInfo> VslaList { get; set; }
}

[DataContract]
public class ActivateAdminUserRequest
{
    [DataMember]
    public string PhoneImei { get; set; }
    [DataMember]
    public string AdminUserName { get; set; }
    [DataMember] 
    public string SecurityToken { get; set; }
    [DataMember]
    public string AdminPassword { get; set; }    
}

[DataContract]
public class DeliverVslaKitRequest
{
    [DataMember]
    public string VslaCode { get; set; }
    [DataMember]
    public string VslaPhoneImei { get; set; }
    [DataMember]
    public string VslaPhoneMsisdn { get; set; }
    [DataMember]
    public string PhoneSerialNumber { get; set; }
    [DataMember]
    public string PhoneManufacturer { get; set; }
    [DataMember]
    public string PhoneModel { get; set; }
    [DataMember]
    public string AdminUserName { get; set; }
    [DataMember]
    public string SecurityToken { get; set; }
    [DataMember]
    public string PhoneImei { get; set; }
    [DataMember]
    public string ReceivedBy { get; set; }
    [DataMember]
    public string RecipientRole { get; set; }
    [DataMember]
    public string DeliveryDate { get; set; }
}

[DataContract]
public class DeliverVslaKitResponse
{
    [DataMember]
    public int DeliveryStatus { get; set; }
}

[DataContract]
public class CaptureGpsLocationRequest
{
    [DataMember]
    public string VslaCode { get; set; }    
    [DataMember]
    public string AdminUserName { get; set; }
    [DataMember]
    public string SecurityToken { get; set; }
    [DataMember]
    public string PhoneImei { get; set; }
    [DataMember]
    public string GpsLocation { get; set; }    
}

[DataContract]
public class CaptureGpsLocationResponse
{
    [DataMember]
    public int StatusCode { get; set; }
}


[DataContract]
public class SubmitVslaDataResponse
{
    [DataMember]
    public int StatusCode { get; set; }    
}

[DataContract]
public class VslaActivationStats
{
    [DataMember]
    public int CountOfRegisteredVslas { get; set; }
    [DataMember]
    public int CountOfActivatedVslas { get; set; }
    [DataMember]
    public string LastActivatedVsla { get; set; }
    [DataMember]
    public DateTime LastActivationTimestamp { get; set; }
}

[DataContract]
public class VslaDataSubmissionStats
{
    [DataMember]
    public int CountOfSubmittedMeetings { get; set; }
    [DataMember]
    public string LastSubmittedVsla { get; set; }
    [DataMember]
    public DateTime LastSubmissionTimestamp { get; set; }
}

[DataContract]
public class HealthStatsResponse
{
    [DataMember]
    public int StatusCode { get; set; } 
    
    //VSLA Activations Stats
    [DataMember]
    public VslaActivationStats VslaActivationStats { get; set; }


    //Submissions Stats
    [DataMember]
    public VslaDataSubmissionStats VslaDataSubmissionStats { get; set; }
}
