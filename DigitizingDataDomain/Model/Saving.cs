using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class Saving
    {
        public virtual int SavingId { get; set; }
        public virtual int SavingIdEx { get; set; }
        public virtual Member Member { get; set; }
        public virtual Meeting Meeting { get; set; }
        public virtual double Amount { get; set; }
    }
}

/*
 * SAVING_ID INT NOT NULL IDENTITY(1,1),
    SAVING_ID_EX INT NOT NULL,
    MEMBER_ID INT NOT NULL,
    MEETING_ID INT NOT NULL,
    AMOUNT DECIMAL(18,2) NOT NULL,
 * */
