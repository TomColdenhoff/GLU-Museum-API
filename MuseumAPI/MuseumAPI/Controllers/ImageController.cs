using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumAPI.Models;
using MuseumAPI.Services;

namespace MuseumAPI.Controllers
{
	[Route("api/[controller]")]
	public class ImageController : ControllerBase
	{
		private readonly IImageProcessService _imageProcessService;

		public ImageController(IImageProcessService imageProcessService) => _imageProcessService = imageProcessService;
			
		
		[HttpPost]
		[Route("process")]
		public async Task<IActionResult> Process([FromForm(Name = "artStyle")] ArtStyle artStyle, [FromForm(Name = "file")] IFormFile file)
		{
			if (file == null)
				return BadRequest();
			
			var image = await _imageProcessService.ProcessImage(artStyle, file);
			return Ok();
		}
	}
}