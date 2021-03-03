using CoreScanner;
using AppDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHolographyH1.Scaners
{
    public static class COM
    {
        public static CCoreScanner CoreScannerObject { get; private set; }
        public static int Status { get; private set; }

        static COM()
        {
            CoreScannerObject = new CCoreScanner();
        }
        public static void OpenConnection()
        {
            int appHandle = 0;
            const short NumberOfScannerTypes = 1;
            short[] scannerTypes = new short[NumberOfScannerTypes];
            scannerTypes[0] = (short)ScannerType.All; //  All scanner types
            int status = -1;
            try
            {
                // Open CoreScanner COM Object
                CoreScannerObject.Open(appHandle,            // Application handle     
                                       scannerTypes,         // Array of scanner types    
                                       NumberOfScannerTypes, // Length of scanner types array 
                                       out status);          // Command execution success/failure return status 
            }
            catch
            {
                Logger.Write("Com connection for scanner is open");
            }
            Status =  status;
        }

        public static void CloseConnection()
        {
            int appHandle = 0;
            int status = -1;

            // Close CoreScanner COM Object
            CoreScannerObject.Close(appHandle,   // Application handle
                                    out status); // Command execution success/failure return status 

            if (status == (int)AppDefs.Status.Success)
            {
                Logger.Write("Com connection for scanner is closed");
            }
            else
            {
                Logger.Write("Com connection for scanner closing failed; COM.CloseConnection()");
            }
            Status = status;
        }
    }
}
