using Realms;
using System;

namespace PdfFieldRetrieval
{
    public class PDFField : RealmObject
    {
        [PrimaryKey]
        public string FieldID => Guid.NewGuid().ToString(); 

        public PDFForm Form { get; set; }

        [Required]
        public string FieldValue { get; set; }
    }
}
