using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class VslaDdActivation
    {
        public virtual int ActivationId {get; set;}
        public virtual Vsla Vsla { get; set; }
        public virtual string PhoneImei01 { get; set; }
        public virtual string PhoneImei02 { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime? ActivationDate { get; set; }
        public virtual string PassKey { get; set; }
        public virtual string SimSerialNo01 { get; set; }
        public virtual string SimImsiNo01 { get; set; }
        public virtual string SimNetworkOperator01 { get; set; }
        public virtual string SimSerialNo02 { get; set; }
        public virtual string SimImsiNo02 { get; set; }
        public virtual string SimNetworkOperator02 { get; set; }
    }
}

/*
VSLA_ID INT NOT NULL,
    PHONE_IMEI NVARCHAR(50) NOT NULL,
    IS_ACTIVE BIT DEFAULT 0,
    ACTIVATION_DT DATETIME,
    PASS_KEY NVARCHAR(50) NOT NULL,
	SIM_SERIAL_NUM NVARCHAR(50),
	SIM_IMSI_NUM NVARCHAR(50),
	SIM_NETWORK_OPERATOR NVARCHAR(50),
	SIM_NETWORK_TYPE NVARCHAR(50),
*/