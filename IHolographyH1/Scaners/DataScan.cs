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
            public string Barcode { get; private set; }
            public string Symbology { get; private set; }
            public ScannerAction ScannerAction { get; private set; }
            public Scanner Scanner { get; private set; }
        public string CreateDateTime { get; private set; }
            public DataScan(string barcode, string symbology, ScannerAction scannerAction, Scanner scanner)
            {
                Barcode = barcode;
                Symbology = symbology;
                ScannerAction = scannerAction;
                Scanner = scanner;
                CreateDateTime = Variable.GetStringDateTime();
            }
            public override string ToString()
            {
                return $"Data scan: action - {ScannerAction}, barcode - {Barcode}, ScannerID - {Scanner.ScannerID}, ScannerSN - {Scanner.Serialnumber}, Symbology - {Symbology}, Date - {CreateDateTime}";
            }

        }
}
