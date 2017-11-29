using System.Collections.Generic;
using System.ServiceModel;
using PhonebookImportServer.Business;
using System.Runtime.Serialization;


namespace PhonebookImportServer.Wcf
{
    /// <summary>
    /// Definice rozhraní WCF služby - metody
    /// </summary>
    [ServiceContract]
    public interface IPhonebookImportService
    {
        [OperationContract]
        string GetAppName();

        [OperationContract]
        [FaultContract(typeof(ImportRecordResponseError))]
        ImportRecordResponse ImportContact(PhonebookRecord record);

        [OperationContract]
        [FaultContract(typeof(ImportRecordsResponseError))]
        ImportRecordsResponse ImportContacts(List<PhonebookRecord> records);
    }

    /// <summary>
    /// Definice rozhraní WCF služby - objekty
    /// </summary>
    [DataContract]
    public partial class PhonebookRecord
    {
        [DataMember(IsRequired = true)]
        public int RecordId;

        [DataMember(IsRequired = true)]
        public string Company; // Name

        [DataMember(IsRequired = true)]
        public string Number;

        [DataMember]
        public string Description;

        [DataMember]
        public string PhoneType;

        [DataMember]
        public int SysPhoneYupeId;

        [DataMember]
        public bool Public;

        [DataMember]
        public bool Vip;

        [DataMember]
        public int SysCountryId;
    }

}
