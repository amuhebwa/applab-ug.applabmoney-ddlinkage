//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitizingDataAdminApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cbt_info
    {
        public Cbt_info()
        {
            this.Vslas = new HashSet<Vsla>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Region { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
    
        public virtual VslaRegion VslaRegion { get; set; }
        public virtual ICollection<Vsla> Vslas { get; set; }
    }
}
