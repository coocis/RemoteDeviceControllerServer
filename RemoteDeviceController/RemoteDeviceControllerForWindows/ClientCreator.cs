using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;
using RemoteDeviceControllerForWindows.FileAccessServiceReference;
using System.ServiceModel;
using System.Text.RegularExpressions;

namespace RemoteDeviceControllerForWindows
{
    class ClientCreator
    {
        public static string currentServerIP;
        public static FileAccessServiceClient fileAccessClient;

        public static List<string> SearchServers_FileAccess()
        {
            List<string> result = new List<string>();
            DiscoveryClient discoveryClient =
                new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindCriteria fc = new FindCriteria(typeof(IFileAccessService));
            fc.Duration = TimeSpan.FromSeconds(1);
            FindResponse response =
                discoveryClient.Find(fc);

            discoveryClient.Close();

            if (response.Endpoints.Count == 0)
            {
                return result;
            }
            else
            {
                foreach (var endpoint in response.Endpoints)
                {
                    NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
                    tcpBinding.MaxBufferSize = int.MaxValue;
                    tcpBinding.MaxReceivedMessageSize = int.MaxValue;
                    InstanceContext ic = new InstanceContext(new FileAccessServiceCallback());
                    FileAccessServiceClient client = new FileAccessServiceClient(
                        ic,
                        tcpBinding,
                         endpoint.Address);
                    result.Add(string.Format("{0} {1}", client.GetServerName(), endpoint.Address));
                    client.Close();
                }
                return result;
            }
        }

        public static FileAccessServiceClient ConnectServer_FileAccess(string address)
        {
            //get string "//255.255.255.255:"
            currentServerIP = Regex.Match(address, "//.*:").Value;
            //remove "//" and ":"
            currentServerIP = Regex.Match(currentServerIP, "[^/].*[^:]").Value;
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.MaxBufferSize = int.MaxValue;
            tcpBinding.MaxReceivedMessageSize = int.MaxValue;
            InstanceContext ic = new InstanceContext(new FileAccessServiceCallback());
            FileAccessServiceClient c = new FileAccessServiceClient(
                        ic,
                        tcpBinding,
                 new EndpointAddress(address));
            fileAccessClient = c;
            return c;
        }
    }
}
