using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDeviceControllerServer
{
    public class FileAccessServiceOptions
    {
        
        public List<string> accessibleDirectory = new List<string>();

        private string _serverName = "server";


        public string ServerName
        {
            get
            {
                return _serverName;
            }

            set
            {
                _serverName = value;
            }
        }
    }
}
