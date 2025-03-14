using System.Net.Mail;
using System.Net;

namespace ReadOn.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string otp)
        {
            try
            {
                var smtpHost = _config["MailSettings:SmtpHost"];
                var smtpPort = int.Parse(_config["MailSettings:SmtpPort"]);
                var smtpUser = _config["MailSettings:SmtpUser"];
                var smtpPass = _config["MailSettings:SmtpPass"];
                var fromEmail = _config["MailSettings:FromEmail"];

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUser, smtpPass),
                    EnableSsl = true
                };

                var subject = "Your OTP Code";
                var body = $"Your OTP code is: <b>{otp}</b>";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
