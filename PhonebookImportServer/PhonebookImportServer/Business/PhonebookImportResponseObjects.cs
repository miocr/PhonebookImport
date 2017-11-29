using System.Collections.Generic;

namespace PhonebookImportServer.Business
{
    public enum ImportErrorType
    {
        Required,
        DuplicateNumber,
        Unknown
    }

    /// <summary>
    /// Návratová třída v případě úspěšného importu položek
    /// </summary>
    public class ImportRecordsResponse
    {
        public int SuccessRecordsCount { get; set; }
    }

    /// <summary>
    /// Návratová třída v případě vyjímky importu položek (FaultContract)
    /// </summary>
    public class ImportRecordsResponseError
    {
        public int ErrorRecordsCount { get; set; }
        public int SuccessRecordsCount { get; set; }
        public string Description { get; set; }
        public List<ImportRecordResponseError> ImportErrors { get; set; }

        public ImportRecordsResponseError()
        {
            ImportErrors = new List<ImportRecordResponseError>();
        }
    }

    /// <summary>
    /// Návratová třída v případě úspěšného importu položky
    /// </summary>

    public class ImportRecordResponse
    {
        public bool Success { get; set; }
    }

    /// <summary>
    /// Návratová třída v případě vyjímky importu položky (FaultContract)
    /// </summary>
    public class ImportRecordResponseError
    {
        public int? RecordId { get; set; }
        public int? ColumnId { get; set; }
        public ImportErrorType? ErrorType { get; set; }
        public string Description { get; set; }
    }

}
