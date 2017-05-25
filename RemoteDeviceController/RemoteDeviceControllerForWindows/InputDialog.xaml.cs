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

namespace RemoteDeviceControllerForWindows
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        private bool _isOK = false;

        private InputDialog()
        {
            InitializeComponent();

            inputTextBox.Focus();
        }

        public static string Show(string tip)
        {
            InputDialog dialog = new InputDialog();
            dialog.tipTextBlock.Text = tip;
            dialog.ShowDialog();
            if (dialog._isOK)
            {
                return dialog.inputTextBox.Text;
            }
            else
            {
                return null;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _isOK = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isOK = false;
            Close();
        }
    }
}
