using OctaviusTheDog.DataAccess.AzureStorage;
using System.Collections.Generic;

namespace OctaviusTheDog.Models
{
    public class PicturesReponse
    {
        public PicturesReponse(bool success)
        {
            Success = success;
            Pictures = new List<PictureBlob>();
        }

        public bool Success { get; set; }

        public List<PictureBlob> Pictures { get; set; }

        public string Message { get; internal set; }
    }
}
