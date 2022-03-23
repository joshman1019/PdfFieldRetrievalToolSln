using iText.Forms;
using iText.Kernel.Pdf;
using System;
using MongoDB.Bson; // MUST REMAIN HERE
using System.Collections.Generic;

namespace PdfFieldRetrieval
{
    class Program
    {
        private static int m_MenuBreak = -1; 
        static void Main(string[] args)
        {
            while (m_MenuBreak == -1)
            {
                Console.WriteLine("--- Main Menu ---"); 
                Console.WriteLine("Please make a selection");
                Console.WriteLine("1) View indexed forms");
                Console.WriteLine("2) Enter new form");
                Console.WriteLine("3) Delete Forms"); 
                Console.WriteLine("ESC) Exit System");
                var result = Console.ReadKey().Key;
                Console.WriteLine(); 
                switch (result)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        {
                            RetrieveSavedForms(); 
                            break; 
                        }
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        {
                            FormInput(); 
                            break; 
                        }
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        {
                            DeleteForms(); 
                            break; 
                        }
                    case ConsoleKey.Escape:
                        {
                            m_MenuBreak = 1; 
                            break; 
                        }
                    default:
                        break;
                }
            }
        }

        static void DeleteForms()
        {
            Console.WriteLine();
            Console.WriteLine("DELETE MENU");
            Console.WriteLine(); 
            List<PDFForm> forms;
            // Retrieve forms
            var realm = MongoFactory.GetApplicationDBRealm();
            forms = new List<PDFForm>(realm.All<PDFForm>());
            var enumerator = forms.GetEnumerator();
            int index = 0; 
            while (enumerator.MoveNext())
            {
                Console.WriteLine($"{index} - {enumerator.Current.FormName}"); 
                index++;
            }
            try
            {
                Console.WriteLine($"{index} - CANCEL DELETE");
                Console.WriteLine();
                Console.WriteLine("Please make a selection to delete/cancel"); 
                var entry = Convert.ToInt32(Console.ReadLine()); 
                if(entry > index || entry < 0)
                {
                    Console.WriteLine("INVALID SELECTION - RETURNING TO MAIN MENU"); 
                }
                if(entry == index)
                {
                    Console.WriteLine("CANCEL");
                    return; 
                }
                if(entry >= 0 && entry <= index)
                {
                    realm.Write(() =>
                    {
                        realm.Remove(forms[entry]);
                        Console.WriteLine("Entry Removed");
                    }); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        static void FormInput()
        {
            Console.WriteLine("Please enter form path");
            var path = Console.ReadLine();
            RetrieveFields(path);
            Console.WriteLine("Done. Please press ENTER to exit this application");
        }

        static void RetrieveSavedForms()
        {
            var realm = MongoFactory.GetApplicationDBRealm();
            var results = realm.All<PDFForm>();
            var finalizedResults = new List<PDFForm>(results);
            int iter = 0; 
            foreach (PDFForm item in finalizedResults)
            {
                Console.WriteLine($"Form Index: {iter}  --- Form Name: {item.FormName}");
                iter++; 
            }
            Console.WriteLine();
            Console.WriteLine("Please make a selection. Enter a form index to list the fields");
            int result;
            if(int.TryParse(Console.ReadLine(), out result))
            {
                if(result <= finalizedResults.Count - 1)
                {
                    Console.WriteLine("Would you like to place the field result within a preformatted line?");
                    Console.WriteLine("Please press Y or N"); 
                    var formLineResult = Console.ReadKey().Key;
                    string preformattedLine = string.Empty; 
                    if(formLineResult == ConsoleKey.Y)
                    {
                        Console.WriteLine("Enter your preformatted line. Field names should be represented by ^ and field values should be represented by !");
                        preformattedLine = Console.ReadLine(); 
                    }
                    Console.WriteLine(); 
                    var selectedForm = finalizedResults[result];
                    Console.WriteLine();
                    Console.WriteLine($"The selected form was at index {result}");
                    Console.WriteLine($"The form name: {selectedForm.FormName}");
                    Console.WriteLine("--- FIELDS ---");
                    Console.WriteLine();
                    foreach (PDFField fieldName in selectedForm.FieldNames)
                    {
                        if(formLineResult == ConsoleKey.Y)
                        {
                            Console.WriteLine(preformattedLine.Replace("^", "\"" + fieldName.FieldValue + "\"").Replace("!", "\"\""));
                        }
                        else
                        {
                            Console.WriteLine(fieldName.FieldValue); 
                        }
                    }
                    Console.WriteLine();
                    Console.WriteLine("***END*** - Press enter to continue"); 
                }
                else
                {
                    Console.WriteLine("Sorry, invalid selection"); 
                }
            }
            else
            {
                Console.WriteLine("Sorry, that was invalid. Please try again"); 
            }
        }

        static void RetrieveFields(string path)
        {
            try
            {
                Console.WriteLine("Would you like to store this form for retrieval later?");
                Console.WriteLine("Please enter Y or N");

                var result = Console.ReadKey().Key;
                Console.WriteLine(); 
                while (result != ConsoleKey.Y && result != ConsoleKey.N)
                {
                    Console.WriteLine("Sorry, that was an invalid selection. Please indicate whether or not you would like to save this form for retrieval later");
                    Console.WriteLine("Please enter Y or N");
                    result = Console.ReadKey().Key;
                    Console.WriteLine(); 
                }

                PdfDocument document = new PdfDocument(new PdfReader(path)); 
                PdfAcroForm form = PdfAcroForm.GetAcroForm(document, false);
                var fields = form.GetFormFields();
                PDFForm newForm = new PDFForm();
                List<string> fieldNames = new List<string>(); 
                if(result == ConsoleKey.Y)
                {
                    Console.WriteLine("Please enter the name of the form for storage");
                    newForm.FormName = Console.ReadLine();
                    Console.WriteLine(); 
                }
                foreach (string formField in fields.Keys)
                {
                    if(result == ConsoleKey.Y)
                    {
                        fieldNames.Add(formField);
                    }
                    Console.WriteLine(formField);
                }
                if(result == ConsoleKey.Y)
                {
                    var realm = MongoFactory.GetApplicationDBRealm();
                    using (var transaction = realm.BeginWrite())
                    {
                        try
                        {
                            realm.Add(newForm);
                            foreach (string fieldName in fieldNames)
                            {
                                newForm.FieldNames.Add(new PDFField() { FieldValue = fieldName, Form = newForm }); 
                            }
                            transaction.Commit();
                            Console.WriteLine("Form and form fields saved for future retrieval"); 
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message); 
                            transaction.Rollback(); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Please press enter to continue");
                Console.ReadLine(); 
            }
        }
    }
}
