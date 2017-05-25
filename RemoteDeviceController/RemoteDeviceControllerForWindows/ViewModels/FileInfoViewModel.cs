using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RemoteDeviceControllerForWindows.ViewModels
{
    class FileInfoViewModel : INotifyPropertyChanged
    {
        private ImageSource _icon;
        private string _name;
        private bool _isFile;

        public ImageSource Icon
        {
            get
            {
                return _icon;
            }

            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsFile
        {
            get
            {
                return _isFile;
            }

            set
            {
                _isFile = value;
                OnPropertyChanged("IsFile");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
 