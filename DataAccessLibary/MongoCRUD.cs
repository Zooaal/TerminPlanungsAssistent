using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DataAccessLibary
{
    public class MongoCRUD
    {
        private IMongoDatabase db;
        // Verbindung zur Datenbank herstellen
        public MongoCRUD(string database)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            db = client.GetDatabase(database);
        }
        // Hinzufügen eines Datensatzes
        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
            
        }
        // Laden von einer ganzen Tabelle
        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }
        // Laden eines speziellen Users anhand der Email
        public T LoadRecordByEmail<T>(string table, string email)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Email", email);
            var result = collection.Find(filter).First();
            return result;
        }
        // Laden eines speziellen Users anhand des Username
        public T LoadRecordByUserName<T>(string table, string userName)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("UserName", userName);

            return collection.Find(filter).First();
        }
        // Laden eines speziellen Users anhand der Id
        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }
        // Updaten eines bestimmten Datensatzes anhand der Id
        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new ReplaceOptions { IsUpsert = true });
        }
        // Löschen eines Datensatzes anhand der Id
        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
