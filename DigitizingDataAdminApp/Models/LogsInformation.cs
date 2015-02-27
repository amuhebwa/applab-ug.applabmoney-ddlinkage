using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Models
{
    public class AllLogsInformation
    {
        public List<LogsInformation> AllLogsList { get; set; }
        public LogsInformation LogDataList { get; set; }
    }
    public class LogsInformation
    {
        public int LogId { get; set; }
        public string userId { get; set; }
        public DateTime LogTime{get; set;}
        public string ActionPerformed { get; set; }
    }
}