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
    
    public partial class Saving
    {
        public int SavingId { get; set; }
        public Nullable<int> SavingIdEx { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> MeetingId { get; set; }
        public Nullable<int> MemberId { get; set; }
    
        public virtual Meeting Meeting { get; set; }
        public virtual Member Member { get; set; }
    }
}
