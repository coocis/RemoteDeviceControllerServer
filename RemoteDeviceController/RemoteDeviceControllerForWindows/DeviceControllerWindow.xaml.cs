using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using RemoteDeviceControllerForWindows.FileAccessServiceReference;
using RemoteDeviceControllerForWindows.ViewModels;
using SevenZip;
using System.Drawing.Imaging;

namespace RemoteDeviceControllerForWindows
{
    /// <summary>
    /// Interaction logic for DeviceControllerWindow.xaml
    /// </summary>
    public partial class DeviceControllerWindow : Window
    {
        private DeviceControllerWindowViewModel _viewModel;
        private FileAccessServiceClient _client;
        private int _fps;
        private float _count = 0;
        private DateTime _dt;

        public DeviceControllerWindowViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }

            set
            {
                _viewModel = value;
            }
        }
        

        public DeviceControllerWindow(FileAccessServiceClient client)
        {
            InitializeComponent();

            _fps = 30;
            ViewModel = DataContext as DeviceControllerWindowViewModel;
            _client = client;
            _client.StartGetView("asb");
            _dt = DateTime.Now;
        }

        public void SetImage(byte[] bs)
        {
            _count++;
            if (_count >= 100)
            {
                fpsTextBlock.Text = (_count / (DateTime.Now - _dt).TotalSeconds).ToString();
                _count = 0;
                _dt = DateTime.Now;
            }
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bs);
            bitmap.EndInit();
            ViewModel.ViewImage = bitmap;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _client.StopGetView("asb");

        }
    }
}
