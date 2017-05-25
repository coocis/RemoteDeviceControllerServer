using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Open.Nat;
using System.Net.Mime;


using RemoteDeviceControllerServer.Services;
using RemoteDeviceController.Util;
using System.IO;

namespace RemoteDeviceControllerServer
{
    public class Server
    {
        private string _serverName = "server1";
        private ServiceHost _serviceHost;
        private Thread _udpBoardcastThread;
        private Thread _socketServerThread;
        private Thread _httpServerThread;
        private FileAccessService _fileAccessService;
        private int _id;
        private UdpClient _udpClient;
        private Socket _server;
        private HttpListener _httpServer;

        public Server(FileAccessServiceOptions options)
        {
            //10.173.233.32
            //_serviceHost = new ServiceHost(typeof(FileAccessService), new Uri("net.tcp://localhost:8111/service"));
            //if you want to control the service, use the following instead
            _fileAccessService = new FileAccessService(options);
            _serviceHost = new ServiceHost(_fileAccessService, new Uri("net.tcp://10.173.233.32:8111/service"));
            var tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxBufferSize = int.MaxValue;
            tcpBinding.MaxReceivedMessageSize = int.MaxValue;
            _serviceHost.AddServiceEndpoint(typeof(IFileAccessService), tcpBinding, "");
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = false;
            _serviceHost.Description.Behaviors.Add(smb);
            _serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
            _serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            _serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            _serviceHost.Open();
            _udpBoardcastThread = new Thread(StartListenUDPBoardcast);
            _udpBoardcastThread.Start();
            NATTraversalSignIn();
            Console.WriteLine(_id);

            _socketServerThread = new Thread(StartSocketServer);
            _socketServerThread.Start();
            _httpServerThread = new Thread(StartHTTPServer);
            _httpServerThread.Start();
            Console.WriteLine(GetLocalIp());
        }

        ~Server()
        {
            _udpBoardcastThread.Abort();
            _socketServerThread.Abort();
            _httpServerThread.Abort();
            _udpClient.Close();
            _serviceHost.Close();
            _server.Close();
            _httpServer.Stop();
            NATTraversalSignOut();
        }
        
        public void Close()
        {
            _udpBoardcastThread.Abort();
            _serviceHost.Close();
        }

