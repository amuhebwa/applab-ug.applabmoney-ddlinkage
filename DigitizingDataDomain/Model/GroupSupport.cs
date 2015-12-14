using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class GroupSupport
    {
        public virtual int SupportId { get; set; }
        public virtual string SupportType { get; set; }
        public virtual Vsla VslaId { get; set; }
        public virtual TechnicalTrainer TrainerId { get; set; }
        public virtual DateTime? SupportDate { get; set; }
    }
}
