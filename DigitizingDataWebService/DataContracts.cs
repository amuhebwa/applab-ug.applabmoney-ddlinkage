using System.Runtime.Serialization;

[DataContract]
public class ActivateVslaForDdRequest
{
    [DataMember]
    public string VslaCode { get; set; }
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