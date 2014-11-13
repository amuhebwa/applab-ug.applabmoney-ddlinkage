using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitizingDataDomain.Model
{
    public class DataSubmission
    {
        public virtual int SubmissionId { get; set; }
        public virtual string SourcePhoneImei { get; set; }
        public virtual string SourceVslaCode { get; set; }
        public virtual string SourceNetworkOperator { get; set; }
        public virtual string SourceNetworkType { get; set; }
        public virtual DateTime? SubmissionTimestamp { get; set; }
        public virtual string Data { get; set; }
        public virtual bool ProcessedFlag { get; set; }
    }
}
