using AppDefs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace IHolographyH1
{
    public static class XMLReader
    {
        public static string Barcode { get; private set; }
        public static string Symbology { get; private set; }
        public static string ScannerSerialNumber { get; private set; }
        public static string ScanerID { get; private set; }

        static XMLReader()
        {
        }

        public static void ParseXML(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            string strData = String.Empty;
            try
            {
                string scanerID = xmlDoc.DocumentElement.SelectNodes("scannerID")[0].InnerText;
                string barcode = xmlDoc.DocumentElement.GetElementsByTagName("datalabel").Item(0).InnerText;
                string symbology = xmlDoc.DocumentElement.GetElementsByTagName("datatype").Item(0).InnerText;
                string scannerSerialNumber = xmlDoc.DocumentElement.GetElementsByTagName("serialnumber").Item(0).InnerText;
                string[] numbers = barcode.Split(' ');

                foreach (string number in numbers)
                {
                    if (String.IsNullOrEmpty(number))
                    {
                        break;
                    }
                    strData += ((char)Convert.ToInt32(number, 16)).ToString();
                }
                Symbology = GetSymbology((int)Convert.ToInt32(symbology));
                Barcode = strData;
                ScannerSerialNumber = scannerSerialNumber;
                ScanerID = scanerID;
            }
            catch
            {
                Barcode = String.Empty;
                Symbology = String.Empty;
                ScannerSerialNumber = String.Empty;
                ScanerID = String.Empty;
            }
        }
        public static int ParseXML(string xml, ref List<Scanner> ListConnectedScanners)
        {
            int status = (int)Status.Failed;
            string scannerID = String.Empty;
            string serialnumber = String.Empty;
            string guid = String.Empty;
            string vid = String.Empty;
            string pid = String.Empty;
            string modelnumber = String.Empty;
            string dom = String.Empty;
            string firmware = String.Empty;

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                XmlElement xRoot = xmlDoc.DocumentElement;

                foreach (XmlNode xnode in xRoot)
                {
                    foreach (XmlNode childnode in xnode.ChildNodes)
                    {
                        if (childnode.Name == "scannerID")
                        {
                            scannerID = childnode.InnerText;
                        }
                        if (childnode.Name == "serialnumber")
                        {
                            serialnumber = childnode.InnerText;
                        }
                        if (childnode.Name == "GUID")
                        {
                            guid = childnode.InnerText;
                        }
                        if (childnode.Name == "VID")
                        {
                            vid = childnode.InnerText;
                        }
                        if (childnode.Name == "PID")
                        {
                            pid = childnode.InnerText;
                        }
                        if (childnode.Name == "modelnumber")
                        {
                            modelnumber = childnode.InnerText;
                        }
                        if (childnode.Name == "DoM")
                        {
                            dom = childnode.InnerText;
                        }
                        if (childnode.Name == "firmware")
                        {
                            firmware = childnode.InnerText;
                        }
                    }
                    ListConnectedScanners.Add(new Scanner(scannerID, serialnumber, guid, vid, pid, modelnumber, dom, firmware));
                }
                if(!ListConnectedScanners.Any()) status = (int)Status.Success;
            }
            catch
            {
                Logger.Write("Couldn't parse XML with connected scanners");
            }
            return status;
        }
        private static string GetSymbology(int Code)
        {
            switch (Code)
            {
                case (int)CodeType.Unknown:
                    return "NOT APPLICABLE";
                case (int)CodeType.Code39:
                    return "CODE 39";
                case (int)CodeType.Codabar:
                    return "CODABAR";
                case (int)CodeType.Code128:
                    return "CODE 128";
                case (int)CodeType.Discrete2of5:
                    return "DISCRETE 2 OF 5";
                case (int)CodeType.Iata:
                    return "IATA";
                case (int)CodeType.Interleaved2of5:
                    return "INTERLEAVED 2 OF 5";
                case (int)CodeType.Code93:
                    return "CODE 93";
                case (int)CodeType.UpcA:
                    return "UPC-A";
                case (int)CodeType.UpcE0:
                    return "UPC-E0";
                case (int)CodeType.Ean8:
                    return "EAN-8";
                case (int)CodeType.Ean13:
                    return "EAN-13";
                case (int)CodeType.Code11:
                    return "CODE 11";
                case (int)CodeType.Code49:
                    return "CODE 49";
                case (int)CodeType.Msi:
                    return "MSI";
                case (int)CodeType.Ean128:
                    return "EAN-128";
                case (int)CodeType.UpcE1:
                    return "UPC-E1";
                case (int)CodeType.Pdf417:
                    return "PDF-417";
                case (int)CodeType.Code16k:
                    return "CODE 16K";
                case (int)CodeType.Code39FullAscii:
                    return "CODE 39 FULL ASCII";
                case (int)CodeType.UpcD:
                    return "UPC-D";
                case (int)CodeType.Code39Trioptic:
                    return "CODE 39 TRIOPTIC";
                case (int)CodeType.Bookland:
                    return "BOOKLAND";
                case (int)CodeType.UpcaWCode128:
                    return "UPC-A w/Code 128 Supplemental";
                case (int)CodeType.Jan13WCode128:
                    return "EAN/JAN-13 w/Code 128 Supplemental";
                case (int)CodeType.Nw7:
                    return "NW-7";
                case (int)CodeType.Isbt128:
                    return "ISBT-128";
                case (int)CodeType.MicroPdf:
                    return "MICRO PDF";
                case (int)CodeType.Datamatrix:
                    return "DATAMATRIX";
                case (int)CodeType.Qrcode:
                    return "QR CODE";
                case (int)CodeType.MicroPdfCca:
                    return "MICRO PDF CCA";
                case (int)CodeType.PostnetUs:
                    return "POSTNET US";
                case (int)CodeType.PlanetCode:
                    return "PLANET CODE";
                case (int)CodeType.Code32:
                    return "CODE 32";
                case (int)CodeType.Isbt128Con:
                    return "ISBT-128 CON";
                case (int)CodeType.JapanPostal:
                    return "JAPAN POSTAL";
                case (int)CodeType.AusPostal:
                    return "AUS POSTAL";
                case (int)CodeType.DutchPostal:
                    return "DUTCH POSTAL";
                case (int)CodeType.Maxicode:
                    return "MAXICODE";
                case (int)CodeType.CanadinPostal:
                    return "CANADIAN POSTAL";
                case (int)CodeType.UkPostal:
                    return "UK POSTAL";
                case (int)CodeType.MacroPdf:
                    return "MACRO PDF";
                case (int)CodeType.MacroQrCode:
                    return "MACRO QR CODE";
                case (int)CodeType.MicroQrCode:
                    return "MICRO QR CODE";
                case (int)CodeType.Aztec:
                    return "AZTEC";
                case (int)CodeType.AztecRune:
                    return "AZTEC RUNE";
                case (int)CodeType.Distance:
                    return "DISTANCE";
                case (int)CodeType.Rss14:
                    return "GS1 DATABAR";
                case (int)CodeType.RssLimited:
                    return "GS1 DATABAR LIMITED";
                case (int)CodeType.RssExpanded:
                    return "GS1 DATABAR EXPANDED";
                case (int)CodeType.Parameter:
                    return "PARAMETER";
                case (int)CodeType.Usps4c:
                    return "USPS 4CB";
                case (int)CodeType.UpuFicsPostal:
                    return "UPU FICS POSTAL";
                case (int)CodeType.Issn:
                    return "ISSN";
                case (int)CodeType.Scanlet:
                    return "SCANLET";
                case (int)CodeType.Cuecode:
                    return "CUECODE";
                case (int)CodeType.Matrix2of5:
                    return "MATRIX 2 OF 5";
                case (int)CodeType.Upca_2:
                    return "UPC-A + 2 SUPPLEMENTAL";
                case (int)CodeType.Upce0_2:
                    return "UPC-E0 + 2 SUPPLEMENTAL";
                case (int)CodeType.Ean8_2:
                    return "EAN-8 + 2 SUPPLEMENTAL";
                case (int)CodeType.Ean13_2:
                    return "EAN-13 + 2 SUPPLEMENTAL";
                case (int)CodeType.Upce1_2:
                    return "UPC-E1 + 2 SUPPLEMENTAL";
                case (int)CodeType.CcaEan128:
                    return "CCA EAN-128";
                case (int)CodeType.CcaEan13:
                    return "CCA EAN-13";
                case (int)CodeType.CcaEan8:
                    return "CCA EAN-8";
                case (int)CodeType.CcaRssExpanded:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCA)";
                case (int)CodeType.CcaRssLimited:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCA)";
                case (int)CodeType.CcaRss14:
                    return "GS1 DATABAR COMPOSITE (CCA)";
                case (int)CodeType.CcaUpca:
                    return "CCA UPC-A";
                case (int)CodeType.CcaUpce:
                    return "CCA UPC-E";
                case (int)CodeType.CccEan128:
                    return "CCC EAN-128";
                case (int)CodeType.Tlc39:
                    return "TLC-39";
                case (int)CodeType.CcbEan128:
                    return "CCB EAN-128";
                case (int)CodeType.CcbEan13:
                    return "CCB EAN-13";
                case (int)CodeType.CcbEan8:
                    return "CCB EAN-8";
                case (int)CodeType.CcbRssExpanded:
                    return "GS1 DATABAR EXPANDED COMPOSITE (CCB)";
                case (int)CodeType.CcbRssLimited:
                    return "GS1 DATABAR LIMITED COMPOSITE (CCB)";
                case (int)CodeType.CcbRss14:
                    return "GS1 DATABAR COMPOSITE (CCB)";
                case (int)CodeType.CcbUpca:
                    return "CCB UPC-A";
                case (int)CodeType.CcbUpce:
                    return "CCB UPC-E";
                case (int)CodeType.SignatureCapture:
                    return "SIGNATURE CAPTUREE";
                case (int)CodeType.Moa:
                    return "MOA";
                case (int)CodeType.Pdf417Parameter:
                    return "PDF417 PARAMETER";
                case (int)CodeType.Chinese2of5:
                    return "CHINESE 2 OF 5";
                case (int)CodeType.Korean3Of5:
                    return "KOREAN 3 OF 5";
                case (int)CodeType.DatamatrixParam:
                    return "DATAMATRIX PARAM";
                case (int)CodeType.CodeZ:
                    return "CODE Z";
                case (int)CodeType.Upca_5:
                    return "UPC-A + 5 SUPPLEMENTAL";
                case (int)CodeType.Upce0_5:
                    return "UPC-E0 + 5 SUPPLEMENTAL";
                case (int)CodeType.Ean8_5:
                    return "EAN-8 + 5 SUPPLEMENTAL";
                case (int)CodeType.Ean13_5:
                    return "EAN-13 + 5 SUPPLEMENTAL";
                case (int)CodeType.Upce1_5:
                    return "UPC-E1 + 5 SUPPLEMENTAL";
                case (int)CodeType.MacroMicroPdf:
                    return "MACRO MICRO PDF";
                case (int)CodeType.OcrB:
                    return "OCRB";
                case (int)CodeType.OcrA:
                    return "OCRA";
                case (int)CodeType.ParsedDriverLicense:
                    return "PARSED DRIVER LICENSE";
                case (int)CodeType.ParsedUid:
                    return "PARSED UID";
                case (int)CodeType.ParsedNdc:
                    return "PARSED NDC";
                case (int)CodeType.DatabarCoupon:
                    return "DATABAR COUPON";
                case (int)CodeType.ParsedXml:
                    return "PARSED XML";
                case (int)CodeType.HanXinCode:
                    return "HAN XIN CODE";
                case (int)CodeType.Calibration:
                    return "CALIBRATION";
                case (int)CodeType.Gs1Datamatrix:
                    return "GS1 DATA MATRIX";
                case (int)CodeType.Gs1Qr:
                    return "GS1 QR";
                case (int)CodeType.Mainmark:
                    return "MAIL MARK";
                case (int)CodeType.Dotcode:
                    return "DOT CODE";
                case (int)CodeType.GridMatrix:
                    return "GRID MATRIX";
                case (int)CodeType.EpcRaw:
                    return "EPC RAW";
                case (int)CodeType.UdiCode:
                    return "UDI CODE";
                default:
                    return "";
            }
        }
    }
}
