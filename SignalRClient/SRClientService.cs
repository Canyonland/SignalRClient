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
using System.Configuration;
namespace SignalRClient
{
  
    public class SRClientService:ServiceBase
    {
        HubConnection hubCnn = new HubConnection(ConfigurationManager.AppSettings["SignalRURL"].ToString());
        IHubProxy hubProxy;
       
        protected override void OnStart(string[] args)
        {

#if DEBUG
            Thread.Sleep(20000);
#endif
            ConnectToSignalHub().Wait();

            RegisterPC(QueryComputerInfo()).Wait();


        }
        async Task ConnectToSignalHub()
        {
            hubProxy = hubCnn.CreateHubProxy("SignalHub");
            var context = SynchronizationContext.Current;

            //define SignalR client method that will be called from the hub
            hubProxy.On<string>("SaveMessage", msg =>
            {
                //save received message to temp file
                using (StreamWriter outfile = new StreamWriter(@"temp.txt", true))
                {
                    outfile.WriteLine(msg);
                }
            });
            await hubCnn.Start();
        }
        protected override void OnStop()
        {
            hubProxy.Invoke("UnRegisterPCClient").Wait();
        }
        async Task RegisterPC(ComputerInfo computer)
        {
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

        private void InitializeComponent()
        {
            // 
            // SRClientService
            // 
            this.ServiceName = "SRClientService";

        }

    }
}
