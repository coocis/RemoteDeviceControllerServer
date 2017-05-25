using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace RemoteDeviceControllerForWindows.ViewModels
{
    class FileBrowserWindowViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<FileInfoViewModel> _fileInfos =
            new ObservableCollection<FileInfoViewModel>();
        private string _currentDirectory;

        public ObservableCollection<FileInfoViewModel> FileInfos
        {
            get
            {
                return _fileInfos;
            }

            set
            {
                _fileInfos = value;
                OnPropertyChanged("FileInfos");
            }
        }

        public string CurrentDirectory
        {
            get
            {
                return _currentDirectory;
            }

            set
            {
                _currentDirectory = value;
                OnPropertyChanged("CurrentDirectory");
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
