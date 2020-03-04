using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumAPI.Models;

namespace MuseumAPI.Services
{
	public interface IImageProcessService
	{
		Task<FileStreamResult> ProcessImage(ArtStyle artStyle, IFormFile file);
	}
}