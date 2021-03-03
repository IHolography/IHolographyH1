using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDefs;

namespace IHolographyH1
{
        public class DataScan
        {
            public string Barcode { get; set; }
            public string ScannerSN { get; set; }
            public string Symbology { get; set; }
            public string ScannerID { get; set; }
            public ScannerAction ScannerAction { get; set; }
        public string CreateDateTime { get; private set; }
            public DataScan(string barcode, string scannerSN, string symbology, string scannerID, ScannerAction scannerAction)
            {
                Barcode = barcode;
                ScannerSN = scannerSN;
                Symbology = symbology;
                ScannerID = scannerID;
                ScannerAction = scannerAction;
                CreateDateTime = Variable.GetStringDateTime();
            }
            public override string ToString()
            {
                return $"Data scan: action - {ScannerAction}, barcode - {Barcode}, ScannerID - {ScannerID}, ScannerSN - {ScannerSN}, Symbology - {Symbology}, Date - {CreateDateTime}";
            }

        }
}
