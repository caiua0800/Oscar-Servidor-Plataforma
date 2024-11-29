using MongoDB.Driver;
using Microsoft.Extensions.Options;
using DotnetBackend.Models;
using System.Threading.Tasks;
namespace DotnetBackend.Services;

    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDBSettings> options, IMongoClient client)
        {
            _database = client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

