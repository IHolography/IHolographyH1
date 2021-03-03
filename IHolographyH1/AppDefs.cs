using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDefs
{
    public enum Status
    {
        Success = 0,
        Locked = 10,
        Failed=-1
    }
    public static class Constant
    {
        public static string LogFilePath { get; private set; }
        public static string LogException { get; private set; }
        public static bool LogEnable { get; private set; }
        public static int LogFileSize { get; private set; }

        static Constant()
        {
            try
            {
                LogFilePath = GetLogPath(ConfigurationManager.AppSettings["LogFilePath"]);
                LogEnable = GetLogEnableFlag(ConfigurationManager.AppSettings["LogEnable"]);
                LogFileSize = GetLogFileSize(ConfigurationManager.AppSettings["LogFileSize"]);
            }
            catch
            {
                LogFilePath = @"C:\Users\Public\Documents\";
                LogEnable = false;
            }
        }
        private static bool GetLogEnableFlag(string value)
        {
            bool result = false;
            if (Boolean.TryParse(value, out bool parsedValue))
            {
                result = parsedValue;
            }
            return result;
        }
        private static int GetLogFileSize(string value)
        {
            int result = 5;
            if (Int32.TryParse(value, out int j))
            {
                result = j > 0 ? j : 5;
            }
            return result;
        }
        private static string GetLogPath(string value)
        {
            string dir = ConfigurationManager.AppSettings["LogFilePath"];
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    dir = @"C:\Users\Public\Documents\";
                }
            }
            return dir;
        }
        
    }
    public static class Variable
    {
        public static string DateTimeFormat { get; private set; }
        static Variable()
        {
            try
            {
                DateTimeFormat = GetDateTimeFormat(ConfigurationManager.AppSettings["DateTimeFormat"]);
            }
            catch
            {
                DateTimeFormat = "G";
            }
        }
        private static string GetDateTimeFormat(string value)
        {
            char[] chars = new char[] { 'D', 'F', 'G', 'M', 'O', 'R', 'T', 'U', 'Y' };
            char ch = value[0];
            char format='G';
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
        public static string GetStringDateTime()
        {
            return DateTime.Now.ToString(DateTimeFormat); 
        }
        
    }

}
