using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PhonebookImportServer.Wcf;

namespace PhonebookImportServer.Business
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class PhonebookImportServiceImpl : IPhonebookImportService 
    {
        public string GetAppName()
        {
            return "PhonebookImportServer";
        }
    }
}
