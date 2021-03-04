﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using AppDefs;

namespace IHolographyH1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        List<DataScan> dataScans = new List<DataScan>();
       
        public MainWindow()
        {
            InitializeComponent();
            TestThread();
            //StartScannerListener();
        }
        public void TestThread()
        {
            ScanListener.ScanListenerEvent += DeleteObject;
            Thread thread = new Thread(new ThreadStart(StartScannerListener)) { Name="Test"};
            thread.IsBackground = true;
            thread.Start();
            Thread.Sleep(200);
        }
        public void StartScannerListener()
        {
            COM.OpenConnection();
            ScanListener.ScannerAction = ScannerAction.BoxScan;
            ScanListener scanListener = new ScanListener(COM.CoreScannerObject);
            scanListener.ScanEvent += Scan;
            // MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            //scanListener.Check();
            // scanListener.ResetAlm();
        }
        private void DeleteObject(Status status)
        {
            //MessageBox.Show(status.ToString());
            Logger.Write("Сработало событие");

        }
        private void Scan(DataScan scan)
        {
            MessageBox.Show(scan.ToString());
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataScan s in dataScans)
            {
                MessageBox.Show(s.ToString());
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = dataScans;
        }
    }
}
