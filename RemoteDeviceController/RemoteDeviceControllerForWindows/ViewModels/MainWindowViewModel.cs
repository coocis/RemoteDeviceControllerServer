using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RemoteDeviceControllerForWindows.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ServerInfoViewModel> _servers = 
            new ObservableCollection<ServerInfoViewModel>();


        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ServerInfoViewModel> Servers
        {
            get
            {
                return _servers;
            }

            set
            {
                _servers = value;
                OnPropertyChanged("Servers");
            }
        }


        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
