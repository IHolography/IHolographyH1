using System;
using CoreScanner;
using CoreScannerLib;
using System.Xml;

namespace IHolographyH1.Scaners
{
    class Scanners
    {
        // Scan event
        public delegate void Scan(DataScan dataScan);
        public event Scan ScanAction;
        // Other diagnostic event
        #region DiagnosticEvents
        public delegate void ScannersHandler(string message);
        public event ScannersHandler Log_Notify;
        public event ScannersHandler CommandExecuteResult_Notify;
        #endregion
        public Scanners()
        {
        }
        public int GetConnectedScanners(out string outXml)
        {
            short numOfScanners = 0;
            string xml = outXml = "";
            int[] scannerIdList = new int[Constant.MaxNumDevices];
            int status = -1;

            if (COM.Status != -1)
            {
                // Get connected scanners
                COM.CoreScannerObject.GetScanners(out numOfScanners, // Returns number of scanners discovered 
                                              scannerIdList,     // Returns array of connected scanner ids 
                                              out xml,        // Output xml containing discovered scanners information 
                                              out status);       // Command execution success/failure return status 

                if (status == (int)Status.Success)
                {
                    outXml = xml;
                    CommandExecuteResult_Notify?.Invoke("Scanners GetConnectedScanners() - Success. Out xml : " + Environment.NewLine + xml);
                }
            }
            return status;
        }
        public int RegisterForEvent()
        {
            // Subscribe for barcode events in cCoreScannerClass
            COM.CoreScannerObject.BarcodeEvent += new
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
            int status = -1;

            // Call register for events
            COM.CoreScannerObject.ExecCommand(opCode,      // Opcode: Register for events
                                          ref inXml,   // Input XML
                                          out outXml,  // Output XML 
                                          out status); // Command execution success/failure return status  

            if (status == (int)Status.Success)
            {
                Log_Notify?.Invoke(DateTime.Now + "   Scanners RegisterForEvents() - Success.");
            }
            else
            {
                Log_Notify?.Invoke(DateTime.Now + "   Scanners RegisterForEvents() - Failed. Error Code : " + status);
            }
            return status;
        }
        private void GetDecodeBarcode(string strXml)
        {
            System.Diagnostics.Debug.WriteLine("Initial XML" + strXml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(strXml);

            string strData = String.Empty;
            string symbologyData = String.Empty;
            string scannerSerialNumber = String.Empty;
            try
            {
                string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
                string symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
                scannerSerialNumber = xmlDoc.DocumentElement.GetElementsByTagName("serialnumber").Item(0).InnerText;
                string[] numbers = barcode.Split(' ');

                foreach (string number in numbers)
                {
                    if (String.IsNullOrEmpty(number))
                    {
                        break;
                    }
                    strData += ((char)Convert.ToInt32(number, 16)).ToString();
                }
                symbologyData = GetSymbology((int)Convert.ToInt32(symbology));

                #region Notify(string str)
                Log_Notify?.Invoke(DateTime.Now + "   Scanners GetDecodeBarcode() - Success.");
                CommandExecuteResult_Notify?.Invoke("   Scanners OnBarcodeEvent(): decodeBarcode= " + strData +
                                                                               "; symbology= " + symbologyData +
                                                                               "; scanner S/N: " + scannerSerialNumber);
                #endregion
                GetDataScan(strData, scannerSerialNumber, symbologyData);
            }
            catch (Exception ex)
            {
                Log_Notify?.Invoke(DateTime.Now + "   Scanners GetDecodeBarcode() - Failed. " + ex.Message);
            }
        }
        ///Barcode symbology
        /// </summary>
        /// <param name="Code">Symbology code</param>
        /// <returns>Symbology name</returns>
        private string GetSymbology(int Code)
        {
            switch (Code)
            {
                case (int)CoreScannerLib.CodeType.Unknown:
                    return "NOT APPLICABLE";
                case (int)CoreScannerLib.CodeType.Code39:
                    return "CODE 39";
                case (int)CoreScannerLib.CodeType.Codabar:
                    return "CODABAR";
                case (int)CoreScannerLib.CodeType.Code128:
                    return "CODE 128";
                case (int)CoreScannerLib.CodeType.Discrete2of5:
                    return "DISCRETE 2 OF 5";
                case (int)CoreScannerLib.CodeType.Iata:
                    return "IATA";
                case (int)CoreScannerLib.CodeType.Interleaved2of5:
                    return "INTERLEAVED 2 OF 5";
                case (int)CoreScannerLib.CodeType.Code93:
                    return "CODE 93";
                case (int)CoreScannerLib.CodeType.UpcA:
                    return "UPC-A";
                case (int)CoreScannerLib.CodeType.UpcE0:
                    return "UPC-E0";
                case (int)CoreScannerLib.CodeType.Ean8:
                    return "EAN-8";
                case (int)CoreScannerLib.CodeType.Ean13:
                    return "EAN-13";
                case (int)CoreScannerLib.CodeType.Code11:
                    return "CODE 11";
                case (int)CoreScannerLib.CodeType.Code49:
                    return "CODE 49";
                case (int)CoreScannerLib.CodeType.Msi:
                    return "MSI";
                case (int)CoreScannerLib.CodeType.Ean128:
                    return "EAN-128";
                case (int)CoreScannerLib.CodeType.UpcE1:
                    return "UPC-E1";
                case (int)CoreScannerLib.CodeType.Pdf417:
                    return "PDF-417";
                case (int)CoreScannerLib.CodeType.Code16k:
                    return "CODE 16K";
                case (int)CoreScannerLib.CodeType.Code39FullAscii:
                    return "CODE 39 FULL ASCII";
                case (int)CoreScannerLib.CodeType.UpcD:
                    return "UPC-D";
                case (int)CoreScannerLib.CodeType.Code39Trioptic:
                    return "CODE 39 TRIOPTIC";
                case (int)CoreScannerLib.CodeType.Bookland:
                    return "BOOKLAND";
                case (int)CoreScannerLib.CodeType.UpcaWCode128:
                    return "UPC-A w/Code 128 Supplemental";
                case (int)CoreScannerLib.CodeType.Jan13WCode128:
                    return "EAN/JAN-13 w/Code 128 Supplemental";
                case (int)CoreScannerLib.CodeType.Nw7:
                    return "NW-7";
                case (int)CoreScannerLib.CodeType.Isbt128:
                    return "ISBT-128";
                case (int)CoreScannerLib.CodeType.MicroPdf:
                    return "MICRO PDF";
                case (int)CoreScannerLib.CodeType.Datamatrix:
                    return "DATAMATRIX";
                case (int)CoreScannerLib.CodeType.Qrcode:
                    return "QR CODE";
                case (int)CoreScannerLib.CodeType.MicroPdfCca:
                    return "MICRO PDF CCA";
                case (int)CoreScannerLib.CodeType.PostnetUs:
                    return "POSTNET US";
                case (int)CoreScannerLib.CodeType.PlanetCode:
                    return "PLANET CODE";
                case (int)CoreScannerLib.CodeType.Code32:
                    return "CODE 32";
                case (int)CoreScannerLib.CodeType.Isbt128Con:
                    return "ISBT-128 CON";
                case (int)CoreScannerLib.CodeType.JapanPostal:
                    return "JAPAN POSTAL";
                case (int)CoreScannerLib.CodeType.AusPostal:
                    return "AUS POSTAL";
                case (int)CoreScannerLib.CodeType.DutchPostal:
                    return "DUTCH POSTAL";
                case (int)CoreScannerLib.CodeType.Maxicode:
                    return "MAXICODE";
                case (int)CoreScannerLib.CodeType.CanadinPostal:
                    return "CANADIAN POSTAL";
                case (int)CoreScannerLib.CodeType.UkPostal:
                    return "UK POSTAL";
                case (int)CoreScannerLib.CodeType.MacroPdf:
                    return "MACRO PDF";
                case (int)CoreScannerLib.CodeType.MacroQrCode:
                    return "MACRO QR CODE";
                case (int)CoreScannerLib.CodeType.MicroQrCode:
                    return "MICRO QR CODE";
                case (int)CoreScannerLib.CodeType.Aztec:
                    return "AZTEC";
                case (int)CoreScannerLib.CodeType.AztecRune:
                    return "AZTEC RUNE";
                case (int)CoreScannerLib.CodeType.Distance:
                    return "DISTANCE";
                case (int)CoreScannerLib.CodeType.Rss14:
                    return "GS1 DATABAR";
                case (int)CoreScannerLib.CodeType.RssLimited:
                    return "GS1 DATABAR LIMITED";
                case (int)CoreScannerLib.CodeType.RssExpanded:
                    return "GS1 DATABAR EXPANDED";
                case (int)CoreScannerLib.CodeType.Parameter:
                    return "PARAMETER";
                case (int)CoreScannerLib.CodeType.Usps4c:
                    return "USPS 4CB";
                case (int)CoreScannerLib.CodeType.UpuFicsPostal:
                    return "UPU FICS POSTAL";
                case (int)CoreScannerLib.CodeType.Issn:
                    return "ISSN";
                case (int)CoreScannerLib.CodeType.Scanlet:
                    return "SCANLET";
                case (int)CoreScannerLib.CodeType.Cuecode:
                    return "CUECODE";
                case (int)CoreScannerLib.CodeType.Matrix2of5:
                    return "MATRIX 2 OF 5";
                case (int)CoreScannerLib.CodeType.Upca_2:
                    return "UPC-A + 2 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Upce0_2:
                    return "UPC-E0 + 2 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Ean8_2:
                    return "EAN-8 + 2 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Ean13_2:
                    return "EAN-13 + 2 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Upce1_2:
                    return "UPC-E1 + 2 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.CcaEan128:
                    return "CCA EAN-128";
                case (int)CoreScannerLib.CodeType.CcaEan13:
                    return "CCA EAN-13";
                case (int)CoreScannerLib.CodeType.CcaEan8:
                    return "CCA EAN-8";
                case (int)CoreScannerLib.CodeType.CcaRssExpanded:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCA)";
                case (int)CoreScannerLib.CodeType.CcaRssLimited:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCA)";
                case (int)CoreScannerLib.CodeType.CcaRss14:
                    return "GS1 DATABAR COMPOSITE (CCA)";
                case (int)CoreScannerLib.CodeType.CcaUpca:
                    return "CCA UPC-A";
                case (int)CoreScannerLib.CodeType.CcaUpce:
                    return "CCA UPC-E";
                case (int)CoreScannerLib.CodeType.CccEan128:
                    return "CCC EAN-128";
                case (int)CoreScannerLib.CodeType.Tlc39:
                    return "TLC-39";
                case (int)CoreScannerLib.CodeType.CcbEan128:
                    return "CCB EAN-128";
                case (int)CoreScannerLib.CodeType.CcbEan13:
                    return "CCB EAN-13";
                case (int)CoreScannerLib.CodeType.CcbEan8:
                    return "CCB EAN-8";
                case (int)CoreScannerLib.CodeType.CcbRssExpanded:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCB)";
                case (int)CoreScannerLib.CodeType.CcbRssLimited:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCB)";
                case (int)CoreScannerLib.CodeType.CcbRss14:
                    return "GS1 DATABAR COMPOSITE (CCB)";
                case (int)CoreScannerLib.CodeType.CcbUpca:
                    return "CCB UPC-A";
                case (int)CoreScannerLib.CodeType.CcbUpce:
                    return "CCB UPC-E";
                case (int)CoreScannerLib.CodeType.SignatureCapture:
                    return "SIGNATURE CAPTUREE";
                case (int)CoreScannerLib.CodeType.Moa:
                    return "MOA";
                case (int)CoreScannerLib.CodeType.Pdf417Parameter:
                    return "PDF417 PARAMETER";
                case (int)CoreScannerLib.CodeType.Chinese2of5:
                    return "CHINESE 2 OF 5";
                case (int)CoreScannerLib.CodeType.Korean3Of5:
                    return "KOREAN 3 OF 5";
                case (int)CoreScannerLib.CodeType.DatamatrixParam:
                    return "DATAMATRIX PARAM";
                case (int)CoreScannerLib.CodeType.CodeZ:
                    return "CODE Z";
                case (int)CoreScannerLib.CodeType.Upca_5:
                    return "UPC-A + 5 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Upce0_5:
                    return "UPC-E0 + 5 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Ean8_5:
                    return "EAN-8 + 5 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Ean13_5:
                    return "EAN-13 + 5 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.Upce1_5:
                    return "UPC-E1 + 5 SUPPLEMENTAL";
                case (int)CoreScannerLib.CodeType.MacroMicroPdf:
                    return "MACRO MICRO PDF";
                case (int)CoreScannerLib.CodeType.OcrB:
                    return "OCRB";
                case (int)CoreScannerLib.CodeType.OcrA:
                    return "OCRA";
                case (int)CoreScannerLib.CodeType.ParsedDriverLicense:
                    return "PARSED DRIVER LICENSE";
                case (int)CoreScannerLib.CodeType.ParsedUid:
                    return "PARSED UID";
                case (int)CoreScannerLib.CodeType.ParsedNdc:
                    return "PARSED NDC";
                case (int)CoreScannerLib.CodeType.DatabarCoupon:
                    return "DATABAR COUPON";
                case (int)CoreScannerLib.CodeType.ParsedXml:
                    return "PARSED XML";
                case (int)CoreScannerLib.CodeType.HanXinCode:
                    return "HAN XIN CODE";
                case (int)CoreScannerLib.CodeType.Calibration:
                    return "CALIBRATION";
                case (int)CoreScannerLib.CodeType.Gs1Datamatrix:
                    return "GS1 DATA MATRIX";
                case (int)CoreScannerLib.CodeType.Gs1Qr:
                    return "GS1 QR";
                case (int)CoreScannerLib.CodeType.Mainmark:
                    return "MAIL MARK";
                case (int)CoreScannerLib.CodeType.Dotcode:
                    return "DOT CODE";
                case (int)CoreScannerLib.CodeType.GridMatrix:
                    return "GRID MATRIX";
                case (int)CoreScannerLib.CodeType.EpcRaw:
                    return "EPC RAW";
                case (int)CoreScannerLib.CodeType.UdiCode:
                    return "UDI CODE";
                default:
                    return "";
            }
        }
        private void OnBarcodeEvent(short eventType, ref string pscanData)
        {
            GetDecodeBarcode(pscanData);
        }
        private void GetDataScan(string barcode, string scannerSN, string symbology)
        {
            DataScan scan = new DataScan(barcode, scannerSN, symbology);
            Logger.Write(scan.ToString(),this);
            ScanAction?.Invoke(scan);
        }
    }
}
