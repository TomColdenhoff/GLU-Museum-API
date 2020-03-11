using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MuseumAPI.Services.Interfaces;

namespace MuseumAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EMailController : ControllerBase
	{
		private readonly IEmailService _emailService;

		public EMailController(IEmailService emailService) => _emailService = emailService;
		
		
		[HttpPost]
		[Route("send")]
		public async Task<IActionResult> Send(string email)
		{
			var sendEmailTask = new Task(() => { _emailService.SendProcessedImageEmail(email); });
			sendEmailTask.Start();
			await sendEmailTask;
			
			return Ok();
		}
	}
}