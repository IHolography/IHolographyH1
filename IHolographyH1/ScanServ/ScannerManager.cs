using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using AppDefs;
using iHolography.Logger;
using iHolography.ScannerService;

namespace IHolographyH1
{
    public class ScannerManager:IScanListener
    {
        //Subscribe to Failed event
        public delegate void FailedApp(string mess);
        public static event FailedApp FailedAppEvent;

        public ScanListener ScanListenerObject { get; private set; }
        ScanListener scanListener;
        public static ScannerAction ScanProductOrBoxProperties { get; private set; }
        public static AppDefs.Mode ScannerMode { get; private set; }
        Dictionary<string, Thread> threadDictionary = new Dictionary<string, Thread>();

        public void StartScannListener()
        {
            Log.LogFilePath=AppDefs.Constant.LogFilePath;
            Log.LogEnable= AppDefs.Constant.LogEnable;
            Log.DateTimeFormat=DataScan.DateTimeFormat= AppDefs.Variable.DateTimeFormat;

            Dictionary<string, Thread> threadDictionary = new Dictionary<string, Thread>();
            Thread thread = new Thread(new ThreadStart(Start));
            threadDictionary.Add("BackgroundScannersThread", thread);
            thread.IsBackground = true;
            thread.Start();
            Thread.Sleep(200);
        }
        private void Start()
        {
            try
            {
                //ScanProductOrBoxProperties = ScannerAction.Undefined;
                SubscribeCheckCreatedScannerListerEvent();
                COM.OpenConnection();
                if (scanListener == null)
                {
                    scanListener = new ScanListener(COM.CoreScannerObject);
                }
                ScanListenerObject = scanListener;
                if (ScanListenerObject.ListConnectedScanners != null)
                {
                    foreach (Scanner scanner in ScanListenerObject.ListConnectedScanners)
                    {
                        SetSpecificAttribute(scanner, (int)AppDefs.LEDCode.Led3Off);
                        SetSpecificAttribute(scanner, (int)AppDefs.BeepCode.OneLongLow);
                    }
                    SubscribeScanEvent();
                }
                else
                {
                    StopScannListener();
                }
            }
            catch
            {
                StopScannListener();
            }
        }
        public void StopScannListener()
        {
                if (scanListener!=null)
                {
                    scanListener.UnSubscribeForBarcodeEvents();
                    if (ScanListenerObject.ListConnectedScanners != null)
                    {
                        foreach (Scanner scanner in ScanListenerObject.ListConnectedScanners)
                        {
                            SetSpecificAttribute(scanner, (int)AppDefs.LEDCode.Led3On);
                            SetSpecificAttribute(scanner, (int)AppDefs.BeepCode.OneLongLow);
                        }
                        SubscribeScanEvent();
                    }
                    else
                    {
                        FailedAppEvent?.Invoke("AppFailed");
                    }
                    ScanListenerObject = scanListener = null;
                    COM.CloseConnection();
                    Log.Write("Oblect ScnListener is deleted, COM object is closed", this);
                    try
                    {
                        threadDictionary["BackgroundScannersThread"].Abort();
                        FailedAppEvent?.Invoke("AppFailed");
                    }
                    catch
                    { }
                }
                else
                {
                    FailedAppEvent?.Invoke("AppFailed");
                }
        }
        public void SetScanProductOrBoxProperties(ScannerAction scanAction)
        {
            //ScanProductOrBoxProperties = scanAction;
            //ScanListener.ScannerAction = (int)ScanProductOrBoxProperties;
        }
        public void SetScannerMode(AppDefs.Mode mode)
        {
            ScannerMode = mode;
            ScanListener.ScannerMode = (int)ScannerMode;
        }
        public void ResetAlm()
        {
            ScanListenerObject.ResetAlm();
        }
        public void SetSpecificAttribute(Scanner scanner, int attributeCode)
        {
            ScanListenerObject.SetSpecificAttribute(scanner, attributeCode);
        }
        public void SetShortTermSpecificAttribute(Scanner scanner, int attributeCode, int milisecond)
        {
            ScanListenerObject.SetShortTermSpecificAttribute(scanner, attributeCode, milisecond);
        }
        //Subscribe
        public void SubscribeScanEvent()
        {
            ScanListenerObject.ScanEvent += ScanEvent;
        }
        public void SubscribeCheckCreatedScannerListerEvent()
        {
            ScanListener.ScanListenerEx += CheckCreatedScannerListerEvent;
        }
        ////Events
        public void ScanEvent(DataScan dataScan)
        {
            Analyzer.Analize(dataScan);
        }
        public void CheckCreatedScannerListerEvent(int status,string message)
        {
            if (status == (int)AppDefs.Status.Failed) StopScannListener();
        }
    }
}
