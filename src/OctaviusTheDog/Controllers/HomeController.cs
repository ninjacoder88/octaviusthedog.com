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
            var model = new HomeIndexModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            try
            {
                var model = new Subscriber();
                model.EmailAddress = request.EmailAddress;
                model._id = Guid.NewGuid().ToString();

                if (string.IsNullOrEmpty(request.EmailAddress))
                    return Json("Email Address must have a value");

                if (!request.EmailAddress.Contains("@"))
                    return Json("Invalid email address");

                await _azureCosmosRepository.SaveAsync(model);
                return new JsonResult(true);
            }
            catch(Exception ex)
            {
                return Json(new PicturesReponse(false) { Message = $"An unknown error occurred while subscribing\r\n{ex}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPictures([FromQuery] int pageNumber)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            try
            {
                var azureImages = await _azureCosmosRepository.LoadByPageAsync(12, pageNumber);

                List<PictureBlob> pictures = new List<PictureBlob>();
                foreach (var azureImage in azureImages)
                {
                    var previewImageBlob = _azureStorageRepository.GetImage("pictures", $"modified_{azureImage._id}");
                    var originalImageBlob = _azureStorageRepository.GetImage("pictures", $"original_{azureImage._id}");

                    pictures.Add(new PictureBlob() { Title = azureImage.Title, PreviewUrl = previewImageBlob.Url, OriginalUrl = originalImageBlob.Url });
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
