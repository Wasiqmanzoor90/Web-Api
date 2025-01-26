using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotenv.net;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;
        public MongoDbService()
        {
            try
            {
                DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            }
            catch (Exception)
            {

                throw new InvalidOperationException("Env failed");
            }

            var mongourl = Environment.GetEnvironmentVariable("MONGO_URI");
            var mongodb = Environment.GetEnvironmentVariable("MONGO_DB");

            if (string.IsNullOrEmpty(mongourl) || string.IsNullOrEmpty(mongodb))
            {
                throw new InvalidOperationException("CLOUDINARY_URL environment variable is not set.");
            }
            var client = new MongoClient(mongourl);
            _database = client.GetDatabase(mongodb);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Product");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

    }
}