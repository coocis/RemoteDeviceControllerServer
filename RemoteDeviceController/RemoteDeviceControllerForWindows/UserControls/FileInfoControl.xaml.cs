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
    /// Interaction logic for FileInfoControl.xaml
    /// </summary>
    public partial class FileInfoControl : UserControl
    {
        public delegate void SaveAsButtonOnClickHandler(FileInfoControl fic);
        public event SaveAsButtonOnClickHandler SaveAsButton_OnClick;
        public delegate void EncryptAsButtoOnClickHandler(FileInfoControl fic);
        public event EncryptAsButtoOnClickHandler EncryptAsButton_OnClick;
        public delegate void DecryptAsButtonOnClickHandler(FileInfoControl fic);
        public event DecryptAsButtonOnClickHandler DecryptAsButton_OnClick;

        public FileInfoControl()
        {
            InitializeComponent();
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(FileInfoControl), new PropertyMetadata(null));



        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FileName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FileNameProperty =
            DependencyProperty.Register("FileName", typeof(string), typeof(FileInfoControl), new PropertyMetadata(""));



        public bool IsFile
        {
            get { return (bool)GetValue(IsFileProperty); }
            set { SetValue(IsFileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFolder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFileProperty =
            DependencyProperty.Register("IsFile", typeof(bool), typeof(FileInfoControl), new PropertyMetadata(false));


        private void saveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SaveAsButton_OnClick != null)
            {
                SaveAsButton_OnClick.Invoke(this);
            }
        }

        private void encryptAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (EncryptAsButton_OnClick != null)
            {
                EncryptAsButton_OnClick.Invoke(this);
            }
        }

        private void decryptAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (DecryptAsButton_OnClick != null)
            {
                DecryptAsButton_OnClick.Invoke(this);
            }
        }
    }
}
