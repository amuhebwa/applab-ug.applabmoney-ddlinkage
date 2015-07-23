using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataAdminApp.Controllers
{
    public class ActivityLoggingSystem
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public ActivityLoggingSystem()
        {
        }
        /**
         * Type 0 : INFORMATION
         *      1 : ERROR
         */
        public void logActivity(String log, int type)
        {
            switch (type)
            {
                case 0:
                    logger.Info(log);
                    break;
                case 1:
                    logger.Error(log);
                    break;
                default:
                    break;
            }

        }
    }
}