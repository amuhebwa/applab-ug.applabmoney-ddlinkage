using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigitizingDataWebService
{    
    public class DataProcessingEngine
    {
        public static DataProcessingResult ProcessVslaData(string jsonData)
        {
            //Read the JsonData

            //Retrieve the kind of Data Item

            //Call the appropriate method
            return ProcessCycleInfo(jsonData);
        }

        private static DataProcessingResult ProcessCycleInfo(string jsonData)
        {
            return new DataProcessingResult { StatusCode = 0, StatusMessage = "SUCCESS" };
        }
    }
}