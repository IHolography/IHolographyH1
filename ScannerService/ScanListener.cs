using System;
using CoreScanner;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Logger;

namespace ScannerService
{
    public class ScanListener
    {
        // Scan event
        public delegate void Scan(DataScan dataScan);
        public event Scan ScanEvent;
        // Create object event
        public delegate void ScanListenerException(int status, string messge);
        public static event ScanListenerException ScanListenerEx;
        // Other diagnostic event
        #region DiagnosticEvents
        //public delegate void ScannersHandler(string message);
        //public event ScannersHandler Log_Notify;
        //public event ScannersHandler CommandExecuteResult_Notify;
        #endregion

        public static CCoreScanner CoreScannerObject { get; private set; }
        public DataScan ScanEventInfo { get; private set; }
        //public static int ScannerAction { get; set; }
        public static int ScannerMode { get; set; }
        public List<Scanner> ListConnectedScanners { get; private set; }

        public ScanListener(CCoreScanner coreScannerObject)
        {
            //ScannerAction = (int)ScanAction.Undefined;
            ScannerMode = (int)Mode.Work;
            CoreScannerObject = coreScannerObject;
            Status status = GetConnectedScanners();
            if (status == Status.Success)
            {
                SubscribeForBarcodeEvents();
                Log.Write("Object ScanListener created", this);
            }
            else
            {
                Log.Write("Object ScanListener don't create. Check connected scanners.", this);
            }
            Check(status);
        }
        private void Check(Status status)
        {
          if (status==Status.Failed) ScanListenerEx?.Invoke((int)Status.Failed, "Object ScanListener is not created. Check initialize static fields or connected scanners.");
        }
        private Status GetConnectedScanners()
        {
            Status status = Status.Success;
            string outXML = String.Empty;
            List<Scanner> scanners = new List<Scanner>();
            if (GetConnectedScanners(out string xml) == (int)Status.Success)
            {
                XMLReader.ParseXML(xml, ref scanners);
                ListConnectedScanners = new List<Scanner>(scanners);
            }
            else
            {
                Log.Write("Can't get scanners online. Check connected scanners");
                ScanListenerEx?.Invoke((int)Status.Failed, "Can't get scanners online. Check connected scanners");
                status = Status.Failed;
            }
            return status;
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
                    status = (int)Status.Success;
                }
                else
                {
                    status = (int)Status.Failed;
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
            int status = (int)Status.Failed;

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
                throw new Exception($"ScanListener can't subscribe for barcode events. Xml file for command: {inXml}");
            }
            return status;
        }
        public int UnSubscribeForBarcodeEvents()
        {
            int res = (int)Status.Failed;
            try
            {
                CoreScannerObject.BarcodeEvent -= ActionOnBarcodeEvent;
                res = (int)Status.Success;
            }
            catch 
            { 
            }
            return res;
        }
        private void GetDecodeBarcode(string strXml)
        {
            try 
            {
                XMLReader.ParseXML(strXml);
                GetDataScan(XMLReader.Barcode, XMLReader.Symbology, XMLReader.ScanerID);
            }
            catch (Exception ex)
            {
                if (XMLReader.ScanerID != String.Empty)
                {
                    Log.Write($"Scanner ID-{XMLReader.ScanerID} couldn't scan barcode", this);
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
                ScanListenerEx?.Invoke((int)Status.Success, "Scanners GetDecodeBarcode() - Failed. " + ex.Message);
            }
        }
        private void ActionOnBarcodeEvent(short eventType, ref string pscanData)
        {
                GetDecodeBarcode(pscanData);
        }
        private Scanner GetScannerById(string scannerID)
        {
            foreach (Scanner scanner in ListConnectedScanners)
            {
                if (scanner.ScannerID == scannerID)
                {
                    return scanner;
                }
            }
            return ListConnectedScanners.FirstOrDefault();
        }
        private void GetDataScan(string barcode, string symbology, string scannerID)
        {
            //if (ScannerMode==(int)Mode.Work)
            //{
            //    if (ScannerAction != (int)ScanAction.Undefined)
            //    {
                    GetData(barcode, symbology, scannerID);
            //    }
            //    else
            //    {
            //        Log.Write("Scanner action undefined. ScanData not available on this scan.", this);
            //        Exception(GetScannerById(scannerID));
            //        ScanListenerEx?.Invoke((int)Status.Success, $"Scanner action undefined. ScanData not available on this scan. {this}");
            //    }
            //}
            //else
            //{
            //    GetData(barcode, symbology, scannerID);
            //}
           
        }
        private void GetData(string barcode, string symbology, string scannerID)
        {
            Scanner scanner = GetScannerById(scannerID);
            ScanEventInfo = new DataScan(barcode, symbology, scanner); //ScanEventInfo = new DataScan(barcode, symbology, scanner, ScannerAction);
            Log.Write(ScanEventInfo.ToString(), this);
            if (scanner.ScannerException == Alm.Ok)
            {
                try
                {
                    ScanEvent?.Invoke(ScanEventInfo);
                }
                catch
                {
                    Log.Write("GetDataScan(): ScanEvent in other thread");
                    Exception(scanner);
                    ScanListenerEx?.Invoke((int)Status.Failed, $"ScannerService.ScanListener.GetDataScan(): ScanEvent can't invoke. Sender:{this}");
                }
            }
            else
            {
                Log.Write($"In scanner Id-{scanner.ScannerID}, SN-{scanner.Serialnumber} active alarm. Try reset it.");
                Exception(scanner);
                ScanListenerEx?.Invoke((int)Status.Success, $"In scanner Id-{scanner.ScannerID}, SN-{scanner.Serialnumber} active alarm. Try reset it.");
            }
        }
        
        private void Exception(Scanner scanner)
        {
            SetAlarmAttributeOnScanner(scanner);
            ResetAlm(scanner);
        }
        public void ResetAlm()
        {
            foreach(Scanner scanner in ListConnectedScanners)
            {
                if (scanner.ScannerException==Alm.Alarm)
                {
                    OffRedLed(scanner);
                    scanner.ResetException();
                }
            }
        }
        public void ResetAlm(Scanner scanner)
        {
                    OffRedLed(scanner);
                    scanner.ResetException();
        }
        private void SetAlarmAttributeOnScanner(Scanner scanner)
        {
            SetShortTermSpecificAttribute(scanner, (int)LEDCode.Led3On, 500);
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
            SetSpecificAttribute(scanner, (int)BeepCode.ThreeShortLow);
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
