using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuseumAPI.Models;
using MuseumAPI.Services.Interfaces;

namespace MuseumAPI.Services
{
	public class ImageProcessService : IImageProcessService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ImageProcessService(IWebHostEnvironment webHostEnvironment) => _webHostEnvironment = webHostEnvironment;
		
		public async Task<FileStreamResult> ProcessImage(ArtStyle artStyle, IFormFile file)
		{
			var filePath = "";

			var processImageTask = new Task(() =>
			{
				filePath = SaveImage(artStyle, file);
				filePath = ProcessImage(artStyle, filePath);
			});

			processImageTask.Start();
			await processImageTask;

			return new FileStreamResult(File.Open(filePath, FileMode.Open), "image/PNG");
		}
		
		private string SaveImage(ArtStyle artStyle, IFormFile file)
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

			return filePath;
		}

		private string ProcessImage(ArtStyle artStyle, string filePath)
		{
			StartPythonScript(artStyle, filePath);

			var processedImagePath = GetProcessedImagePath();
			
			return processedImagePath;
		}

		private void StartPythonScript(ArtStyle artStyle, string filePath)
		{
			var pythonLocation =
				Path.Combine(_webHostEnvironment.WebRootPath, "Executables");
			
			try
			{
				using var myProcess = new Process
				{
					StartInfo =
					{
						WorkingDirectory = pythonLocation, UseShellExecute = false, FileName = $"python3",
						CreateNoWindow = true,
						Arguments =
							$"neural_style_transfer.py --image \"{filePath}\" --model {ResolveModelPath(artStyle)}"
					}

				};
				myProcess.Start();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private string GetProcessedImagePath()
		{
			var dirInfo = new DirectoryInfo(Path.Combine(_webHostEnvironment.WebRootPath, "Images"));

			while (dirInfo.GetFiles().Length == 1)
			{
				Thread.Sleep(100);
			}

			var filePath = dirInfo.GetFiles().FirstOrDefault(f => f.FullName.Contains("processedImage"))?.FullName;

			return filePath;
		}

		private string ResolveModelPath(ArtStyle artStyle)
		{
			return artStyle switch
			{
				ArtStyle.Composition => "models/eccv16/composition_vii.t7",
				ArtStyle.LaMuse => "models/instance_norm/la_muse.t7",
				ArtStyle.StarryNight => "models/instance_norm/starry_night.t7",
				ArtStyle.TheWave => "models/eccv16/the_wave.t7",
				ArtStyle.Candy => "models/instance_norm/candy.t7",
				ArtStyle.Feathers => "models/instance_norm/feathers.t7",
				ArtStyle.Mosaic => "models/instance_norm/mosaic.t7",
				ArtStyle.TheScream => "models/instance_norm/the_scream.t7",
				ArtStyle.Udnie => "models/instance_norm/udnie.t7",
				_ => null,
			};
		}
	}
}