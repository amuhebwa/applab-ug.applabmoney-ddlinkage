using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class Fine
    {
        public virtual int FineId { get; set; }
        public virtual int FineIdEx { get; set; }
        public virtual Member Member { get; set; }
        public virtual Meeting IssuedInMeeting { get; set; }
        public virtual double Amount { get; set; }
        public virtual string FineTypeName { get; set; }
        public virtual int FineTypeId { get; set; }
        public virtual DateTime? ExpectedDate { get; set; }
        public virtual Meeting PaidInMeeting { get; set; }
        public virtual bool IsCleared { get; set; }
        public virtual DateTime? DateCleared { get; set; }
    }
}


/*
private int fineId;
    private Meeting meeting;
    private Member member;
    private String fineTypeName;
    private int fineTypeId;
    private double amount;
    private Date expectedDate;
    private boolean isCleared;
    private Date dateCleared;
    private Meeting paidInMeeting;
*/