using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using MuseumAPI.Services.Interfaces;

namespace MuseumAPI.Services
{
	public class EmailService : IEmailService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public EmailService(IWebHostEnvironment webHostEnvironment) => _webHostEnvironment = webHostEnvironment;

		public void SendProcessedImageEmail(string emailAddress)
		{
			var smtpClient = CreateSmtpClient();
			var mailMessage = ComposeEmail(emailAddress);
			smtpClient.Send(mailMessage);
		}

		private static SmtpClient CreateSmtpClient()
		{
			return new SmtpClient()
			{
				Host = "smtp-mail.outlook.com",
				Port = 587,
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials =
					new NetworkCredential("tom.coldenhoff@outlook.com", "TomCol2013") // TODO Change to appsettings.json
			};
		}

		private MailMessage ComposeEmail(string emailAddress)
		{
			var mailMessage = new MailMessage
			{
				From = new MailAddress("tom.coldenhoff@outlook.com"),
				To = { emailAddress},
				Subject = "Your newly painted image",
				IsBodyHtml = true
			};

			var res = new LinkedResource(Path.Combine(_webHostEnvironment.WebRootPath, "Images", "processedImage.PNG"))
			{
				ContentId = Guid.NewGuid().ToString(),
				TransferEncoding = TransferEncoding.Base64
			};
			res.ContentType.MediaType = "image/png";
			
			var htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
			var alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
			alternateView.LinkedResources.Add(res);
			
			mailMessage.AlternateViews.Add(alternateView);

			return mailMessage;
		}
	}
}