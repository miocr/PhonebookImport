using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using PhonebookImportServer.Business;
using PhonebookImportServer.Wcf;

namespace PhonebookImportServer
{
    class Program
    {
        static void SetLogging()
        {
            LoggingConfiguration cfg = new LoggingConfiguration();

            ColoredConsoleTarget target = new ColoredConsoleTarget();
            target.Layout = "${date:format=yyyy\\.MM\\.dd HH\\:mm\\:ss}  [${level:uppercase=true}] ${callsite}  =>  ${message}  <${exception:format=tostring}>";

            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            cfg.AddTarget("console", target);

            #region Logování do souboru
            /*
            FileTarget fileTarget = new FileTarget();
            fileTarget.FileName = "${basedir}/PhonebookImportService.log";
            fileTarget.Layout = "${date:format=yyyy\\.MM\\.dd HH\\:mm\\:ss}  [${level:uppercase=true}] ${callsite}  =>  ${message}  <${exception:format=tostring}>";
            cfg.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
            cfg.AddTarget("file", fileTarget);
            */
            #endregion

            try
            {
                LogManager.Configuration = cfg;
            }
            catch { }
        }

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            SetLogging();
            new Program().Run();
        }

        private void Run()
        {
            logger.Info("Application start");

            PhonebookImportServiceImpl sampleService = new PhonebookImportServiceImpl(logger);

            WcfServiceHost serviceHost = new WcfServiceHost();

            serviceHost.Open(sampleService);

            Console.ReadKey();

            serviceHost.Close();
        }

    }
}
