using iText.Forms;
using iText.Kernel.Pdf;
using System;

namespace PdfFieldRetrieval
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter form path");
            var path = Console.ReadLine();
            RetrieveFields(path);
            Console.WriteLine("Done. Please press ENTER to exit this application");
            Console.ReadLine(); 

        }

        static void RetrieveFields(string path)
        {
            try
            {
                PdfDocument document = new PdfDocument(new PdfReader(path)); 
                PdfAcroForm form = PdfAcroForm.GetAcroForm(document, false);
                var fields = form.GetFormFields();
                foreach (string formField in fields.Keys)
                {
                    Console.WriteLine(formField);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                throw;
            }
        }
    }
}
