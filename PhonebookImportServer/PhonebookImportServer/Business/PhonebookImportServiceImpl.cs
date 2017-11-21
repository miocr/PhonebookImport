using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PhonebookImportServer.Wcf;
using PhonebookImportServer.Business;
using System.Data.SqlServerCe;

namespace PhonebookImportServer.Business
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class PhonebookImportServiceImpl : IPhonebookImportService
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Phonebook"].ConnectionString;
        private PhonebookEntities db = new PhonebookEntities();

        private PhonebookWCF PhonebookItemFromEntity(phonebook phonebookEntity)
        {
            PhonebookWCF phonebookItem = new PhonebookWCF()
            {
                Company = phonebookEntity.company,
                Number = phonebookEntity.number,
                PhoneType = phonebookEntity.phone_type,
                Description = phonebookEntity.description
                // todo next props
            };
            return phonebookItem;
        }

        private phonebook PhonebookItemToEntity(PhonebookWCF phonebookItem)
        {
            phonebook entityPhonebookItem = new phonebook()
            {
                company = phonebookItem.Company,
                number = phonebookItem.Number,
                phone_type= phonebookItem.PhoneType,
                description = phonebookItem.Description
                // todo next props
            };
            return entityPhonebookItem;
        }

        public string GetAppName()
        {
            return "PhonebookImportServer";
        }

        public phonebook GetPhonebook(int phonebookId)
        {
            //PhonebookEntities db = new PhonebookEntities();
            phonebook phonebookEntity = db.phonebook.SingleOrDefault<phonebook>(p => p.id == phonebookId);
            return phonebookEntity;
        }

        public bool AddContact(PhonebookWCF phonebookItem)
        {
            phonebook phonebookEntity = PhonebookItemToEntity(phonebookItem);
            db.phonebook.Add(phonebookEntity);
            db.SaveChanges();
            return true;
        }

        public bool ImportContacts()
        {



            //SqlCeEngine engine = new SqlCeEngine(connectionString);
            //engine.Repair(null, RepairOption.RecoverAllOrFail);

            var db = new PhonebookEntities();

            phonebook newPhonebookItem = new phonebook()
            {
                company = "Company",
                number = "123456789",
                description = "Description",
                phone_type = "MOBIL",
                vip = false,
                @public = true
            };

            //db.PhonebookItem.Add(newPhonebookItem);
            //db.SaveChanges();


            SqlCeConnection dbConnection = new SqlCeConnection(connectionString);
            dbConnection.Open();
            SqlCeCommand command = dbConnection.CreateCommand();
            command.CommandText = "SELECT * FROM phonebook";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }

            /*

            Phonebook newPhonebook = new Phonebook()
            {
                Company = "Company",
                Number = "123456789",
                Description = "Description",
                Phone_type = "MOBIL",
                Vip = false,
                Public = true
            };

            phoneBookContext dbContext = new phoneBookContext(connectionString);
            dbContext.Phonebook.InsertOnSubmit(newPhonebook);
            
            // Submit the change to the database.
            try
            {
                dbContext.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Make some adjustments.
                // ...
                // Try again.
                dbContext.SubmitChanges();
            }

            */

            return true;
        }
    }
}
