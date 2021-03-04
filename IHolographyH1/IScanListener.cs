using AppDefs;
using CoreScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IHolographyH1
{
    interface IScanListener
    {
        ScanListener ScanListener { get; set; }
        CCoreScanner CoreScannerObject { get; set; }
        DataScan ScanEventInfo { get; set; }
        ScannerAction ScannerAction { get; set; }
        List<Scanner> ListConnectedScanners { get; set; }

        void OpenConnection();
        void CloseConnection();
        void ResetAlm();
        void SetSpecificAttribute(Scanner scanner, int attributeCode);
        void SetShortTermSpecificAttribute(Scanner scanner, int attributeCode, int milisecond);
        void GetScanEvent(DataScan dataScan);
        //Подписка на событие
        void Scan(DataScan dataScan);
    }
}
