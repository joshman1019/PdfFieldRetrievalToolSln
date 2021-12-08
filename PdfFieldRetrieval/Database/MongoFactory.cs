using Realms;

namespace PdfFieldRetrieval
{
    public static class MongoFactory
    {
        public static Realm GetApplicationDBRealm()
        {
            return Realm.GetInstance(new PdfFieldRetrievalDBConfig()); 
        }
    }
}
