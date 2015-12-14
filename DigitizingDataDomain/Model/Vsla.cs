using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class Vsla
    {
        public virtual int VslaId { get; set; }
        public virtual string VslaCode { get; set; }
        public virtual string VslaName { get; set; }
        public virtual VslaRegion VslaRegion { get; set; }
        public virtual DateTime? DateRegistered { get; set; }
        public virtual DateTime? DateLinked { get; set; }
        public virtual string PhysicalAddress { get; set; }
        public virtual string VslaPhoneMsisdn { get; set; }
        public virtual string GpsLocation { get; set; }
        public virtual string ContactPerson { get; set; }
        public virtual string PositionInVsla { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual Cbt_info CBT { get; set; }
        public virtual int Status { get; set; }
        public virtual string GroupAccountNumber { get; set; }
        public virtual int NumberOfCycles { get; set; }

        //Collections: Hacked to allow for reporting
        public virtual IList<VslaDdActivation> VslaDdActivationList { get; set; }
        public virtual AggregationBindingList<VslaDdActivation> VslaDdActivations
        {
            get
            {
                return new AggregationBindingList<VslaDdActivation>(VslaDdActivationList);
            }
        }

        //Members
        public virtual IList<Member> MemberList { get; set; }
        public virtual AggregationBindingList<Member> Members
        {
            get
            {
                return new AggregationBindingList<Member>(MemberList);
            }
        }

        //VslaCycles
        public virtual IList<VslaCycle> VslaCycleList { get; set; }
        public virtual AggregationBindingList<VslaCycle> VslaCycles
        {
            get
            {
                return new AggregationBindingList<VslaCycle>(VslaCycleList);
            }
        }

        //Display both the VSLA Code and VSLA Name
        public virtual string VslaCodeAndName
        {
            get { return VslaCode + ": " + VslaName; }
        }
    }
}
