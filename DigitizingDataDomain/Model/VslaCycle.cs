using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitizingDataDomain.Collections;

namespace DigitizingDataDomain.Model
{
    public class VslaCycle
    {
        public virtual int CycleId { get; set; }
        public virtual int CycleIdEx { get; set; }
        public virtual Vsla Vsla { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual double InterestRate { get; set; }
        public virtual double SharePrice { get; set; }
        public virtual int MaxShareQuantity { get; set; }
        public virtual double MaxStartShare { get; set; }
        public virtual string CycleCode { get; set; }
        public virtual bool IsEnded { get; set; }
        public virtual DateTime? DateEnded { get; set; }
        public virtual double SharedAmount { get; set; }

        //Meetings
        public virtual IList<Meeting> MeetingList { get; set; }
        public virtual AggregationBindingList<Meeting> Meetings
        {
            get
            {
                return new AggregationBindingList<Meeting>(MeetingList);
            }
        }
    }
}

/*
 * CYCLE_ID INT NOT NULL IDENTITY(1,1),
    CYCLE_ID_EX INT NOT NULL,
    VSLA_ID INT NOT NULL,
    [START_DATE] DATETIME,
    END_DATE DATETIME,
    INTEREST_RATE DECIMAL(5,2) DEFAULT 0,
    CYCLE_CODE NVARCHAR(20),
    SHARE_PRICE DECIMAL(12,2) DEFAULT 0,
    MAX_SHARE_QUANTITY INT DEFAULT 0,
    MAX_START_SHARE DECIMAL(12,2) DEFAULT 0,
    IS_ENDED BIT DEFAULT 0,
    DATE_ENDED DATETIME,
    SHARED_AMOUNT DECIMAL(18,2) DEFAULT 0,
 * */
