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

namespace RemoteDeviceControllerForWindows.UserControls
{
    /// <summary>
    /// Interaction logic for ServerInfoControl.xaml
    /// </summary>
    public partial class ServerInfoControl : UserControl
    {
        public ServerInfoControl()
        {
            InitializeComponent();
        }

        public string ServerName
        {
            get { return (string)GetValue(ServerNameProperty); }
            set { SetValue(ServerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServerNameProperty =
            DependencyProperty.Register("ServerName", typeof(string), typeof(ServerInfoControl), new PropertyMetadata("server"));
        

        public string ServiceAddress
        {
            get { return (string)GetValue(ServiceAddressProperty); }
            set { SetValue(ServiceAddressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ServiceAddress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ServiceAddressProperty =
            DependencyProperty.Register("ServiceAddress", typeof(string), typeof(ServerInfoControl), new PropertyMetadata("unknow"));


        public bool IsFileAccessServiceSupport
        {
            get { return (bool)GetValue(IsFileAccessServiceSupportProperty); }
            set { SetValue(IsFileAccessServiceSupportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFileAccessServiceSupport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileAccessServiceSupportProperty =
            DependencyProperty.Register("IsFileAccessServiceSupport", typeof(bool), typeof(ServerInfoControl), new PropertyMetadata(false));

    }
}
