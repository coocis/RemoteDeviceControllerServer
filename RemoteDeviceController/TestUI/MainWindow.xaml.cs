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
using ImageFile = System.Drawing.Image;
using System.IO;

using RemoteDeviceController.Util;

namespace TestUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FileEncryptor.Encrypt("a", "b");
            byte[] bs = FileEncryptor.Decrypt("b");
            FileStream sw = new FileStream("c.jpg", FileMode.Create);
            sw.Write(bs, 0, bs.Length);
            sw.Close();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(bs);
            bitmap.EndInit();
            image.Source = bitmap;
        }
    }
}
