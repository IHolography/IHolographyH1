using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using AppDefs;
using Logger;
using ScannerService;

namespace IHolographyH1
{
    public class StartScanListen 
    {
        public ScanListener ScanListenerObject { get; private set; }
        public static ScannerAction ScanProductOrBoxProperties { get; private set; }
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
                ScanProductOrBoxProperties = ScannerAction.Undefined;
                SubscribeCheckCreatedScannerListerEvent();
                COM.OpenConnection();
                ScanListener scanListener = new ScanListener(COM.CoreScannerObject);
                ScanListenerObject = scanListener;
                SubscribeScanEvent();
            }
            catch(Exception ex)
            {
                StopScannListener(ex.ToString());
            }
        }
        public void StopScannListener()
        {
            ScanListenerObject = null;
            COM.CloseConnection();
            Log.Write("Oblect ScnListener is deleted, COM object is closed",this);
            try
            {
                threadDictionary["BackgroundScannersThread"].Abort();
            }
            catch 
            { }
            
        }
        private void StopScannListener(string message)
        {
            throw new Exception($"{this} object deleted. Reason: {message}");
        }
        public void SetScanProductOrBoxProperties(ScannerAction scanAction)
        {
            ScanProductOrBoxProperties = scanAction;
            ScanListener.ScannerAction = (int)ScanProductOrBoxProperties;
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
        void SubscribeScanEvent()
        {
            ScanListenerObject.ScanEvent += ScanEvent;
        }
        void SubscribeCheckCreatedScannerListerEvent()
        {
            ScanListener.ScanListenerEx += CheckCreatedScannerListerEvent;
        }
        ////Events
        void ScanEvent(DataScan dataScan)
        {
            MessageBox.Show(dataScan.ToString());
        }
        void CheckCreatedScannerListerEvent(int status,string message)
        {
            if (status == (int)AppDefs.Status.Failed)
            {
                StopScannListener();
            }
            else
            {
                MessageBox.Show(message);
            }
        }
    }
}
