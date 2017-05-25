using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDeviceControllerForWindows.ViewModels
{
    class ServerInfoViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _serviceAddress;
        private int _port;
        private bool _isFileAccessServiceSupport;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public string ServiceAddress
        {
            get
            {
                return _serviceAddress;
            }

            set
            {
                _serviceAddress = value;
                OnPropertyChanged("ServiceAddress");
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }

            set
            {
                _port = value;
                OnPropertyChanged("Port");
            }
        }

        public bool IsFileAccessServiceSupport
        {
            get
            {
                return _isFileAccessServiceSupport;
            }

            set
            {
                _isFileAccessServiceSupport = value;
                OnPropertyChanged("IsFileAccessServiceSupport");
            }
        }

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
