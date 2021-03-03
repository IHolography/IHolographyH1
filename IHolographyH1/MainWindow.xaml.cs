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
using IHolographyH1.Scaners;
using System.Threading;
using System.IO;

namespace IHolographyH1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<DataScan> dataScans = new List<DataScan>();
        public MainWindow()
        {
            InitializeComponent();

            COM.OpenConnection();

            
            Scanners scanners = new Scanners();
            scanners.GetConnectedScanners(out string outXml);
            scanners.RegisterForEvent();
            scanners.ScanAction += GetScanAndPutInList;


        }

        public void GetScanAndPutInList(DataScan dataScan)
        {
            dataScans.Add(dataScan);
        }
        public void ShowDataScans()
        {
            foreach(DataScan dataScan in dataScans)
            {
                MessageBox.Show(dataScan.Barcode);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ShowDataScans();
        }
    }
}
