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
using System.Windows.Shapes;
using RemoteDeviceControllerForWindows.FileAccessServiceReference;

namespace RemoteDeviceControllerForWindows
{
    /// <summary>
    /// Interaction logic for TextViewerWindow.xaml
    /// </summary>
    public partial class TextViewerWindow : Window
    {

        private string _directroy;
        private string _fileName;
        private FileAccessServiceClient _client;

        public TextViewerWindow(string directory, string fileName)
        {
            InitializeComponent();
            _directroy = directory;
            _fileName = fileName;
            _client = ClientCreator.fileAccessClient;
            Title = fileName;
            contentTextBlock.Text = Encoding.UTF8.GetString(_client.GetFile(directory + "\\" + fileName));
        }
    }
}