        private void StartListenUDPBoardcast()
        {
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 8111));
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                byte[] data = _udpClient.Receive(ref endpoint);
                if (data.Count() > 0)
                {
                    string ip = Encoding.UTF8.GetString(data);
                    Console.WriteLine("Receive UDP Boardcast from: " + ip);
                    byte[] localIpBytes = Encoding.UTF8.GetBytes(GetLocalIp());
                    _udpClient.Send(localIpBytes, localIpBytes.Length, new IPEndPoint(IPAddress.Parse(ip), 8111));
                }
                Thread.Sleep(1000);
            }
        }
        
        private void StartSocketServer()
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(new IPEndPoint(IPAddress.Parse(GetLocalIp()), 8333));
            _server.Listen(0);
            Socket client = _server.Accept();
            Console.WriteLine("Accept Client");
            while (true)
            {
                try
                {
                    byte[] lengthByte = new byte[8];
                    client.Receive(lengthByte, lengthByte.Length, SocketFlags.None);
                    long length = BitConverter.ToInt64(lengthByte, 0);

                    byte[] operationByte = new byte[4];
                    client.Receive(operationByte, operationByte.Length, SocketFlags.None);
                    int operation = BitConverter.ToInt32(operationByte, 0);

                    MemoryStream ms = new MemoryStream();
                    long count = 0;
                    byte[] data = new byte[4096];
                    while (count < length)
                    {
                        long left = length - count;
                        if (left > data.Length)
                        {
                            left = data.Length;
                        }
                        int i = client.Receive(data, (int)left, SocketFlags.None);
                        ms.Write(data, 0, i);
                        count += i;
                    }

                    data = ms.ToArray();
                    ReceiveMessage(client, operation, data);
                    ms.Close();
                }
                catch (Exception)
                {
                    client.Close();
                    Console.WriteLine("Client Connect Closed! Waiting for a new connection...");
                    client = _server.Accept();
                    Console.WriteLine("Accept Client");
                }
            }
        }

        private void ReceiveMessage(Socket client, int operation, byte[] data)
        {
            switch (operation)
            {
                //Cancel all requests
                case -1:
                    {
                        int i = 4096;
                        data = new byte[4096];
                        while (i >= 4096)
                        {
                            i = client.Receive(data, 4096, SocketFlags.None);
                        }
                        break;
                    }
                //GetDirectoryFileNames
                case 1:
                    {
                        string path = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Receive Message: Get Directory File Names at " + path);
                        List<string> fileNames = _fileAccessService.GetDirectoryFileNames(path);
                        string s = "";
                        fileNames.ForEach(n => s += n + ",");
                        //remove the last "," in s
                        if (!string.IsNullOrEmpty(s))
                        {
                            s = s.Remove(s.Length - 1);
                        }
                        byte[] sendData = Encoding.UTF8.GetBytes(s);
                        List<byte> message = new List<byte>();
                        message.AddRange(BitConverter.GetBytes((long)(sendData.Length)));
                        message.AddRange(BitConverter.GetBytes(1));
                        message.AddRange(sendData);
                        client.Send(message.ToArray());
                        //client.Close();
                        break;
                    }
                //GetDirectoryFolderNames
                case 2:
                    {
                        string path = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Receive Message: Get Directory Folder Names at " + path);
                        List<string> folderNames = _fileAccessService.GetDirectoryFolderNames(path);
                        string s = "";
                        folderNames.ForEach(n => s += n + ",");
                        //remove the last "," in s
                        if (!string.IsNullOrEmpty(s))
                        {
                            s = s.Remove(s.Length - 1);
                        }
                        byte[] sendData = Encoding.UTF8.GetBytes(s);
                        List<byte> message = new List<byte>();
                        message.AddRange(BitConverter.GetBytes((long)(sendData.Length)));
                        message.AddRange(BitConverter.GetBytes(2));
                        message.AddRange(sendData);
                        client.Send(message.ToArray());
                        //client.Close();
                        break;
                    }
                //GetAccessibleDirectories
                case 3:
                    {
                        break;
                    }
                //GetFile
                case 4:
                    {
                        string path = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Receive Message: Get File at " + path);
                        List<byte> message = new List<byte>();
                        message.AddRange(BitConverter.GetBytes(_fileAccessService.GetFileSize(path)));
                        message.AddRange(BitConverter.GetBytes(4));
                        client.Send(message.ToArray());
                        NetworkStream ns = new NetworkStream(client);
                        _fileAccessService.GetFile(path, ns, 64 * 1024);
                        ns.Close();
                        break;
                    }
                //GetFileThumbnail
                case 5:
                    {
                        string path = Encoding.UTF8.GetString(data);
                        Console.WriteLine("Receive Message: Get File Thumbnail at " + path);
                        byte[] file = _fileAccessService.GetFileThumbnail(path);
                        List<byte> message = new List<byte>();
                        message.AddRange(BitConverter.GetBytes((long)(file.Length)));
                        message.AddRange(BitConverter.GetBytes(5));
                        message.AddRange(file);
                        client.Send(message.ToArray());
                        break;
                    }

                default:
                    break;
            }
        }

        private void StartHTTPServer()
        {
            _httpServer = new HttpListener();
            // Add the prefixes.
            _httpServer.Prefixes.Add(string.Format("http://{0}:{1}/", GetLocalIp(), 8444));
            _httpServer.Start();
            while (true)
            {
                Console.WriteLine("HTTP Server Listening...");
                HttpListenerContext context = _httpServer.GetContext();
                HttpListenerRequest request = context.Request;
                string filePath = request.QueryString["filepath"];
                filePath = TextEncryptor.Decrypt_Hex(filePath);
                Console.WriteLine("HTTP Server Get Request: get file at " + filePath);
                HttpListenerResponse response = context.Response;
                //response.ContentType = "application/octet-stream";
                //response.ContentType = "video/mpeg4";
                //response.AddHeader("Content-Disposition", "inline;FileName=" + filePath.Split('\\').Last());
                response.ContentLength64 = _fileAccessService.GetFileSize(filePath);
                if (response.OutputStream.CanWrite)
                {
                    _fileAccessService.GetFile(filePath, response.OutputStream, 64 * 1024);
                }
                response.OutputStream.Close();
            }
        }

        private string GetLocalIp()
        {
            string ip = string.Empty;
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetwork")
                {
                    ip = address.ToString();
                }
            }
            return ip;
        }
        
        private void NATTraversalSignIn()
        {

        }

        private void NATTraversalSignOut()
        {

        }
    }
}
