using System;
using CoreScanner;
using System.Xml;
using AppDefs;
using System.Collections.Generic;

namespace IHolographyH1
{
    class ScanListener
    {
        // Scan event
        public delegate void Scanner(DataScan dataScan);
        public event Scanner ScanEvent;
        // Other diagnostic event
        #region DiagnosticEvents
        //public delegate void ScannersHandler(string message);
        //public event ScannersHandler Log_Notify;
        //public event ScannersHandler CommandExecuteResult_Notify;
        #endregion

        public static CCoreScanner CoreScannerObject { get; private set; }
        public DataScan ScanEventInfo { get; private set; }
        public static ScannerAction ScannerAction { get; set; }
        public static short ConnectedScannersCount { get; private set; }
        private static List<string> ListIdScannersWithException { get; set; }

        public ScanListener(CCoreScanner coreScannerObject)
        {
            ListIdScannersWithException = new List<string>();
            CoreScannerObject = coreScannerObject;
            GetConnectedScanners();
            RegisterForEvent();
            Logger.Write("Object created",this);
        }
        private void GetConnectedScanners()
        {
            if (GetConnectedScanners(out short numOfScanners) == (int)Status.Success)
            {
                ConnectedScannersCount = numOfScanners;
            }
            else
            {
                ConnectedScannersCount = 0;
            }
        }
        private static int GetConnectedScanners(out short numOfScanners)
        {

            numOfScanners = 0;
            string xml = "";
            int status = (int)Status.Failed;
            int[] scannerIdList = new int[Constant.MaxNumDevices];

            if (COM.Status == (int)Status.Success)
            {
                // Get connected scanners
                CoreScannerObject.GetScanners(out numOfScanners, // Returns number of scanners discovered 
                                              scannerIdList,     // Returns array of connected scanner ids 
                                              out xml,        // Output xml containing discovered scanners information 
                                              out status);       // Command execution success/failure return status 

                if (status == (int)Status.Success && numOfScanners > 0)
                {
                    status = (int)AppDefs.Status.Success;
                }
            }
            return status;
        }
        private int RegisterForEvent()
        {
            // Subscribe for barcode events in cCoreScannerClass
            CoreScannerObject.BarcodeEvent += new
            _ICoreScannerEvents_BarcodeEventEventHandler(OnBarcodeEvent);

            int eventIdCount = 1; // Number of events to register (only barcode events)
            int[] eventIdList = new int[eventIdCount];
            // Register for barcode events only
            eventIdList[0] = (int)EventType.Barcode;

            string eventIds = String.Join(",", eventIdList);
            string inXml = "<inArgs>" +
                           "<cmdArgs>" +
                           "<arg-int>" + eventIdCount + "</arg-int>" +   // Number of events to register
                           "<arg-int>" + eventIds + "</arg-int>" +       // Event id list of events to register for
                           "</cmdArgs>" +
                           "</inArgs>";

            int opCode = (int)Opcode.RegisterForEvents;
            string outXml = "";
            int status = (int)AppDefs.Status.Failed;

            // Call register for events
            COM.CoreScannerObject.ExecCommand(opCode,  // Opcode: Register for events
                                          ref inXml,   // Input XML
                                          out outXml,  // Output XML 
                                          out status); // Command execution success/failure return status  

            if (status == (int)Status.Success)
            {
                //Log_Notify?.Invoke(DateTime.Now + "   Scanners RegisterForEvents() - Success.");
            }
            else
            {
                //Log_Notify?.Invoke(DateTime.Now + "   Scanners RegisterForEvents() - Failed. Error Code : " + status);
            }
            return status;
        }
        private void GetDecodeBarcode(string strXml)
        {
            try 
            { 
                XMLReader.ParseXML(strXml);
                GetDataScan(XMLReader.Barcode, XMLReader.ScannerSerialNumber, XMLReader.Symbology, XMLReader.ScanerID);
            }
            catch
            {
                if (XMLReader.ScanerID == String.Empty)
                {
                    Logger.Write($"Scanner ID-{XMLReader.ScanerID} couldn't scan barcode", this);
                    Exception(XMLReader.ScanerID);
                }
                else
                {
                        for (short i = 1; i <= ConnectedScannersCount; i++)
                        {
                            Exception(i.ToString());
                        }
                }
                //Log_Notify?.Invoke(DateTime.Now + "   Scanners GetDecodeBarcode() - Failed. " + ex.Message);
            }
        }
        
        private void OnBarcodeEvent(short eventType, ref string pscanData)
        {
                GetDecodeBarcode(pscanData);
        }
        private void GetDataScan(string barcode, string scannerSN, string symbology, string scannerID)
        {
            if (ScannerAction == ScannerAction.Undefined)
            {
                Logger.Write("Scanner action undefined. ScanData not available on this scan.", this);
                Exception(scannerID);
                //throw new Exception();
            }
            else
            {
                ScanEventInfo = new DataScan(barcode, scannerSN, symbology, scannerID, ScannerAction);
                Logger.Write(ScanEventInfo.ToString(), this);
                ScanEvent?.Invoke(ScanEventInfo);
            }
        }
        private void Exception(string scannerID)
        {
            SetAlm(scannerID);
            ListIdScannersWithException.Add(scannerID);
        }
        public void ResetAlm()
        {
            if (ListIdScannersWithException != null && ListIdScannersWithException.Count!=0)
            {
                foreach (string i in ListIdScannersWithException)
                {
                    OffRedLed(i);
                }
                ListIdScannersWithException.Clear();
            }
            else
            {
                for (short i = 1; i <= ConnectedScannersCount; i++)
                {
                    OffRedLed(i.ToString());
                }
            }
        }
        private void SetAlm(string scannerID)
        {
            OnRedLed(scannerID);
            OnSpecificBeep(scannerID);
        }
        private void OnRedLed(string scannerID)
        {
            SetSpecificAttribute(scannerID, (int)LEDCode.Led3On);
        }
        private void OffRedLed(string scannerID)
        {
            SetSpecificAttribute(scannerID, (int)LEDCode.Led3Off);
        }
        private void OnSpecificBeep(string scannerID)
        {
            SetSpecificAttribute(scannerID, (int)BeepCode.ThreeLongLow);
        }
        public void SetSpecificAttribute(string scannerID,int attributeCode)
        {
            int status = (int)Status.Failed;
            string outXml = String.Empty;
            int opCode = (int)Opcode.SetAction;
            string inXml = "<inArgs>" +
                                          "<scannerID>" + scannerID.ToString() + "</scannerID>" +
                                          "<cmdArgs>" +
                                          "<arg-int>" + attributeCode +  // Specify beeper code
                                          "</arg-int>" +
                                          "</cmdArgs>" +
                                          "</inArgs>";

            CoreScannerObject.ExecCommand(opCode, // Opcode: for Scanner LED on (SET_ACTION)
               ref inXml, // Input XML
               out outXml, // Output XML 
               out status); // Command execution success/failure return status
        }
    }
}
