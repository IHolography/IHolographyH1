using System;
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
using Logger;
using ScannerService;
using WeigherService;
using System.IO.Ports;

namespace IHolographyH1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {

        List<DataScan> dataScans = new List<DataScan>();
        ScannerManager scanListen;
        Weigher weigher;


        public MainWindow()
        {
            //StartScanListen scanListen = new StartScanListen();
            //scanListen.SetScanProductOrBoxProperties(ScannerAction.BoxScan);
            //scanListen.StartScannListener();

            weigher = new Weigher("COM2", 9600, Parity.None, StopBits.Two, 8, 5, AppDefs.Constant.LogEnable, AppDefs.Variable.DateTimeFormat, AppDefs.Constant.LogFilePath);
            weigher.WeighFinish += ShowMess;
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ScannerManager.FailedAppEvent += CloseApp;
            try
            {
                scanListen = new ScannerManager();
                scanListen.StartScannListener();
            }
            catch 
            {
                Log.Write("Application failed");
                Application.Current.Shutdown();
            }
        }
        private void CloseApp(string mess)
        {
            Log.Write(mess);
            MessageBox.Show("SSSs");
            //Application.Current.Shutdown();
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataScan s in dataScans)
            {
                MessageBox.Show(s.ToString()+sender.ToString());
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = dataScans;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (scanListen != null)
            {
                scanListen.StopScannListener();
            }
            //scanListen = null;
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            weigher.GetData();
        }
        private void ShowMess(float val)
        {
            MessageBox.Show(val.ToString());

        }
    }
}
