namespace MuseumAPI.Services.Interfaces
{
	public interface IEmailService
	{
		void SendProcessedImageEmail(string emailAddress);
	}
}