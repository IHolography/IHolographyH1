using System.IO.Ports;
using iHolography.Logger;

namespace iHolography
{
    namespace WeigherService
    {
        public class Weigher
        {
            public delegate void WeigherDone(float value);
            public event WeigherDone WeighFinish;
            public int DataFromWeigherSelectionCount { get; set; }
            public int StatusWeigher { get; private set; }


            #region LoggerProperties
            public static int LogStatus { get; set; }
            public static string LogFilePath { get; set; }
            public static bool LogEnable { get; set; }
            public static string DateTimeFormat { get; set; }
            #endregion

            int dataFromWeigherCount;


            float[] dataArr;

            public Weigher(string serialPortName, int baudRate, Parity parity, StopBits stopBits, int dataBits, int dataFromWeigherSelectionCount, bool logEnable, string dateTimeFormat, string logFilePath)
            {
                Log.LogEnable = COM.LogEnable = Parser.LogEnable = LogEnable = logEnable;
                Log.LogFilePath = COM.LogFilePath = Parser.LogFilePath = LogFilePath = logFilePath;
                Log.DateTimeFormat = COM.DateTimeFormat = Parser.DateTimeFormat = DateTimeFormat = dateTimeFormat;

                DataFromWeigherSelectionCount = dataFromWeigherSelectionCount >= 5 ? dataFromWeigherSelectionCount : 5;
                StatusWeigher = (int)COM.Open(serialPortName, baudRate, parity, stopBits, dataBits);
            }
            public void GetData()
            {
                dataFromWeigherCount = 0;

                dataArr = new float[DataFromWeigherSelectionCount - 2];
                try
                {
                    if (COM.mySerialPort.IsOpen == true)
                    {
                        COM.Close();
                    }
                    COM.mySerialPort.Open();
                    COM.mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                    Log.Write("Scale subscribe for weigh event");
                }
                catch { }
            }
            private void AddDataInArray(object sender)
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadLine();
                if (dataFromWeigherCount != 0)
                {
                    dataArr[dataFromWeigherCount - 1] = Parser.GetFloatValue(indata);
                }
            }
            private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
            {
                if (dataFromWeigherCount < DataFromWeigherSelectionCount - 1)
                {
                    AddDataInArray(sender);
                    dataFromWeigherCount++;
                }
                else
                {
                    float value = Parser.GetFilteredWeight(dataArr);
                    Log.Write($"Weigh done. Finish weigh={value}");
                    WeighFinish?.Invoke(value);
                    dataArr = null;
                    COM.mySerialPort.DataReceived -= DataReceivedHandler;
                    COM.Close();
                }
            }
        }
    }
}
