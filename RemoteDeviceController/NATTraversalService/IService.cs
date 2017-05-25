using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace NATTraversalService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IService
    {
        // TODO: Add your service operations here
        [OperationContract]
        int SignIn(string serverName, string serviceAddress);
        [OperationContract(IsOneWay = true)]
        void SignOut(int serverId);
        [OperationContract]
        List<string> RequestServerNames();
        [OperationContract]
        List<string> RequestServerAddresses();
    }
}
