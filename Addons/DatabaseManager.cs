using System;
using Raven;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

        public T Load<T>(string ID){
            return session.Load<T>(ID);
        }

        public GuildSettings GetGuildSettings(ulong guildID)
        {
            return session.Query<GuildSettings>().Where(g => g.guildID == guildID, true).First() as GuildSettings;
        }

        public List<GuildSettings> GetAllGuildSettings()
        {
            return session.Query<GuildSettings>().ToList();
        }

        public void SaveGuildSettings(GuildSettings settings)
        {
            session.Store(settings);
            session.SaveChanges();
        }

        public void OverwriteGuildSettings(GuildSettings old, GuildSettings _new)
        {
            Program.Rin.guildSettings.Remove(old);
            Program.Rin.guildSettings.Add(_new);
            session.Delete<GuildSettings>(old);
            session.Store(_new);
            session.SaveChanges();
        }
    }
}