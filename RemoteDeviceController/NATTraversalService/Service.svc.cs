using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace NATTraversalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service : IService
    {

        private int _idCount = 1;
        private List<ServerInfo> _onlineServer = new List<ServerInfo>();

        public List<string> RequestServerAddresses()
        {
            return (from s in _onlineServer
                    select s.ServiceAddress)
                   .ToList();
        }

        public List<string> RequestServerNames()
        {
            return (from s in _onlineServer
                    select s.Name)
                   .ToList();
        }

        public int SignIn(string serverName, string serviceAddress)
        {
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint =
                properties[RemoteEndpointMessageProperty.Name]
                as RemoteEndpointMessageProperty;

            ServerInfo si = new ServerInfo();
            si.ID = _idCount++;
            si.Name = serverName;
            si.ServiceAddress = endpoint.Address + ":" + endpoint.Port.ToString() + serviceAddress;
            _onlineServer.Add(si);
            return si.ID;
        }

        public void SignOut(int serverId)
        {
            _onlineServer.RemoveAll(server => server.ID == serverId);
        }
    }

    class ServerInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ServiceAddress { get; set; }
    }
}
