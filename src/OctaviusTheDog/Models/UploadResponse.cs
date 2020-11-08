namespace OctaviusTheDog.Models
{
    public class UploadResponse
    {
        public UploadResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; }

        public string Message { get; set; }
    }
}
