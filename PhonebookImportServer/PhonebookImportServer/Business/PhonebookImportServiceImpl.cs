using NLog;
using PhonebookImportServer.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;

namespace PhonebookImportServer.Business
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class PhonebookImportServiceImpl : IPhonebookImportService
    {

        #region Private 

        private const string commonErrorMessage = "Při zpracování požadavku došlo k chybě. Další informace mohou být v 'Exception.Detail'";

        private Logger hostLogger;

        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Phonebook"].ConnectionString;

        private PhonebookEntities db = new PhonebookEntities();

        private List<string> numbers;

        private PhonebookRecord PhonebookEntityTpRecord(phonebook phonebookEntity)
        {
            PhonebookRecord phonebookItem = new PhonebookRecord()
            {
                Company = phonebookEntity.company,
                Number = phonebookEntity.number
                //TODO next properties
                //PhoneType = phonebookEntity.phone_type,
                //Description = phonebookEntity.description

            };
            return phonebookItem;
        }

        private phonebook PhonebookRecordToEntity(PhonebookRecord phonebookItem)
        {
            phonebook entityPhonebookItem = new phonebook()
            {
                company = phonebookItem.Company.Trim(),
                number = phonebookItem.Number.Trim()
                //TODO next properties
                //phone_type = phonebookItem.PhoneType,
                //description = phonebookItem.Description
            };
            return entityPhonebookItem;
        }

        private phonebook GetContact(int phonebookId)
        {
            //phonebook phonebookEntity = db.phonebook.Single<phonebook>(p => p.id == phonebookId);
            phonebook phonebookEntity = db.phonebook.Find(new int[] { phonebookId });
            return phonebookEntity;
        }

        private phonebook GetContactByPhoneNumber(string phoneNumber)
        {
            //phonebook phonebookEntity = db.phonebook.Single<phonebook>(p => p.number == phoneNumber);
            phonebook phonebookEntity = db.phonebook.Where(p => p.number == phoneNumber).First<phonebook>();
            return phonebookEntity;
        }

        private List<string> GetContactsPhoneNumbers()
        {
            Array numbersArray = db.phonebook.Select(p => p.number).ToArray<string>();
            List<string> numbers = numbersArray.OfType<string>().ToList();
            return numbers;
        }

        private ImportRecordResponse ImportContact(PhonebookRecord record, bool batchMode)
        {
            if (!batchMode)
                numbers = GetContactsPhoneNumbers();

            ImportRecordResponse response = new ImportRecordResponse();
            ImportRecordResponseError errorResponse = new ImportRecordResponseError();

            #region Validate
            if (String.IsNullOrEmpty(record.Company))
            {
                errorResponse.Description = "Jméno musí být uvedeno";
                errorResponse.RecordId = record.RecordId;
                errorResponse.ErrorType = ImportErrorType.Required;
                errorResponse.ColumnId = 0;
                hostLogger?.Error("Error: 'Name' required [recordId:" + record.RecordId + "]");
                throw new FaultException<ImportRecordResponseError>(errorResponse, commonErrorMessage);
            }

            if (String.IsNullOrEmpty(record.Number))
            {
                errorResponse.Description = "Tel.číslo musí být uvedeno";
                errorResponse.RecordId = record.RecordId;
                errorResponse.ErrorType = ImportErrorType.Required;
                errorResponse.ColumnId = 1;
                hostLogger?.Error("Error: 'Number' required [recordId:" + record.RecordId + "]");
                throw new FaultException<ImportRecordResponseError>(errorResponse, commonErrorMessage);
            }

            if (numbers != null && numbers.Count > 0)
            {
                // TODO iteligentnejsi kontrola, napr. i varianty zacinajici na '+narodnikod' atd
                // Dalsi moznost je nastaveni indexu bez duplicit na sloupci number v phonebook
                // a odchytit exception pri db.Save. Toto by bylo rychlejsi a jednodussi, ale nebylo
                // by mozne resit duplicity importu zapisu cisel v ruznem formatu...
                // Ale je mozne, ze toto je uz nejak reseno na urovni pripravy importniho CSV
                // protoze v ukazkach byla jen cisla bez mezinarodniho predcisli. Pak by bylo 
                // lepsi zvolit variantu hlidani duplicit na urovni SQL
                if (numbers.Contains(record.Number.Trim()))
                {
                    errorResponse.Description = "Tel.číslo již v seznamu existuje";
                    errorResponse.RecordId = record.RecordId;
                    errorResponse.ErrorType = ImportErrorType.DuplicateNumber;
                    errorResponse.ColumnId = 1; // TODO na string jmeno
                    hostLogger?.Error("Error: 'Number' duplicated [recordId:" + record.RecordId + "]");
                    throw new FaultException<ImportRecordResponseError>(errorResponse, commonErrorMessage);
                }
            }
            #endregion

            phonebook phonebookEntity = PhonebookRecordToEntity(record);
            try
            {
                db.phonebook.Add(phonebookEntity);
                if (!batchMode)
                    db.SaveChanges();
                response.Success = true;
            }
            catch (Exception e)
            {
                errorResponse.RecordId = record.RecordId;
                errorResponse.ErrorType = ImportErrorType.Unknown;
                errorResponse.Description = e.Message;
                throw new FaultException<ImportRecordResponseError>(errorResponse, commonErrorMessage);
            }
            return response;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Konstrukce WCF služby s logováním do logu hostitele
        /// </summary>
        public PhonebookImportServiceImpl(Logger logger)
        {
            hostLogger = logger;
        }

        /// <summary>
        /// Konstrukce WCF služby
        /// </summary>
        public PhonebookImportServiceImpl()
        {
            hostLogger = null;
        }

        #endregion

        #region Public

        /// <summary>
        /// Metoda vrátí identifikátor WCF služby
        /// </summary>
        public string GetAppName()
        {
            return "PhonebookImportServer";
        }

        /// <summary>
        /// Metoda uloží položku do databáze
        /// </summary>
        public ImportRecordResponse ImportContact(PhonebookRecord record)
        {
            return ImportContact(record, false);
        }

        /// <summary>
        /// Metoda uloží položky do databáze
        /// </summary>
        public ImportRecordsResponse ImportContacts(List<PhonebookRecord> records)
        {
            ImportRecordsResponse response = new ImportRecordsResponse();
            ImportRecordsResponseError errorResponse = new ImportRecordsResponseError();

            numbers = GetContactsPhoneNumbers();

            foreach (PhonebookRecord record in records)
            {
                ImportRecordResponse recordResponse;
                try
                {
                    recordResponse = ImportContact(record, true);
                    if (recordResponse.Success)
                    {
                        numbers.Add(record.Number);
                        response.SuccessRecordsCount++;
                    }
                }
                catch (FaultException<ImportRecordResponseError> e)
                {
                    errorResponse.ImportErrors.Add(e.Detail);
                    errorResponse.ErrorRecordsCount++;
                }
            }

            if (response.SuccessRecordsCount > 0)
            {
                try
                {
                    db.SaveChanges();
                }
                catch (DataException ex)
                {
                    throw new DataException(ex.Message, ex.InnerException);
                }
            }

            if (errorResponse.ImportErrors.Count > 0)
            {
                errorResponse.SuccessRecordsCount = response.SuccessRecordsCount;
                throw new FaultException<ImportRecordsResponseError>(errorResponse, commonErrorMessage);
            }

            return response;
        }

#endregion

    }
}
