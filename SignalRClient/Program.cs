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
    
    class Program
    {
        static void Main(string[] args)
        {
           // run as console application
           // SRClient srClient = new SRClient();

            //run as Windows service
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new SRClientService() };
            ServiceBase.Run(ServicesToRun);
        }

    }
}
