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
    
    public partial class VslaRegion
    {
        public VslaRegion()
        {
            this.Cbt_info = new HashSet<Cbt_info>();
        }
    
        public int RegionId { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
    
        public virtual ICollection<Cbt_info> Cbt_info { get; set; }
    }
}
