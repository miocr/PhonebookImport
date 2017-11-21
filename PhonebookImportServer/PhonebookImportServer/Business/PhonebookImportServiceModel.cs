using System.Runtime.Serialization;
using PhonebookImportServer.Wcf;
using PhonebookImportServer.Business;

namespace PhonebookImportServer.Business
{
    [DataContract]
    public partial class PhonebookWCF
    {
        [DataMember]
        int SysCountryId;

        [DataMember]
        public string Number;

        [DataMember]
        public string Description;

        [DataMember]
        public string Company;

        [DataMember]
        public string PhoneType;

        [DataMember]
        bool Public;

        [DataMember]
        bool Vip;
    }
}
