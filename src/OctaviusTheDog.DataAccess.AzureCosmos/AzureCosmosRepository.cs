﻿using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OctaviusTheDog.DataAccess.AzureCosmos
{
    public interface IAzureCosmosRepository
    {
        Task SaveAsync(AzureImage azureImage);

        Task<AzureImage> LoadByIdAsync(string id);

        Task<List<AzureImage>> LoadByPageAsync(int pageSize, int pageNumber);
    }

    public class AzureCosmosRepository : IAzureCosmosRepository
    {
        public AzureCosmosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<AzureImage>> LoadByPageAsync(int pageSize, int pageNumber)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("OctaviusTheDog");
            var collection = database.GetCollection<AzureImage>("AzureImages");

            var findResult = collection.Find(x => true);
            var sortResult = findResult.SortByDescending(x => x.UploadTime);
            var skipResult = sortResult.Skip(pageSize * (pageNumber - 1));
            var limitResult = skipResult.Limit(pageSize);
            var list = await limitResult.ToListAsync();

            return list;
        }

        public async Task<AzureImage> LoadByIdAsync(string id)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("OctaviusTheDog");
            var collection = database.GetCollection<AzureImage>("AzureImages");
            var cursor = await collection.FindAsync(x => x._id == id);

            var list = await cursor.ToListAsync();

            return list.SingleOrDefault();
        }

        public async Task SaveAsync(AzureImage azureImage)
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("OctaviusTheDog");
            var collection = database.GetCollection<AzureImage>("AzureImages");
            await collection.InsertOneAsync(azureImage);
        }

        private string _connectionString;
    }
}
