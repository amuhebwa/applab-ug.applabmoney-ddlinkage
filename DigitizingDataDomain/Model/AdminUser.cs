using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class AdminUser
    {
        public virtual int UserId { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string SecurityToken { get; set; }
        public virtual string Surname { get; set; }
        public virtual string OtherNames { get; set; }
        public virtual string ActivationPhoneImei { get; set; }
        public virtual DateTime? ActivationDate { get; set; }
        public virtual AddressInfo AddressInfo { get; set; }
    }
}

/*
 * ADMIN_USER_ID INT NOT NULL IDENTITY(1,1),
	ADMIN_USER_NAME NVARCHAR(50) NOT NULL,
	ADMIN_PASSWORD NVARCHAR(50),
	SECURITY_TOKEN NVARCHAR(50),
	SURNAME NVARCHAR(30) NOT NULL,
	OTHER_NAMES NVARCHAR(50),
	TELEPHONE_01 NVARCHAR(20),
	TELEPHONE_02 NVARCHAR(20),
	ACTIVATION_IMEI NVARCHAR(20),
	ACTIVATION_DT DATETIME,
*/