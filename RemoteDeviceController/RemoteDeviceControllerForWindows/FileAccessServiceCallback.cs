using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RemoteDeviceControllerForWindows.FileAccessServiceReference;

namespace RemoteDeviceControllerForWindows
{
    public class FileAccessServiceCallback : IFileAccessServiceCallback
    {
        public void SendView(byte[] data)
        {
            MainWindow.Instance.DeviceControllerWindow.SetImage(data);
        }
    }
}
