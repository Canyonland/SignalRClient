using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace SignalRClient
{
    public class SignalRClientService : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            StartListening().Wait();
        }

        private static async Task StartListening()
        {
            try
            {
                var hubConnection = new HubConnection("http://localhost:8080/");
                IHubProxy hubProxy = hubConnection.CreateHubProxy("SignalHub");
                hubProxy.On<string, string>("addMessage", (name, message) =>
                {
                    Log.Info(string.Format("Incoming data: {0} {1}", name, message));
                });
                ServicePointManager.DefaultConnectionLimit = 10;
                await hubConnection.Start();
                Log.Info("Connected");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
