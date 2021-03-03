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
using AppDefs;

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
            Scan.ScannerAction = ScannerAction.Undefined;
            
            Scan scan = new Scan(COM.CoreScannerObject);
            


        }


        private void button_Click(object sender, RoutedEventArgs e)
        {
            Scan.ResetAlm();
        }
    }
}
