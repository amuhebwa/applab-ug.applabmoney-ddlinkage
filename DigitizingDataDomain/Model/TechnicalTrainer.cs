﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class TechnicalTrainer
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Passkey { get; set; }
        public virtual string Username { get; set; }
        public virtual string LastName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual VslaRegion VslaRegion { get; set; }
        public virtual string Status { get; set; }
    }
}
