namespace ReadOn.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string otp);
    }
}
