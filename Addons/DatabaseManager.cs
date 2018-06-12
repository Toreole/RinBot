using System;
using Raven;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace RinBot
{
    public class DatabaseManager 
    {
        public IDocumentStore documentStore;
        public IDocumentSession session;

        public DatabaseManager(){
            //since the RavenDB is hosted locally, its using the local adress.
            documentStore = new DocumentStore(){
                Urls = new[] {"http://localhost:8080"}
            }.Initialize();

            session = documentStore.OpenSession(database: "Config");
        }

        public class TestClass
        {
            public string name = "ur mom";
            public string description = "gay lol";
            public int number = 42;
            public TestClass(string a, string b, int c)
            {
                name = a;
                description = b;
                number = c;
            }
        }

        public T Load<T>(string ID){
            return session.Load<T>(ID);
        }
    }
}