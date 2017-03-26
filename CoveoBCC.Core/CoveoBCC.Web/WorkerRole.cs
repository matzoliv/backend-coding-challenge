using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Owin.Hosting;

namespace CoveoBCC.Web
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent( false );
        private IDisposable m_disposable;

        public override void Run ()
        {
            try
            {
                this.RunAsync( this.cancellationTokenSource.Token ).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart ()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[ "HTTP" ];
            string baseUri = String.Format( "{0}://{1}", endpoint.Protocol, endpoint.IPEndpoint );

            m_disposable = WebApp.Start<Startup>( new StartOptions( url: baseUri ) );

            bool result = base.OnStart();

            return result;
        }

        public override void OnStop ()
        {
            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            m_disposable?.Dispose();
        }

        private async Task RunAsync ( CancellationToken cancellationToken )
        {
            while ( !cancellationToken.IsCancellationRequested )
            {
                await Task.Delay( 120000 );
            }
        }
    }
}
