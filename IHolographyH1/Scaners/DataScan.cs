using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDefs;

namespace IHolographyH1.Scaners
{
        public class DataScan
        {
            const string formatDateTime = "dd.MM.yyyy HH:mm:ss";
            public string Barcode { get; set; }
            public string ScannerSN { get; set; }
            public string Symbology { get; set; }
            public string CreateDateTime { get; private set; }
            public DataScan(string barcode, string scannerSN, string symbology)
            {
                Barcode = barcode;
                ScannerSN = scannerSN;
                Symbology = symbology;
            CreateDateTime = Variable.GetStringDateTime();//DateTime.Now.ToString(formatDateTime);
            }
            public override string ToString()
            {
                return $"Data scan: barcode - {Barcode}, ScannerSN - {ScannerSN}, Symbology - {Symbology}, Date - {CreateDateTime}";
            }

        }
}
