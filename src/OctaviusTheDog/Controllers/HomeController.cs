using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OctaviusTheDog.Models;

namespace OctaviusTheDog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Pictures()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPictures()
        {
            string continuationToken = string.Empty;
            int? segmentSize = 10;

            const string ContainerName = "pictures";
            string storageAccountUrl = $"https://octaviusthedog.blob.core.windows.net/{ContainerName}/";
 
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("StorageAccount"));
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

                do
                {
                    var blobs = blobContainerClient.GetBlobsAsync().AsPages(continuationToken, segmentSize);

                    PicturesReponse picturesReponse = new PicturesReponse(true);
                    picturesReponse.BaseUrl = storageAccountUrl;

                    await foreach (Page<BlobItem> page in blobs)
                    {
                        foreach (BlobItem blobItem in page.Values)
                        {
                            picturesReponse.Names.Add(blobItem.Name);
                        }
                        continuationToken = page.ContinuationToken;
                    }

                    return Json(picturesReponse);

                } while (continuationToken != "");
            }
            catch(RequestFailedException ex)
            {
                return Json(new PicturesReponse(false) { Message = "An error occurred while getting pictures" });
            }
            catch
            {
                return Json(new PicturesReponse(false) { Message = "An unknown error occurred while getting pictures" });
            }
        }
    }
}
