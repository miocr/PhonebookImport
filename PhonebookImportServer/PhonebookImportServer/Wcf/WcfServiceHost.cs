using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace PhonebookImportServer.Wcf
{
    class WcfServiceHost
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private const int Port = 1987;
        private const string EndPointAddress = "SampleServer";

        private ServiceHost serviceHost;


        public void Open(IPhonebookImportService sampleService)
        {
            try
            {
                OpenService(sampleService);

            }
            catch (Exception e)
            {
                HandleException(e);
            }

        }

        private void OpenService(IPhonebookImportService sampleService)
        {
            logger.Info("Starting WCF service.");

            var host = CreateServiceHost(sampleService);
            InitBehavior(host);
            InitEndPoints(host);
            host.Open();
            serviceHost = host;
            CheckService();

            logger.Info("WCF service has started.");
        }

        private ServiceHost CreateServiceHost(IPhonebookImportService sampleService)
        {
            var uri = new Uri(string.Format("net.tcp://localhost:{0}", Port));
            return new ServiceHost(sampleService, uri);
        }

        private void InitBehavior(ServiceHost host)
        {
            InitMetadataBehavior(host);
            InitDebugBehavior(host);
        }

        private void InitMetadataBehavior(ServiceHost host)
        {
            ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (metadataBehavior == null)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(metadataBehavior);
            }
        }

        private void InitDebugBehavior(ServiceHost host)
        {
#if DEBUG
            ServiceDebugBehavior debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debug == null)
            {
                debug = new ServiceDebugBehavior();
                host.Description.Behaviors.Add(debug);
            }
            debug.IncludeExceptionDetailInFaults = true;
#endif
        }

        private void InitEndPoints(ServiceHost host)
        {
            InitServiceEndPoint(host);
            InitMexEndPoint(host);
        }

        private void InitServiceEndPoint(ServiceHost host)
        {
            host.AddServiceEndpoint(typeof(IPhonebookImportService), CreateBinding(), EndPointAddress);
        }

        private NetTcpBinding CreateBinding()
        {

            return new NetTcpBinding(SecurityMode.None)
            {
                ReaderQuotas = { MaxArrayLength = 2147483647 },
                MaxReceivedMessageSize = int.MaxValue,
                CloseTimeout = new TimeSpan(0, 1, 0),
                PortSharingEnabled = false
            };
        }

        private void InitMexEndPoint(ServiceHost host)
        {
            var endPoint = string.Format("{0}/mex", EndPointAddress);
            var mexBinding = MetadataExchangeBindings.CreateMexTcpBinding();
            host.AddServiceEndpoint(typeof(IMetadataExchange), mexBinding, endPoint);
        }

        private void CheckService()
        {
            serviceHost.ChannelDispatchers
                .OfType<ChannelDispatcher>()
                .SelectMany(channel => channel.Endpoints)
                .ToList()
                .ForEach(endPoint => logger.Info(string.Format("Listening on {0}", endPoint.EndpointAddress)));
        }

        private void HandleException(Exception exception)
        {
            logger.Info("Error during starting WCF service.", exception);
        }

        public void Close()
        {
            logger.Info("Stopping WCF service.");
            try
            {
                serviceHost.Close();
                logger.Info("WCF service has stopped.");
            }
            catch (Exception e)
            {
                logger.Error("Error during stopping WCF service.", e);
            }
        }

    }
}
