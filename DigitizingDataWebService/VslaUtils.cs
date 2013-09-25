using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Security;

namespace DigitizingDataWebService
{
    public class VslaUtils
    {
        public const int MIN_RANDOM_PASSKEY = 11111;
        public const int MAX_RANDOM_PASSKEY = 99999;

        public static int GeneratePassKey(int min, int max)
        {           
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[4];
    
            rng.GetBytes(buffer);
            int result = BitConverter.ToInt32(buffer, 0);

            return new Random(result).Next(min, max);
        }

        public static int GeneratePassKey()
        {
            return GeneratePassKey(MIN_RANDOM_PASSKEY, MAX_RANDOM_PASSKEY);
        }

        public static string ActivateVslaForDigitizingData(string vslaCode, string sourceImei)
        {
            return "Activated";
        }

    }
}