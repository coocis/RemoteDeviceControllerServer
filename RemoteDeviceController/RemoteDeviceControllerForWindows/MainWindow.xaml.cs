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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using RemoteDeviceControllerForWindows.ViewModels;
using RemoteDeviceControllerForWindows.UserControls;
using System.Collections.ObjectModel;
using RemoteDeviceControllerForWindows.NATTraversalServiceReference;
using System.IO;

namespace RemoteDeviceControllerForWindows
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        private FileBrowserWindow _fileBrowserWindow;
        private DeviceControllerWindow _deviceControllerWindow;

        private MainWindowViewModel _viewModel;
        
        public static MainWindow Instance
        {
            get
            {
                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        public FileBrowserWindow FileBrowserWindow
        {
            get
            {
                return _fileBrowserWindow;
            }

            private set
            {
                _fileBrowserWindow = value;
            }
        }

        public DeviceControllerWindow DeviceControllerWindow
        {
            get
            {
                return _deviceControllerWindow;
            }

            private set
            {
                _deviceControllerWindow = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            _instance = this;
            _viewModel = DataContext as MainWindowViewModel;
            _viewModel.Servers = new ObservableCollection<ServerInfoViewModel>();
             List<string> serverInfos = ClientCreator.SearchServers_FileAccess();
            //List<string> serverInfos = new List<string>();
            foreach (var info in serverInfos)
            {
                string[] ss = info.Split(' ');
                ServerInfoViewModel si = new ServerInfoViewModel();
                si.Name = ss[0];
                //TODO: get ip address and port from endpoint address
                si.ServiceAddress = ss[1];
                si.Port = 0;
                si.IsFileAccessServiceSupport = true;
                _viewModel.Servers.Add(si);

                //FileBrowserWindow = new FileBrowserWindow(ClientCreator.ConnectServer_FileAccess(si.ServiceAddress));
                //FileBrowserWindow.Show();
                //DeviceControllerWindow = new DeviceControllerWindow(ClientCreator.ConnectServer_FileAccess(si.ServiceAddress));
                //DeviceControllerWindow.Show();
            }

            //WebClient wc = new WebClient();
            //StreamReader sr = new StreamReader(wc.OpenRead("http://coocis.net/NATTraversalModels"));
            //ttextBox.Text = sr.ReadToEnd();
            
            //FileBrowserWindow fbw = new FileBrowserWindow(
                //ClientCreator.ConnectServer_FileAccess("net.tcp://183.63.156.106/service"));
                //ClientCreator.ConnectServer_FileAccess("net.tcp://10.173.233.32:8111/service"));
            
            //fbw.Show();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void ipAddressTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;

            new FileBrowserWindow(ClientCreator.ConnectServer_FileAccess(tb.Text));
        }

        private void ServerInfoControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var serverInfo = sender as ServerInfoControl;
            FileBrowserWindow fbw = new FileBrowserWindow(
                ClientCreator.ConnectServer_FileAccess(serverInfo.ServiceAddress));
            fbw.Show();
        }
    }
}
