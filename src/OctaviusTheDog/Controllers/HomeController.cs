using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using OctaviusTheDog.Models;

namespace OctaviusTheDog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConnectionStringProvider connectionStringProvider;

        public HomeController(IConnectionStringProvider connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
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
        public async Task<IActionResult> GetPictures([FromQuery] int pageNumber)
        {
            string continuationToken = string.Empty;
            int? segmentSize = 10;

            const string ContainerName = "pictures";

            if (pageNumber < 1)
                pageNumber = 1;

            int currentPage = 0;
 
            try
            {
                var blobContainerClient = new BlobServiceClient(connectionStringProvider.GetConnectionString("StorageAccount"))
                    .GetBlobContainerClient(ContainerName);

                PicturesReponse reponse = new PicturesReponse(true);

                do
                {
                    currentPage++;
                    var blobs = blobContainerClient.GetBlobsAsync(prefix: "modified_").AsPages(continuationToken, segmentSize);

                    await foreach (Page<BlobItem> page in blobs)
                    {
                        if(pageNumber == currentPage)
                        {
                            foreach (BlobItem blobItem in page.Values)
                            {
                                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                                reponse.Pictures.Add(new PictureBlob() { BlobName = blobItem.Name, Url = blobClient.Uri.ToString() });
                            }
                        }
                        else
                        {
                            continuationToken = page.ContinuationToken;
                        }

                        currentPage++;
                    }

                } while (continuationToken != "" && currentPage < pageNumber);

                return Json(reponse);
            }
            catch(RequestFailedException ex)
            {
                return Json(new PicturesReponse(false) { Message = "An error occurred while getting pictures" });
            }
            catch(Exception)
            {
                return Json(new PicturesReponse(false) { Message = "An unknown error occurred while getting pictures" });
            }
        }
    }
}
