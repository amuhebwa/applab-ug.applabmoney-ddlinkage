using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class Users
    {
        public virtual int UserId { get; set; }
        public virtual string Username { get; set; }
        public virtual string Fullname { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual string UserLevel { get; set; }
    }
}
