using System;

namespace iHolography
{
    namespace Logger.Deffs
    {
        #region Enum
        public enum Alm
        {
            Undefined = 0,
            Alarm = 1,
            Ok = 2
        }
        public enum ScannerAction
        {
            Undefined = 0,
            BoxScan = 1,
            ProductScan = 2
        }

        public enum Status
        {
            Success = 0,
            Locked = 10,
            Failed = -1
        }


        #endregion
        public static class Constant
        {

        }
        public static class Variable
        {
            static Variable()
            {

            }
            private static string GetDateTimeFormat(string value)
            {
                char[] chars = new char[] { 'D', 'F', 'G', 'M', 'O', 'R', 'T', 'U', 'Y' };
                char ch = value[0];
                char format = 'G';
                foreach (char i in chars)
                {
                    if (char.ToUpper(ch) == i)
                    {
                        format = ch;
                        break;
                    }
                }
                return format.ToString();
            }
            public static string GetStringDateTime(string dateTimeFormat)
            {
                return DateTime.Now.ToString(dateTimeFormat);
            }

        }

    }
}
