using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PhonebookImportServer.Wcf
{
    [ServiceContract]
    public interface IPhonebookImportService
    {
        [OperationContract]
        string GetAppName();
    }
}
