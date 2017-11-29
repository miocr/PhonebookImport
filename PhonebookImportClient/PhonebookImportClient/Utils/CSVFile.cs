using System;
using System.Data;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhonebookImportClient.Utils
{
    /// <summary>
    /// Třída pro práci s CSV souborem 
    /// </summary>
    class CSVFile
    {
        #region Private Properties

        private const int maxCSVColumns = 20;
        private char delimiter;

        #endregion

        #region Public Properties

        public string HashCode { get; set; }
        public bool HasHeader { get; set; } = false;
        public int EncodingCP { get; set; } = 1250;
        public string FilePathName { get; set; }
        public bool ReadyForRead { get; set; } = false;

        #endregion

        #region Constructors
        public CSVFile()
        {
            if (Properties.Settings.Default["Delimiter"] != null)
                delimiter = Properties.Settings.Default.Delimiter;
            else
                delimiter = ';';
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Metoda otevře soubor CSV a vytvoří jeho hash identifáktor
        /// </summary>
        /// <returns>False, pokud soubor nebyl otevřen</returns>
        public bool Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".csv";
            openFileDialog.Filter = "CSV soubory (.csv)|*.csv";
            openFileDialog.Title = "Výběr souboru pro import";
            bool? openResult = openFileDialog.ShowDialog();
            if (openResult.HasValue && openResult.Value)
            {
                FilePathName = openFileDialog.FileName;
                HashCode = CalculateFileHash();
            }
            return openResult.Value;
        }

        /// <summary>
        /// Metoda otevře soubor CSV a vytvoří jeho hash identifáktor
        /// </summary>
        /// <param name="csvFileNamePath">Název souboru vetně cesty</param>
        /// <returns>False, pokud soubor nebyl otevřen</returns>
        public bool Open(string csvFileNamePath)
        {
            if (!String.IsNullOrEmpty(csvFileNamePath))
            {
                FilePathName = csvFileNamePath;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Načte CSV soubor do DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable Read()
        {
            if (String.IsNullOrEmpty(FilePathName))
                return null;

            DataSet dataSet = new DataSet();
            dataSet.Tables.Add("Import");
            DataTable csvDataTable = dataSet.Tables["Import"];

            string[] csvLines = { };
            try
            {
                csvLines = File.ReadAllLines(FilePathName, Encoding.GetEncoding(EncodingCP));
            }
            catch (Exception)
            {
                MessageBox.Show("Soubor se nepodařilo otevřit.", "Chyba",
                      MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            int columnsCount = 0;
            string[] rowValues;
            DataRow dataRow = null;
            if (csvLines.Length > 0)
            {
                rowValues = csvLines[0].Split(delimiter);
                columnsCount = (rowValues.Length > maxCSVColumns) ? maxCSVColumns : rowValues.Length;
                if (columnsCount > 0)
                {
                    //Header
                    for (int i = 0; i < rowValues.Length; i++)
                    {
                        DataColumn col = new DataColumn();
                        col.MaxLength = 200;
                        col.DataType = typeof(string);
                        if (HasHeader)
                            col.ColumnName = rowValues[i];
                        else
                            col.ColumnName = String.Format("Sloupec {0}", i + 1);
                        csvDataTable.Columns.Add(col);
                    }

                    //Data
                    for (int row = HasHeader ? 1 : 0; row < csvLines.Length; row++)
                    {
                        dataRow = csvDataTable.NewRow();
                        rowValues = csvLines[row].Split(delimiter);
                        // jen validni radky (pocet sloupcu radku odpovida prvnimu radku)
                        if (rowValues.Length == columnsCount)
                        {
                            for (int col = 0; col < columnsCount; col++)
                                dataRow[col] = Convert.ToString(rowValues[col]);
                        }
                        csvDataTable.Rows.Add(dataRow);
                    }
                }
            }
            return csvDataTable;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Metoda vytvoří jednoduchý hash pro identifikaci CSV souboru 
        /// </summary>
        private string CalculateFileHash()
        {
            int? fhash = null;
            if (!String.IsNullOrEmpty(FilePathName))
            {
                FileInfo fi = new FileInfo(FilePathName);
                fhash = string.Concat(fi.Name, fi.LastWriteTime.ToUniversalTime(),
                    fi.CreationTime.ToUniversalTime(), fi.Length.ToString()).GetHashCode();
            }
            return (fhash.HasValue) ? fhash.Value.ToString() : null;
        }
        #endregion

    }
}
