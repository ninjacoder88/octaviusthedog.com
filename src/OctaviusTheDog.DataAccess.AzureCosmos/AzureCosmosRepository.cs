using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace OctaviusTheDog.DataAccess.AzureCosmos
{
    public interface IAzureCosmosRepository
    {
        Task SaveAsync(AzureImage azureImage);

        Task<AzureImage> LoadAsync(string id);
    }

    public class AzureCosmosRepository : IAzureCosmosRepository
    {
        public AzureCosmosRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<AzureImage> LoadAsync(string id)
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
