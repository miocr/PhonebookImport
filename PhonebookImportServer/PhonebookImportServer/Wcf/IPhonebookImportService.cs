using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PhonebookImportServer.Business;

namespace PhonebookImportServer.Wcf
{
    [ServiceContract]
    public interface IPhonebookImportService
    {
        [OperationContract]
        string GetAppName();

        [OperationContract]
        bool ImportContacts();

        [OperationContract]
        bool AddContact(PhonebookWCF phonebookItem);

        [OperationContract]
        phonebook GetPhonebook(int id);
    }
}
