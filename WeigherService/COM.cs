using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iHolography.Logger;

namespace iHolography
{
    namespace WeigherService
    {
        public static class COM
        {

            #region LoggerProperties
            public static int LogStatus { get; set; }
            public static string LogFilePath { get; set; }
            public static bool LogEnable { get; set; }
            public static string DateTimeFormat { get; set; }
            #endregion
            public static string SerialPortName { get; private set; }
            public static int BaudRate { get; private set; }
            public static Parity Parity { get; private set; }
            public static StopBits StopBits { get; private set; }
            public static SerialPort MySerialPort { get; private set; }
            public static int DataBits { get; private set; }

            public static SerialPort mySerialPort;
            static COM()
            {
                mySerialPort = new SerialPort();
                MySerialPort = mySerialPort;
            }

            public static int Open(string serialPortName, int baudRate, Parity parity, StopBits stopBits, int dataBits)
            {
                #region Initialize
                SerialPortName = serialPortName;
                BaudRate = baudRate;
                Parity = parity;
                StopBits = stopBits;
                DataBits = dataBits;
                Status status = Status.Failed;
                mySerialPort = new SerialPort(SerialPortName);

                mySerialPort.BaudRate = BaudRate;
                mySerialPort.Parity = Parity;
                mySerialPort.StopBits = StopBits;
                mySerialPort.DataBits = DataBits;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.RtsEnable = true;
                #endregion

                try
                {
                    mySerialPort.Open();
                    status = Status.Success;
                    Log.Write($"Com port for weigher opened");
                }
                catch (Exception ex)
                {
                    Log.Write($"Can't open Com port for weigher. Reason:{ex}");
                }
                return (int)status;
            }

            public static int Close()
            {
                Status status = Status.Failed;
                try
                {
                    mySerialPort.Close();
                    status = Status.Success;
                    Log.Write($"Com port for weigher closed");
                }
                catch (Exception ex)
                {
                    Log.Write($"Can't close Com port for weigher. Reason:{ex}");
                }
                return (int)status;
            }

        }
    }
}