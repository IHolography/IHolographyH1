using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using AppDefs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHolographyH1
{
    static class Logger
    {
        public static int LogStatus { get; private set; }

        static Logger()
        {
            LogStatus = (int)Status.Failed;
        }
        
        private static async void WriteDoc(string message)
        {
            if (Constant.LogEnable)
            {
                string filePath = Constant.LogFilePath + "//LogFile_IHolographyH1.txt";
                try
                {
                    if (!File.Exists(filePath))
                    {
                        using (StreamWriter sw = new StreamWriter(filePath, false, System.Text.Encoding.Default))
                        {
                            await sw.WriteLineAsync(message);
                            LogStatus = (int)Status.Success;
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(filePath, true, System.Text.Encoding.Default))
                        {
                            await sw.WriteLineAsync(message);
                            LogStatus = (int)Status.Success;
                        }
                    }
                }
                catch
                {
                    LogStatus = (int)Status.Failed;
                }
            }
        }
      
        public static void Write(string message)
        {
            WriteDoc($"date: {Variable.GetStringDateTime()} ; message: {message}");
        }
        public static void Write(string message, object sender)
        {
            WriteDoc($"date: {Variable.GetStringDateTime()} ; sender:{sender}; message: {message}");
        }

    }
}
