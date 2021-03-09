using ScannerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDefs;

namespace IHolographyH1
{
    public static class Analyzer
    {
        //Mode: -Test -Work
        //Action:  -Box -Product
        public static void Analize(DataScan data)
        {
            if (data.ScannerAction==(int)ScannerAction.BoxScan)
            {
                //newBox or delete Box
            }
            if (data.ScannerAction == (int)ScannerAction.ProductScan)
            {
                //addProduct or getProduct
            }
        }


    }
}
