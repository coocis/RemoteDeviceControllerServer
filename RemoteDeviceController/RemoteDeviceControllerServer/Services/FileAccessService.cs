using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.IO;
using System.Windows.Forms;
using RemoteDeviceController.Util;
using System.Drawing;
using System.Drawing.Imaging;
using SevenZip;
using System.Threading;
using Accord;
using Accord.Video;
using System.Text.RegularExpressions;

namespace RemoteDeviceControllerServer.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FileAccessService" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class FileAccessService : IFileAccessService
    {
        private static Dictionary<ClientInfo, IFileAccessCallback> _clients = new Dictionary<ClientInfo, IFileAccessCallback>();
        private FileAccessServiceOptions _options;
        private Rectangle _screenRectangle;
        private List<ScreenCaptureStream> _screenCaptureStreams = new List<ScreenCaptureStream>();
        private DateTime _dt;
        private IFileAccessCallback _sendViewCallbacks;

        public FileAccessServiceOptions Options
        {
            get
            {
                return _options;
            }

            set
            {
                _options = value;
            }
        }

        public FileAccessService(FileAccessServiceOptions options)
        {
            _dt = DateTime.Now;
            Options = options;

            foreach (Screen screen in
                      Screen.AllScreens)
            {
                _screenRectangle = Rectangle.Union(_screenRectangle, screen.Bounds);
            }
            for (int i = 0; i < 2; i++)
            {
                ScreenCaptureStream stream = new ScreenCaptureStream(_screenRectangle);
                stream.FrameInterval = (1000 / 30);
                stream.NewFrame += new NewFrameEventHandler(video_NewFrame);
                _screenCaptureStreams.Add(stream);
            }
        }

        ~FileAccessService()
        {
            StopScreenCaptureStream();
        }

        public void Connect(string name)
        {
            ClientInfo clientInfo = new ClientInfo();
            clientInfo.name = name;

            _clients.Add(clientInfo,
                OperationContext.Current.GetCallbackChannel<IFileAccessCallback>());
        }

        public string GetServerName()
        {
            return Options.ServerName;
        }

        public List<string> GetAccessibleDirectories()
        {
            return _options.accessibleDirectory.ToList();
        }

        public List<string> GetDirectoryFileNames(string directory)
        {
            Console.WriteLine("Receive Get Directory Files Request: " + directory);
            if (Regex.IsMatch(directory, "[^:][\\\\]$"))
            {
                directory = directory.Remove(directory.Length - 1);
            }
            if (!Directory.Exists(directory))
            {
                return new List<string>();
            }
            return (from fileName
                   in Directory.GetFiles(directory).ToList()
                    where !(new FileInfo(fileName)).Attributes.HasFlag(FileAttributes.Hidden)
                    select fileName.Split('\\').Last())
                   .ToList();
        }

        public List<string> GetDirectoryFolderNames(string directory)
        {
            Console.WriteLine("Receive Get Directory Folders Request: " + directory);
            if (Regex.IsMatch(directory, "[^:][\\\\]$"))
            {
                directory = directory.Remove(directory.Length - 1);
            }
            if (!Directory.Exists(directory))
            {
                return new List<string>();
            }
            return (from folderName
                   in Directory.GetDirectories(directory).ToList()
                   where !(new DirectoryInfo(folderName)).Attributes.HasFlag(FileAttributes.Hidden)
                    select folderName.Split('\\').Last())
                   .ToList();
        }

        public byte[] GetFile(string filePath)
        {
            Console.WriteLine("Receive Get File Request: " + filePath);
            FileStream fs = new FileStream(filePath, FileMode.Open);
            //TODO: may not have enough space to get the whole file
            byte[] bs = new byte[fs.Length];
            fs.Read(bs, 0, bs.Length);
            fs.Close();
            if (!filePath.Split('\\').Last().Contains(FileEncryptor.encryptedFileExtension))
            {
                //bs = FileEncryptor.Encrypt(bs);
            }
            return bs;
        }

        public void GetFile(string filePath, Stream stream, int blockSize)
        {
            Console.WriteLine("Receive Get File Request: " + filePath);
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bs = new byte[blockSize];
            int i = 0;
            try
            {
                while ((i = fs.Read(bs, 0, blockSize)) > 0)
                {
                    stream.Write(bs, 0, i);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            fs.Close();
        }

        public long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public byte[] GetFileThumbnail(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            byte[] bs = new byte[fs.Length];
            fs.Read(bs, 0, bs.Length);
            fs.Close();
            string fileExtension = "." + filePath.Split('.').Last();
            if (fileExtension.Contains(FileEncryptor.encryptedFileExtension))
            {
                bs = FileEncryptor.Decrypt(bs);
            }
            return ImageProcessor.GetThumbnail(bs);
        }

        public void StartGetView(string clientID)
        {
            IFileAccessCallback callback = OperationContext.Current.GetCallbackChannel<IFileAccessCallback>();
            ClientInfo c = new ClientInfo();
            c.id = clientID;
            c.screenShotService = true;
            _clients.Add(c, callback);
            _sendViewCallbacks = callback;
            StartScreenCaptureStream();
        }

        public void StopGetView(string clientID)
        {
            _clients.Remove((from c in _clients where c.Key.id.Equals(clientID) select c.Key).FirstOrDefault());

            if (_clients.Count == 0 || _clients.Keys.First(c => c.screenShotService) == null)
            {
                StopScreenCaptureStream();
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            MemoryStream ms = new MemoryStream();
            Bitmap origin = eventArgs.Frame;
            //origin.SetResolution(_screenRectangle.Width * 0.8f, _screenRectangle.Height * 0.8f);
            using (Graphics g = Graphics.FromImage(origin))
            {
                Rectangle cursor = new Rectangle(Control.MousePosition, new Size(18, 13));
                Cursors.Default.Draw(g, cursor);
            }
            Bitmap resize;
            resize = new Bitmap(origin, 
                new Size((int)(_screenRectangle.Width * 0.8), (int)(_screenRectangle.Height * 0.8)));
            resize.Save(ms, ImageFormat.Jpeg);
            /*
            origin.Save(ms, ImageFormat.Jpeg);
            */
            //(from c in _clients where c.Key.screenShotService select c.Value).ToList().ForEach(cb => cb.SendView(ms.ToArray()));
            _sendViewCallbacks.SendView(ms.ToArray());
            ms.Close();
            //resize.Dispose();
        }

        private void StartScreenCaptureStream()
        {
            _screenCaptureStreams.ForEach(
                s =>
                {
                    if (!s.IsRunning)
                    {
                        s.Start();
                    }
                }
                );
        }

        private void StopScreenCaptureStream()
        {
            _screenCaptureStreams.ForEach(
                s =>
                {
                    if (s.IsRunning)
                    {
                        s.Stop();
                    }
                }
                );
        }

        private byte[] Compress_7zip(byte[] data)
        {
            byte[] result = SevenZipCompressor.CompressBytes(data);
            Console.WriteLine("{0,2}", (float)result.Length / data.Length);
            return result;
        }
    }

    class ClientInfo
    {
        public string id;
        public string name;
        public string ip;
        public int port;
        public bool screenShotService = false;
    }
}
