using System;
using System.Windows;

using System.Resources;
using System.Globalization;

using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.ServiceModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PhonebookImportClient.Resource;
using PhonebookImportClient.Utils;
using PhonebookImportClient.PhobeboookImportService;
using PhonebookImportClient.Models;

namespace PhonebookImportClient.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Private Properties

        private PhonebookImportServiceClient service;

        private CSVFile csvFile;
        private CSVFilesHistory csvFilesHistory;
        private CSVFilesHistoryItem historyItem;

        private const char delimiter = ';';

        private bool _cSVHasHeader = false;
        private bool _cP1250 = true;
        private bool _cP852 = false;
        private DataTable _dataTable;
        private DataView _dataGridData;
        private ObservableCollection<bool> _tabControlItemEnabled =
            new ObservableCollection<bool>(new[] { true, false, false, false });
        private int _tabControlSelectedIndex = 0;
        private List<string> _assignComboColumns;
        private int _assignedNameColumnIndex = -1;
        private int _assignedNumberColumnIndex = -1;
        private string _responseMessage = String.Empty;
        private bool _columnsAssignmentValid = false;

/*
        // TODO do Resource
        private string[] _popisKroku = new[] {
//Popis kroku 1
@"Vítejte v průvodci importu telefonních kontaktů. 

Stisknětě tlačítko [Otevřít CSV soubor] a vyberte soubor pro import.",

//Popis kroku 2
@"Nastavte parametry CSV souboru. Pokud už byl tento soubor dřívě importován, parametry se automaticky nastaví podle nastavení při prvním importu. Jestliže soubor importujete poprvé a nevíte, co tyto parametry znamenají, ponechte nastavené výchozí hodnoty. 
Následně stiskněte tlačítko [Načíst data ze souboru].",

//Popis kroku 3
@"Pro import kontaktů je nutné určit, v jakém sloupci CSV souboru se nachází hodnoty Jméno a Telefon. Podívejte se na náhled dat a přiřaďte název sloupce, v kterém je Jméno a Telefon. Pokud už byl tento soubor dřívě importován, parametry se automaticky nastavení při prvním importu. 
Následně stiskněte tlačítko [Importovat data].",

//Popis kroku 4
@"Podívejte se na výsledek importu. Pokud nebylo možné některý záznam importovat, v seznamu je číslo řádku CSV souboru, na kterém se nachází tento záznam a důvod, proč se nepodařilo záznam importovat. Můžete se pokusit o opravu a import provést znovu. 
Tlačítkem [Ukončit aplikaci] ukončíte tento program."
        };
*/
        #endregion

        #region Public Properties

        public bool CSVHasHeader
        {
            get { return _cSVHasHeader; }
            set
            {
                _cSVHasHeader = value;
                NotifyPropertyChanged();
            }
        }
        public bool CP1250
        {
            get { return _cP1250; }
            set
            {
                _cP1250 = value;
                NotifyPropertyChanged();
            }
        }

        public bool CP852
        {
            get { return _cP852; }
            set
            {
                _cP852 = value;
                NotifyPropertyChanged();
            }
        }

        public string ResponseMessage
        {
            get { return _responseMessage; }
            set
            {
                _responseMessage = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<bool> TabControlItemEnabled
        {
            get { return _tabControlItemEnabled; }
            set
            {
                _tabControlItemEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public int TabControlSelectedIndex
        {
            get { return _tabControlSelectedIndex; }
            set
            {
                _tabControlSelectedIndex = value;
                NotifyPropertyChanged();
            }
        }

        public DataView DataGridData
        {
            get { return _dataGridData; }
            set
            {
                _dataGridData = value;
                NotifyPropertyChanged();
            }
        }

        public string Title
        {
            get { return "PhonebookImportClient"; }
        }

        /*
        public string[] PopisKroku
        {
            get { return _popisKroku; }
            set { _popisKroku = value; }
        }
        */
        public StringsResourceHelper PopisKrokuRes { get; } 
            = new StringsResourceHelper("PopisKroku");

        public List<string> AssignComboColumns
        {
            get { return _assignComboColumns; }
            set
            {
                _assignComboColumns = value;
                NotifyPropertyChanged();
            }
        }

        public int AssignedNameColumnIndex
        {
            get { return _assignedNameColumnIndex; }
            set
            {
                _assignedNameColumnIndex = value;
                ValidateColumnsAssignment(value, AssignedNumberColumnIndex);
                NotifyPropertyChanged();
            }
        }

        public int AssignedNumberColumnIndex
        {
            get { return _assignedNumberColumnIndex; }
            set
            {
                _assignedNumberColumnIndex = value;
                ValidateColumnsAssignment(AssignedNameColumnIndex, value);
                NotifyPropertyChanged();
            }
        }

        private string _assignedNameColumnText = String.Empty;
        public string AssignedNameColumnText
        {
            get { return _assignedNameColumnText; }
            set
            {
                _assignedNameColumnText = value;
                NotifyPropertyChanged();
            }
        }

        private string _assignedNumberColumnText = String.Empty;
        public string AssignedNumberColumnText
        {
            get { return _assignedNumberColumnText; }
            set
            {
                _assignedNumberColumnText = value;
                NotifyPropertyChanged();
            }
        }

        #endregion



        #region Constructors

        public MainViewModel()
        {
            //resManager = new ResourceManager("PhonebookImportClient.Resource.Res", typeof(MainViewModel).Assembly);
            //resHelper = new ResourceHelper(resManager);
            
            csvFile = new CSVFile();
            csvFilesHistory = new CSVFilesHistory();
            TabControlSetStep(1);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Výběr CSV souboru a jeho načtení do DataGrid
        /// </summary>
        private void OpenCSVFile()
        {
            if (csvFile.Open())
            {
                historyItem = csvFilesHistory.GetItem(csvFile.HashCode);

                if (historyItem == null)
                {
                    csvFile.HasHeader = CSVHasHeader;
                    csvFile.EncodingCP = CP1250 ? 1250 : 852;
                }
                else
                {
                    // Pro Read CSV
                    csvFile.HasHeader = historyItem.HasHeader;
                    csvFile.EncodingCP = historyItem.EncodingCP;

                    // Pro UI 
                    CSVHasHeader = historyItem.HasHeader;
                    CP1250 = (historyItem.EncodingCP == 1250);
                    CP852 = (historyItem.EncodingCP == 852);
                }

                csvFile.ReadyForRead = true;
                TabControlSetStep(2);
            }
        }

        private void ReadCSVFile()
        {
            csvFile.HasHeader = CSVHasHeader;
            csvFile.EncodingCP = (CP1250) ? 1250 : 852;
            _dataTable = csvFile.Read();

            if (_dataTable != null)
            {
                DataGridData = _dataTable.DefaultView;
                FillAssignCombos();

                //Pro UI Import CSV
                if (historyItem == null)
                {
                    AssignedNameColumnIndex = -1;
                    AssignedNumberColumnIndex = -1;
                }
                else
                {
                    AssignedNameColumnIndex = historyItem.NameColumnIndex;
                    AssignedNumberColumnIndex = historyItem.NumberColumnIndex;
                }
                TabControlSetStep(3);
            }
        }

        private void ImportData()
        {
            service = new PhonebookImportServiceClient();
            SaveSetting();
            ImportContacts();
            TabControlSetStep(4);
        }

        private void SaveSetting()
        {
            if (historyItem == null)
            {
                historyItem = new CSVFilesHistoryItem()
                {
                    FileName = Path.GetFileName(csvFile.FilePathName),
                    HashCode = csvFile.HashCode,
                    HasHeader = CSVHasHeader,
                    NameColumnIndex = AssignedNameColumnIndex,
                    NumberColumnIndex = AssignedNumberColumnIndex,
                    EncodingCP = CP1250 ? 1250 : 852
                };
                csvFilesHistory.SaveNewSettings(historyItem);
            }
            else
            {
                // TODO update 
                // Zjistit, zda v historii nejsou pro tento CSV jine predvolby
                // a pokud ano, tento zaznam aktualizovat
            }
        }

        private void FillAssignCombos()
        {
            if (AssignComboColumns == null)
            {
                AssignComboColumns = new List<string>();
            }
            else
            {
                AssignComboColumns.Clear();
                AssignedNameColumnText = String.Empty;
                AssignedNumberColumnText = String.Empty;
            }

            foreach (DataColumn col in _dataTable.Columns)
                AssignComboColumns.Add((string)col.ColumnName);
        }

        private bool ValidateColumnsAssignment(int nameAssignedIndex, int numberAssignedIndex)
        {
            bool result = false;
            if (nameAssignedIndex > -1 && numberAssignedIndex > -1)
            {
                if (nameAssignedIndex == numberAssignedIndex)
                    MessageBox.Show("Pro Jméno a Telefon nemůže být přiřazen stejný sloupec.\n" +
                        "Je potřeba vybrat jiný sloupec.", "Chyba přiřazení",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    result = true;
            }
            _columnsAssignmentValid = result;
            return result;
        }

        private void TabControlSetStep(int step)
        {
            if (step >= 1 && step <= 4)
            {
                for (int i = 0; i < TabControlItemEnabled.Count; i++)
                {
                    TabControlItemEnabled[i] = (i < (step));
                }
                TabControlSelectedIndex = step - 1;
            }
        }

        private void ImportContacts()
        {
            int rowNumber = 1;
            List<PhonebookRecord> records = new List<PhonebookRecord>();
            ImportRecordsResponse response = null;
            StringBuilder responseMessageSB = new StringBuilder();

            foreach (DataRow row in _dataTable.Rows)
            {
                PhonebookRecord record = new PhonebookRecord()
                {
                    RecordId = rowNumber++,
                    Company = (string)row[AssignComboColumns[_assignedNameColumnIndex]],
                    Number = (string)row[AssignComboColumns[_assignedNumberColumnIndex]]
                };
                records.Add(record);
            }

            try
            {
                response = service.ImportContacts(records.ToArray());
                responseMessageSB.AppendFormat("Počet úspěšně importovaných záznamů: {0}",
                    response.SuccessRecordsCount);
            }
            catch (FaultException<ImportRecordsResponseError> ex)
            {
                responseMessageSB.AppendFormat
                    ("Počet úspěšně importovaných záznamů: {0}\n\n", ex.Detail.SuccessRecordsCount);
                responseMessageSB.AppendFormat
                    ("Počet záznamů, které se nepodařilo importovat: {0}\n\n", ex.Detail.ErrorRecordsCount);
                responseMessageSB.Append("Seznam chyb v importním souboru::\n");
                foreach (ImportRecordResponseError importError in ex.Detail.ImportErrors)
                {
                    responseMessageSB.AppendFormat(
                        "Řádek: {0}\t\tSloupec: {1}\t\tChyba: {2}\n",
                        importError.RecordId.Value.ToString("00"),
                        importError.ColumnId.Value.ToString("00"),
                        importError.ErrorType.ToString());
                }
            }
            catch (CommunicationException ex)
            {
                responseMessageSB.AppendFormat("Komunikace se službou se bohužel nezdařila. \n\n{0}\n{1}",
                    //ex.Message, (ex.InnerException != null) ? ex.InnerException.Message : null);
                    ex.Message, ex.InnerException?.Message);
            }
            catch (DataException ex)
            {
                responseMessageSB.AppendFormat("Bohužel došlo k chybě služby při práci s databází. \n\n{0}\n{1}",
                     ex.Message, ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                responseMessageSB.AppendFormat("Bohužel došlo k chybě v programu. \n\n{0}\n{1}",
                     ex.Message, ex.InnerException?.Message);
            }
            finally
            {
                ResponseMessage = responseMessageSB.ToString();
            }
        }

        private void ExitApp()
        {
            Application.Current.MainWindow.Close();
        }

        #endregion

        #region Commands

        private bool AlwaysTrue() { return true; }
        private bool AlwaysFalse() { return false; }

        public ICommand OpenCSVFileCmd { get { return new RelayCommand(OpenCSVFile, AlwaysTrue); } }
        public ICommand ReadCSVFileCmd { get { return new RelayCommand(ReadCSVFile, ReadCSVFileCmd_CanExecute); } }
        public ICommand ImportDataCmd { get { return new RelayCommand(ImportData, ImportDataCmd_CanExecute); } }
        public ICommand ExitAppCmd { get { return new RelayCommand(ExitApp, AlwaysTrue); } }

        #region ExecuteGuards

        public bool ReadCSVFileCmd_CanExecute()
        {
            return csvFile.ReadyForRead;
        }

        public bool ImportDataCmd_CanExecute()
        {
            return _columnsAssignmentValid;
        }

        #endregion
        #endregion
    }
}
