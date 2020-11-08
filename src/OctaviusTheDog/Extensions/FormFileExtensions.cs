using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace OctaviusTheDog.Extensions
{
    public static class FormFileExtensions
    {
        public static string GetFileName(this IFormFile formFile)
        {
            string filename = ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName.Trim('"');

            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }
    }
}
