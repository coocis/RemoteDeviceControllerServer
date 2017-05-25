using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RemoteDeviceControllerClient.FileAccessServiceReference;

namespace RemoteDeviceControllerClient
{
    public class Client
    {
        public int Port { get; private set; }

        private UdpClient _udpClient;
        private IPEndPoint _udpEndpoint;
        private int _port;
        private FileAccessServiceClient _client;
        private bool _isConnected;

        public Client()
        {
            //_udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _port));
            //_udpEndpoint = new IPEndPoint(IPAddress.Any, 0);
            //_udpBoardcastListenThread = new Thread(ListenUDPBoardcastFromServer);
            //_udpBoardcastListenThread.Start();
        }

        ~Client()
        {
            //_udpClient.Close();
            //_udpBoardcastListenThread.Abort();
            if (_client != null)
            {
                _client.Close();
            }
        }

        public bool TryConnect()
        {

            DiscoveryClient discoveryClient =
                new DiscoveryClient(new UdpDiscoveryEndpoint());

            FindResponse response =
                discoveryClient.Find(new FindCriteria(typeof(IFileAccessService)));

            discoveryClient.Close();

            if (response.Endpoints.Count == 0)
            {
                Console.WriteLine("\nNo services are found.");
                return false;
            }
            else
            {
                FileAccessServiceClient client = new FileAccessServiceClient(new NetTcpBinding(SecurityMode.None),
                     response.Endpoints[0].Address);
                Console.WriteLine("Find!");
                Console.WriteLine(client.GetAccessibleDirectorys()[0]);
                foreach (var n in client.GetDirectoryFileNames(@"h:\"))
                {
                    Console.WriteLine(n);
                }
                foreach (var n in client.GetDirectoryFileNames(@"d:\"))
                {
                    Console.WriteLine(n);
                }
                return true;
            }
        }
    }
}
