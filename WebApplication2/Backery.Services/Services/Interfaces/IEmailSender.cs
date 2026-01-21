namespace WebApplication2.Backery.Services.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string email, string subject, string message);
    }
}
