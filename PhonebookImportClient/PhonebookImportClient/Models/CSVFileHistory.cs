using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhonebookImportClient.Models
{
    /// <summary>
    /// Třída jednoho CSV souboru v histori 
    /// </summary>
    public class CSVFilesHistoryItem
    {
        public string FileName { get; set; }
        public string HashCode { get; set; }
        public bool HasHeader { get; set; }
        public int EncodingCP { get; set; }
        public int NameColumnIndex { get; set; }
        public int NumberColumnIndex { get; set; }
    }
}
