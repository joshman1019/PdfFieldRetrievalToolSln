using Realms;
using System;
using System.IO;

namespace PdfFieldRetrieval
{
    public class PdfFieldRetrievalDBConfig : RealmConfiguration
    {
        private readonly string m_FileName = "PdfFieldRetrieval.realm";
        private readonly string m_AppPath = AppDomain.CurrentDomain.BaseDirectory; 
        public PdfFieldRetrievalDBConfig()
        {
            string databasePath = Path.Combine(m_AppPath, m_FileName);
            DatabasePath = databasePath;
            MigrationCallback = (migration, oldMigrationVer) =>
            {
                Console.WriteLine("Migration checklist ---"); 
                Console.WriteLine($"The previous migration version is: {oldMigrationVer}");

                // MIGRATIONS HERE IF NECESSARY
            }; 
        }
    }
}
