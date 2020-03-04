using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumAPI.Models;

namespace MuseumAPI.Services
{
	public class ImageProcessService : IImageProcessService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ImageProcessService(IWebHostEnvironment webHostEnvironment) => _webHostEnvironment = webHostEnvironment;
		
		public async Task<FileStreamResult> ProcessImage(ArtStyle artStyle, IFormFile file)
		{
			SaveImage(artStyle, file);
			var filePath = ProcessImage(artStyle);

			return null;
		}
		
		private void SaveImage(ArtStyle artStyle, IFormFile file)
		{
			// Wipe dir first
			var dirInfo = new DirectoryInfo(Path.Combine(_webHostEnvironment.WebRootPath, "Images"));
			foreach (var fileInfo in dirInfo.GetFiles())
			{
				fileInfo.Delete();
			}
			
			var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images",
				$"{artStyle.ToString()}.{DateTime.UtcNow.ToString("dd-MM-yyyy-HH:mm:ss")}.{file.FileName.Split('.').Last()}");
			using var fileStream = new FileStream(filePath, FileMode.Create);
			file.CopyTo(fileStream);
		}

		private string ProcessImage(ArtStyle artStyle)
		{
			return null;
		}
	}
}