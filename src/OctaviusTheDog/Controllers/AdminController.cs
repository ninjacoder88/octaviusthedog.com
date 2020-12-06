using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OctaviusTheDog.DataAccess.AzureCosmos;
using OctaviusTheDog.DataAccess.AzureStorage;
using OctaviusTheDog.Extensions;
using OctaviusTheDog.Models;
using OctaviusTheDog.Utility;

namespace OctaviusTheDog.Controllers
{
    public class AdminController : Controller
    {
        public AdminController(IWebHostEnvironment webHostEnvironment, IAzureCosmosRepository azureCosmosRepository, IAzureStorageRepository azureStorageRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _azureCosmosRepository = azureCosmosRepository;
            _azureStorageRepository = azureStorageRepository;
            _imageResizer = new ImageResizer();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files, string title)
        {
            try
            {
                foreach (IFormFile source in files)
                {
                    string fileName = source.GetFileName();
                    var fullFilePath = GetFullFilePath(fileName);

                    var imageId = Guid.NewGuid().ToString();

                    byte[] originalFile;
                    Image image;
                    using (var memoryStream = new MemoryStream())
                    {
                        await source.CopyToAsync(memoryStream);
                        originalFile = memoryStream.ToArray();
                        image = Image.FromStream(memoryStream);
                    }

                    var originalHeight = image.Height;
                    var originalWidth = image.Width;
                    const int MaxWidth = 250;

                    if(originalWidth > MaxWidth)
                    {
                        double scaledWidth = 250;

                        double scaleFactor = originalWidth / scaledWidth;
                        double scaledHeight = originalHeight / scaleFactor;

                        var bitmap = _imageResizer.ResizeImage(image, (int)scaledWidth, (int)scaledHeight);

                        byte[] scaledFile;
                        using (var memoryStream = new MemoryStream())
                        {
                            bitmap.Save(memoryStream, ImageFormat.Png);
                            scaledFile = memoryStream.ToArray();
                        }

                        await _azureStorageRepository.UploadImageAsync("pictures", $"modified_{imageId}", scaledFile);
                    }

                    await _azureStorageRepository.UploadImageAsync("pictures", $"original_{imageId}", originalFile);
                    await _azureCosmosRepository.SaveAsync(new AzureImage() {OriginalFileName = fileName, Title = title, UploadTime = DateTime.Now, _id = imageId });

                    System.IO.File.Delete(fullFilePath);
                }

                return new JsonResult(new UploadResponse(true));
            }
            catch(Exception e)
            {
                return new JsonResult(new UploadResponse(false) { Message = e.ToString() });
            }
        }

        private string GetFullFilePath(string filename)
        {
            return _webHostEnvironment.WebRootPath + "\\uploads\\" + filename;
        }

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IAzureCosmosRepository _azureCosmosRepository;
        private readonly IAzureStorageRepository _azureStorageRepository;
        private ImageResizer _imageResizer;
    }
}
