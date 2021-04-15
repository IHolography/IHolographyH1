using AppDefs;
using iHolography.ScannerService;

namespace IHolographyH1
{
    public interface IScanListener
    {
        void StartScannListener();
        void StopScannListener();
        void SetScanProductOrBoxProperties(ScannerAction scanAction);
        void SetScannerMode(AppDefs.Mode mode);
        void ResetAlm();
        void SetSpecificAttribute(Scanner scanner, int attributeCode);
        void SetShortTermSpecificAttribute(Scanner scanner, int attributeCode, int milisecond);
        void SubscribeScanEvent();
        void SubscribeCheckCreatedScannerListerEvent();
        //Event TODO
        void ScanEvent(DataScan dataScan);
        void CheckCreatedScannerListerEvent(int status, string message);
    }
}
