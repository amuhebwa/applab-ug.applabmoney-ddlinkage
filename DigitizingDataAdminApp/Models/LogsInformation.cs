using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

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

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LogTime { get; set; }
        public string formattedLogTime { get { return LogTime.Value.ToShortDateString(); } }

        public string ActionPerformed { get; set; }
    }
}