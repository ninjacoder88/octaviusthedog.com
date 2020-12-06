using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OctaviusTheDog.DataAccess.AzureCosmos;
using OctaviusTheDog.DataAccess.AzureStorage;
using OctaviusTheDog.Models;

namespace OctaviusTheDog.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IAzureStorageRepository azureStorageRepository, IAzureCosmosRepository azureCosmosRepository)
        {
            _azureStorageRepository = azureStorageRepository;
            _azureCosmosRepository = azureCosmosRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPictures([FromQuery] int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            try
            {
                var images = await _azureStorageRepository.GetImages("pictures", "modified_", 12, pageNumber);

                List<PictureBlob> pictures = new List<PictureBlob>();
                foreach(var image in images)
                {
                    var imageId = image.BlobName.Replace("modified_", string.Empty);
                    var azureImage = await _azureCosmosRepository.LoadAsync(imageId);

                    if (azureImage == null)
                        continue;

                    pictures.Add(new PictureBlob() { Title = azureImage.Title, Url = image.Url });
                }

                return Json(new PicturesReponse(true) {Pictures = pictures });
            }
            catch(Exception ex)
            {
                return Json(new PicturesReponse(false) { Message = $"An unknown error occurred while getting pictures\r\n{ex}" });
            }
        }

        private readonly IAzureStorageRepository _azureStorageRepository;
        private readonly IAzureCosmosRepository _azureCosmosRepository;
    }
}
