using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace OctaviusTheDog.DataAccess.SendGrid
{
    public interface IMailSender
    {
        Task SendAsync(string emailAddress);
    }

    public class MailSender : IMailSender
    {
        public MailSender(string apiKey)
        {
            _apiKey = apiKey;
        }
        
        public async Task SendAsync(string emailAddress)
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress("octavius@em5694.octaviusthedog.com", "Octavius");
            var subject = "New Pictures of Octavius";
            var to = new EmailAddress(emailAddress);
            var plainTextContent = "New pictures of Octavius have been uploaded. Head over to octaviusthedog.com";
            var htmlContent = CreateBody(emailAddress);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        private string CreateBody(string emailAddress)
        {
            string body = "";

            body += "<img src='https://octaviusthedog.blob.core.windows.net/pictures/modified_9c5aba3e-c239-429e-9506-7f5352eb3b8a'/>";
            body += "<h1>New Pictures of Octavius</h1>";
            body += "<h3>Check them out at <a href='https://octaviusthedog.com'>octaviusthedog.com</a></h3>";
            body += "<p>As a subscriber to Octavius The Dog, you are the first to know when new pictures are available</p>";
            body += $"<p><a href='https://octaviusthedog.com/home/unsubscribe?emailAddress={emailAddress}'>Unsubscribe</a>";

            return body;
        }

        private string _apiKey;
    }
}
