using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using RemoteDeviceController.Util;
using RemoteDeviceControllerServer;
using RemoteDeviceControllerClient;
using System.Threading;
using RemoteDeviceControllerServer.Services;
using System.Net;
using System.Collections.Specialized;
using Open.Nat;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path1 = "a";
            string path2 = "b.cae";
            string path3 = "c.jpg";
            DateTime t = DateTime.Now;
            Console.WriteLine((DateTime.Now - t).Duration().TotalMilliseconds);
            Server server = new Server(new FileAccessServiceOptions() { accessibleDirectory = new List<string> { @"h:\" } });
            //Test();
            //WebClient wc = new WebClient();
            //NameValueCollection nv = new NameValueCollection();
            //nv.Add("ServerName", "server 2");
            //nv.Add("ServiceAddress", "net.tcp://localhost:8111/service");
            //StreamReader sr = new StreamReader(wc.OpenRead("http://coocis.net/NATTraversal/Create"));
            //Console.WriteLine(sr.ReadToEnd());
            //byte[] bs = wc.UploadValues("http://coocis.net/NATTraversal/Create", nv);
            //Console.WriteLine(Encoding.UTF8.GetString(bs));
            //FileAccessService fas = new FileAccessService(new FileAccessServiceOptions() { accessibleDirectory = new List<string> { @"h:\" } });
            //fas.GetFile(@"f:\a.jpg");
            //Image i = Image.FromStream(new MemoryStream(fas.GetFileThumbnail(@"f:\a.jpg")));
            //i.Save(@"f:\b.jpg");
            //i.Dispose();

            Console.ReadLine();
        }

        static async void Test()
        {
            var nat = new NatDiscoverer();

            // we don't want to discover forever, just 5 senconds or less
            var cts = new CancellationTokenSource(5000);

            // we are only interested in Upnp NATs because PMP protocol doesn't allow to list mappings
            var device = await nat.DiscoverDeviceAsync(PortMapper.Upnp, cts);

            foreach (var mapping in await device.GetAllMappingsAsync())
            {
                Console.WriteLine(mapping);
            }

            Console.WriteLine("donw");
        }
    }
}
