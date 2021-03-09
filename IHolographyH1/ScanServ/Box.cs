using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using AppDefs;
using Logger;
using ScannerService;

namespace IHolographyH1.ScanServ
{
    public class Box
    {
        public Scanner Scanner { get; private set; }
        public string Barcode { get; private set; }
        public string BoxCreateTime { get; private set; }
        public List<Product> Products { get; private set; }
        List<Product> products;

        public Box(DataScan data)
        {
            products = new List<Product>();
            Scanner = data.Scanner;
            Barcode = data.Barcode;
            BoxCreateTime = data.CreateDateTime;
        }

        public void AddProduct(Product product)
        {
            products.Add(product);
            UpdateList();
        }
        private void UpdateList()
        {
            Products =new List<Product>(products);
        }
        public void FinishBox()
        {
            //TODO: write in DB after check
        }
        public void GetLast()
        {
            if (products != null && products.Count>0)
            {
                products.RemoveAt(products.Count-1);
            }
            else
            {
                Log.Write("Cant delete last product from box, becouse it's empty",this);
            }
        }
        public void DisBand()
        {
            //TODO: disband box ???
        }


    }
}
