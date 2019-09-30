using MongoDB.Driver;
using UnityEngine;
public class Mongo
{
    private const string MONGO_URI = "mongodb://localhost:27017";
    private const string DATABASE_NAME = "test";

    private MongoClient client;
    private MongoServer server;
    private MongoDatabase db;

    public void Init()
    {
        client = new MongoClient(MONGO_URI);
        server = client.GetServer();
        db = server.GetDatabase(DATABASE_NAME);

        Debug.Log("Database has been initiliazed");

    }
    public void Shutdown()
    {
        client = null;
        server.Shutdown();
        db = null;
    }


}