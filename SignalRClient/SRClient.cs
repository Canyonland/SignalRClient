using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    class ComputerInfo
    {
        public string Make { get; set; }
        public string Model { get; set; }
    }
    public class SRClient
    {
        HubConnection hubCnn = new HubConnection("http://localhost:6672/signalr");
        IHubProxy hubProxy;
        public SRClient()
        {
            RegisterPC(QueryComputerInfo()).Wait();
            Console.WriteLine("registered");
            string cmd = Console.ReadLine();

            if (cmd == "stop")
            {
                //hubCnn.Stop();
                hubProxy.Invoke("UnRegisterPCClient").Wait();

            }
        }
        async Task RegisterPC(ComputerInfo computer)
        {
            hubCnn = new HubConnection("http://localhost:6672/signalr");


            hubProxy = hubCnn.CreateHubProxy("SignalHub");
            var context = SynchronizationContext.Current;
            hubProxy.On<string>("SaveMessage", msg =>
            {
                using (StreamWriter outfile = new StreamWriter(@"temp.txt", true))
                {
                    outfile.WriteLine(msg);
                }
            });
            await hubCnn.Start();
            await hubProxy.Invoke("RegisterPCClient", computer.Make, computer.Model);

        }

        private static ComputerInfo QueryComputerInfo()
        {
            ComputerInfo computerInfo = new ComputerInfo();
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            //collection to store all management objects
            var moc = mc.GetInstances().GetEnumerator();
            if (moc.MoveNext())
            {
                var firstInstance = moc.Current;
                computerInfo.Make = firstInstance["Manufacturer"].ToString();
                computerInfo.Model = firstInstance["Model"].ToString();
            }
          
            return computerInfo;

        }

    }
}
