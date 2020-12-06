using System;

namespace OctaviusTheDog.DataAccess.AzureCosmos
{
    public class AzureImage
    {
        public string _id { get; set; }

        public string OriginalFileName { get; set; }

        public string Title { get; set; }

        public DateTime UploadTime { get; set; }
    }
}
