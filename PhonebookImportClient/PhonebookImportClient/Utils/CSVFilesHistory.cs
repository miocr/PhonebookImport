using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhonebookImportClient.Models;

namespace PhonebookImportClient.Utils
{
    /// <summary>
    /// Třída pro práci s historií CSV souborů
    /// Historie se ukládá do souboru 'csvfileshistory.cfg' (název lze změnít v app.config)
    /// v adresáří aplikace. Každý řádek obsahuje informace o CSV souboru, který byl již
    /// importován. Hodnoty jsou oddělený znakem ;
    /// nazev_souboru; hash; csv_has_header; code_page; name_column_index; number_column_index
    /// </summary>
    class CSVFilesHistory
    {
        #region Properties
        private string cfgFilePathName;
        public List<CSVFilesHistoryItem> Items { get; set; }
        #endregion

        #region Constructors
        public CSVFilesHistory()
        {
            cfgFilePathName = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location),
                String.IsNullOrEmpty(Properties.Settings.Default.ConfigFileName) ?
                    "csvfileshistory.cfg" : Properties.Settings.Default.ConfigFileName);
            ReadAllHistory();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Metoda vyhledá v historii záznam o CSV souboru podle jeho hash
        /// </summary>
        /// <param name="fileHash"></param>
        /// <returns>Objekt nebo null pokud není v historii</returns>
        public CSVFilesHistoryItem GetItem(string fileHash)
        {
            if (!String.IsNullOrEmpty(fileHash))
            {
                foreach (CSVFilesHistoryItem item in Items)
                {
                    if (item.HashCode == fileHash)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Metoda uloží do do historie záznam o CSV souboru 
        /// </summary>
        /// <param name="newHistoryItem">Nová záznam historie</param>
        public void SaveNewSettings(CSVFilesHistoryItem newHistoryItem)
        {
            StringBuilder newConfig = new StringBuilder("\n");
            newConfig.AppendFormat("{0};{1};{2};{3};{4};{5}",
                newHistoryItem.FileName,
                newHistoryItem.HashCode,
                newHistoryItem.HasHeader,
                newHistoryItem.EncodingCP,
                newHistoryItem.NameColumnIndex,
                newHistoryItem.NumberColumnIndex
                );

            File.AppendAllText(cfgFilePathName, newConfig.ToString());
            if (Items == null)
                Items = new List<CSVFilesHistoryItem>();
            Items.Add(newHistoryItem);
        }

        private void ReadAllHistory()
        {
            Items = new List<CSVFilesHistoryItem>();
            if (File.Exists(cfgFilePathName))
            {
                string[] cfgItems = File.ReadAllLines(cfgFilePathName);
                for (int i = 0; i < cfgItems.Length; i++)
                {
                    string[] cfgItem = cfgItems[i].Split(new char[] { ';' });
                    try
                    {
                        CSVFilesHistoryItem historyItem = new CSVFilesHistoryItem();
                        historyItem.FileName = cfgItem[0];
                        historyItem.HashCode = cfgItem[1];
                        historyItem.HasHeader = (cfgItem[2] == true.ToString());
                        historyItem.EncodingCP = int.Parse(cfgItem[3]);
                        historyItem.NameColumnIndex = int.Parse(cfgItem[4]);
                        historyItem.NumberColumnIndex = int.Parse(cfgItem[5]);
                        Items.Add(historyItem);
                    }
                    catch { }
                }
            }
        }

        #endregion
    }

    ///// <summary>
    ///// Třída jednoho CSV souboru v histori 
    ///// </summary>
    //public class CSVFilesHistoryItem
    //{
    //    public string FileName { get; set; }
    //    public string HashCode { get; set; }
    //    public bool HasHeader { get; set; }
    //    public int EncodingCP { get; set; }
    //    public int NameColumnIndex { get; set; }
    //    public int NumberColumnIndex { get; set; }
    //}

}

