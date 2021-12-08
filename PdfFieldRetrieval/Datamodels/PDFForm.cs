using Realms;
using System;
using System.Collections.Generic;

namespace PdfFieldRetrieval
{
    public class PDFForm : RealmObject
    {
        [PrimaryKey]
        public string ID { get; set; } = Guid.NewGuid().ToString(); 

        [Required]
        public string FormName { get; set; }

        public IList<PDFField> FieldNames { get; }

    }
}
