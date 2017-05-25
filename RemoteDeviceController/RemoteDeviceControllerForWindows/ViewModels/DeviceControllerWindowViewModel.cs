using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RemoteDeviceControllerForWindows.ViewModels
{
    public class DeviceControllerWindowViewModel : INotifyPropertyChanged
    {
        private BitmapImage _viewImage;

        public BitmapImage ViewImage
        {
            get
            {
                return _viewImage;
            }

            set
            {
                _viewImage = value;
                OnPropertyChanged("ViewImage");
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
