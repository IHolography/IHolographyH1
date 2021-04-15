
using System;

namespace iHolography
{
    namespace ScannerService
    {
        public class Scanner : ICloneable
        {
            public string ScannerID { get; private set; }
            public string Serialnumber { get; private set; }
            public string GUID { get; private set; }
            public string VID { get; private set; }
            public string PID { get; private set; }
            public string Modelnumber { get; private set; }
            public string DoM { get; private set; }
            public string Firmware { get; private set; }
            public Alm ScannerException { get; private set; }
            public Scanner(string ScannerID, string Serialnumber, string GUID, string VID, string PID, string Modelnumber, string DoM, string Firmware)
            {
                ScannerException = Alm.Ok;
                this.ScannerID = ScannerID;
                this.Serialnumber = Serialnumber;
                this.GUID = GUID;
                this.VID = VID;
                this.PID = PID;
                this.Modelnumber = Modelnumber;
                this.DoM = DoM;
                this.Firmware = Firmware;
            }
            public void SetException()
            {
                ScannerException = Alm.Alarm;
            }
            public void ResetException()
            {
                ScannerException = Alm.Ok;
            }
            public object Clone()
            {
                return this.MemberwiseClone();
            }

        }
    }
}
