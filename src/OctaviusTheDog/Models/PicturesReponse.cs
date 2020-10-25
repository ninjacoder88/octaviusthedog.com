using System.Collections.Generic;

namespace OctaviusTheDog.Models
{
    public class PicturesReponse
    {
        public PicturesReponse(bool success)
        {
            Success = success;
            Names = new List<string>();
        }

        public bool Success { get; }

        public string Message { get; set; }

        public string BaseUrl { get; set; }

        public List<string> Names { get; set; }
    }
}
