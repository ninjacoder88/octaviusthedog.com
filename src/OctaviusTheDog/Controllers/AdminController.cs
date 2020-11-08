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
using OctaviusTheDog.Extensions;
using OctaviusTheDog.Models;
using OctaviusTheDog.Utility;

namespace OctaviusTheDog.Controllers
{
    public class AdminController : Controller
    {
        public AdminController(IWebHostEnvironment webHostEnvironment, IConnectionStringProvider connectionStringProvider)
        {
            _webHostEnvironment = webHostEnvironment;
            _connectionStringProvider = connectionStringProvider;
            _imageResizer = new ImageResizer();
            _blobServiceClient = new BlobServiceClient(_connectionStringProvider.GetConnectionString("StorageAccount"));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IList<IFormFile> files)
        {
            const string ContainerName = "pictures";

            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);

                foreach (IFormFile source in files)
                {
                    string fileName = source.GetFileName();
                    var fullFilePath = GetFullFilePath(fileName);

                    var dateString = DateTime.Now.ToString("yyyyMMddhhmmss");
                    var newFileName = fileName.Replace(" ", string.Empty).Replace("-", string.Empty).ToLower();

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

                        using (var stream = new MemoryStream(scaledFile))
                        {
                            await blobContainerClient.UploadBlobAsync($"modified_{dateString}_{newFileName}", stream);
                        }
                    }

                    using (var stream = new MemoryStream(originalFile))
                    {
                        await blobContainerClient.UploadBlobAsync($"original_{dateString}_{newFileName}", stream);
                    }

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

        private BlobServiceClient _blobServiceClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private ImageResizer _imageResizer;
    }
}
