using AppDefs;
using ScannerService;

namespace IHolographyH1
{
    interface IScanListener
    {
        void StartScannListener();
        void StoptScannListener();
        void OpenConnection();
        void CloseConnection();
        void ResetAlm();
        void SetSpecificAttribute(Scanner scanner, int attributeCode);
        void SetShortTermSpecificAttribute(Scanner scanner, int attributeCode, int milisecond);
        void GetScanEvent(DataScan dataScan);
        //Подписки на событие
        void Scan(DataScan dataScan);
        void CheckCreatedScannerLister(int status);
    }
}
