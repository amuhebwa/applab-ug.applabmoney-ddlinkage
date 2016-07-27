using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class UserPermissions
    {
        public virtual int Level_Id { get; set; }
        public virtual string UserType { get; set; }
    }
}
