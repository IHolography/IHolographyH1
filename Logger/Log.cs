using System;
using System.IO;
using iHolography.Logger.Deffs;

namespace iHolography
{
    namespace Logger
    {
        public static class Log
        {
            public static int LogStatus { get; set; }
            public static string LogFilePath { get; set; }
            public static bool LogEnable { get; set; }
            public static string DateTimeFormat { get; set; }

            static Log()
            {
                LogStatus = (int)Status.Failed;

                LogFilePath = @"C:\Users\Public\Documents\";
                LogEnable = true;
                DateTimeFormat = "G";
            }

            private static async void WriteDoc(string message)
            {
                try
                {
                    if (LogEnable)
                    {
                        string filePath = LogFilePath + "//LogFile_App.txt";
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
                            throw new Exception("Logger cann't write file.");
                        }
                    }
                }
                catch
                {
                    throw new NullReferenceException();
                }
            }

            public static void Write(string message)
            {
                WriteDoc($"date: {Variable.GetStringDateTime(DateTimeFormat)} ; message: {message}");
            }
            public static void Write(string message, object sender)
            {
                WriteDoc($"date: {Variable.GetStringDateTime(DateTimeFormat)} ; sender:{sender}; message: {message}");
            }
        }
    }
}
