
namespace iHolography
{
    namespace ScannerService
    {
        public class DataScan
        {
            public string Barcode { get; private set; }
            public string Symbology { get; private set; }
            public int ScannerAction { get; private set; }
            public Scanner Scanner { get; private set; }
            public static string DateTimeFormat { get; set; }
            public string CreateDateTime { get; private set; }
            public DataScan(string barcode, string symbology, Scanner scanner, int scannerAction = -1)
            {
                DateTimeFormat = "G";
                Barcode = barcode;
                Symbology = symbology;
                ScannerAction = scannerAction;
                Scanner = (Scanner)scanner.Clone();
                CreateDateTime = Variable.GetStringDateTime(DateTimeFormat);
            }
            public override string ToString()
            {
                return $"Data scan: action - {ScannerAction}, barcode - {Barcode}" +
                       $", ScannerID - {Scanner.ScannerID}, ScannerSN - {Scanner.Serialnumber}, Symbology - {Symbology}, Date - {CreateDateTime}";
            }

        }
    }
}
