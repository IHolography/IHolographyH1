using System;
using CoreScanner;
using System.Xml;
using AppDefs;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace IHolographyH1
{
    class ScanListener
    {
        // Scan event
        public delegate void Scan(DataScan dataScan);
        public event Scan ScanEvent;
        // Other diagnostic event
        #region DiagnosticEvents
        //public delegate void ScannersHandler(string message);
        //public event ScannersHandler Log_Notify;
        //public event ScannersHandler CommandExecuteResult_Notify;
        #endregion

        public static CCoreScanner CoreScannerObject { get; private set; }
        public DataScan ScanEventInfo { get; private set; }
        public static ScannerAction ScannerAction { get; set; }
        public List<Scanner> ListConnectedScanners { get; private set; }

        public ScanListener(CCoreScanner coreScannerObject)
        {
            CoreScannerObject = coreScannerObject;
            GetConnectedScanners();
            SubscribeForBarcodeEvents();
            Logger.Write("Object created",this);
        }

        private void GetConnectedScanners()
        {
            string outXML = String.Empty;
            if (GetConnectedScanners(out string xml) == (int)Status.Success)
            {
                List<Scanner> scanners = new List<Scanner>();

                if(XMLReader.ParseXML(xml, ref scanners)==(int)Status.Success)
                {
                    ListConnectedScanners = new List<Scanner>(scanners);
                }
                else
                {
                    ListConnectedScanners = new List<Scanner>();
                }
            }
            else
            {
                ListConnectedScanners = new List<Scanner>();
            }
        }
        private int GetConnectedScanners(out string outXml)
        {
            short numOfScanners = 0;
            outXml = String.Empty;
            string xml = String.Empty;
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
                    outXml = xml;
                    status = (int)AppDefs.Status.Success;
                }
            }
            return status;
        }
        private int SubscribeForBarcodeEvents()
        {
            CoreScannerObject.BarcodeEvent += new
            _ICoreScannerEvents_BarcodeEventEventHandler(ActionOnBarcodeEvent);

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
                if (XMLReader.ScanerID != String.Empty)
                {
                    Logger.Write($"Scanner ID-{XMLReader.ScanerID} couldn't scan barcode", this);
                    Exception(GetScannerById(XMLReader.ScanerID));
                }
                else
                {
                    foreach(Scanner scanner in ListConnectedScanners)
                    {
                        Exception(scanner);
                    }
                }
                //Log_Notify?.Invoke(DateTime.Now + "   Scanners GetDecodeBarcode() - Failed. " + ex.Message);
            }
        }
        private void ActionOnBarcodeEvent(short eventType, ref string pscanData)
        {
                GetDecodeBarcode(pscanData);
        }
        private Scanner GetScannerById(string scannerID)
        {
            var scanner = ListConnectedScanners.Where(scan => scan.ScannerID.Equals(scannerID));
            return (Scanner)scanner;
        }
        private void GetDataScan(string barcode, string scannerSN, string symbology, string scannerID)
        {
            if (ScannerAction != ScannerAction.Undefined)
            {
                ScanEventInfo = new DataScan(barcode, symbology, ScannerAction, GetScannerById(scannerID));
                Logger.Write(ScanEventInfo.ToString(), this);
                ScanEvent?.Invoke(ScanEventInfo);
            }
            else
            {
                Logger.Write("Scanner action undefined. ScanData not available on this scan.", this);
                Exception(GetScannerById(scannerID));
                //throw new Exception();
            }
        }
        private void Exception(Scanner scanner)
        {
            SetAlarmAttributeOnScanner(scanner);
        }
        public void ResetAlm()
        {
            foreach(Scanner scanner1 in ListConnectedScanners)
            {
                if (scanner1.ScannerException==Alm.Alarm)
                {
                    OffRedLed(scanner1);
                    scanner1.ResetException();
                }
            }
        }
        private void SetAlarmAttributeOnScanner(Scanner scanner)
        {
            OnRedLed(scanner);
            OnSpecificBeep(scanner);
            scanner.SetException();
        }
        private void OnRedLed(Scanner scanner)
        {
            SetSpecificAttribute(scanner, (int)LEDCode.Led3On);
        }
        private void OffRedLed(Scanner scanner)
        {
            SetSpecificAttribute(scanner, (int)LEDCode.Led3Off);
        }
        private void OnSpecificBeep(Scanner scanner)
        {
            SetSpecificAttribute(scanner, (int)BeepCode.ThreeLongLow);
        }
        public void SetSpecificAttribute(Scanner scanner, int attributeCode)
        {
            int status = (int)Status.Failed;
            string outXml = String.Empty;
            int opCode = (int)Opcode.SetAction;
            string inXml = "<inArgs>" +
                                          "<scannerID>" + scanner.ScannerID + "</scannerID>" +
                                          "<cmdArgs>" +
                                          "<arg-int>" + attributeCode + 
                                          "</arg-int>" +
                                          "</cmdArgs>" +
                                          "</inArgs>";

            CoreScannerObject.ExecCommand(opCode,
               ref inXml, 
               out outXml, 
               out status); 
        }
        public void SetShortTermSpecificAttribute(Scanner scanner, int attributeCode, int milisecond)
        {
            SetSpecificAttribute(scanner, attributeCode);
            Thread.Sleep(milisecond);
            OffRedLed(scanner);
        }
    }
}
