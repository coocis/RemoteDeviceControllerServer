using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RemoteDeviceController.Util
{
    public class ImageProcessor
    {
        public static byte[] GetThumbnail(byte[] image)
        {
            Image i = Image.FromStream(new MemoryStream(image));
            Image i2 = i.GetThumbnailImage(120, 120, new Image.GetThumbnailImageAbort(() => true), IntPtr.Zero);
            i.Dispose();
            MemoryStream ms = new MemoryStream();
            i2.Save(ms, ImageFormat.Jpeg);
            return ms.ToArray();
        }
    }
}
