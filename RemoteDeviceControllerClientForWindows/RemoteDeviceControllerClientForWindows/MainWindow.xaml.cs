using System;
using System.Collections.Generic;
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
using System.ServiceModel;
using RemoteDeviceControllerClientForWindows.ServiceReference1;


namespace RemoteDeviceControllerClientForWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void runButton_Click(object sender, RoutedEventArgs e)
        {

            //Service1Client client = new Service1Client(new NetTcpBinding(SecurityMode.None, false),
            //    new EndpointAddress("net.tcp://localhost:2820/RemoteDeviceControllerServer/Service1.svc"));

            //Service1Client client = new Service1Client(new WSDualHttpBinding(),
            //    new EndpointAddress("http://localhost:2829/Service1.svc"));
            Service1Client client2 = new Service1Client();
            resultTextBox.Text = client2.GetData(int.Parse(resultTextBox.Text));
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            Service1Client client2 = new Service1Client(new BasicHttpBinding(),
                new EndpointAddress("http://" + ipTextBox.Text + "/Service1.svc"));
            ipTextBox.Text = client2.GetData(1);
        }
    }
}
