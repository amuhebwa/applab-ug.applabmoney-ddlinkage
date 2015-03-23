using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllSingleMeetingProcedures
    {
        public List<SingleMeetingProcedures> allMeetingData { get; set; }
    }
    public class SingleMeetingProcedures
    {
        public int Id { get; set; }
        public string isPresent { get; set; }
        public string memberName { get; set; }
        public decimal amountSaved { get; set; }
        public decimal finedAmount { get; set; }
        public decimal principleAmount { get; set; }
        public int loanNumber { get; set; }
    }
}