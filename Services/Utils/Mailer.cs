using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using IsekaiFantasyBE.Models.Users;

namespace IsekaiFantasyBE.Services.Utils
{
    public class Mailer
    {
        private IConfiguration _config;
        private IWebHostEnvironment _environment;

        public Mailer(IConfiguration config, IWebHostEnvironment environment)
        {
            _config = config;
            _environment = environment;
        }

        public async void SendEmailVerification(PreRegistrationUser user)
        {
            var templateContent = GetEmailTemplate("verify-email.html");

            var emailBody = templateContent
                .Replace("{{Name}}", user.Username)
                .Replace("{{VerificationLink}}", $"{_config["BaseUrls:AppBaseUrl"]}/verify?token={user.EmailValidationToken}");

            MailMessage email = GetEmail("Validação de Usuário - Isekai Fantasy RPG", emailBody);
            email.To.Add(user.Email);

            await SmtpClient().SendMailAsync(email);
        }

        public MailMessage GetEmail(string subject, string body)
        {
            return new MailMessage
            {
                From = new MailAddress(_config["Emails:SenderEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
            };
        }

        public string GetEmailTemplate(string path)
        {
            var emailTemplate = Path.Combine(
                _environment.ContentRootPath,
                _config["FilePaths:TemplatesFolder"],
                path
            );

            return File.ReadAllText(emailTemplate);
        }

        public SmtpClient SmtpClient()
        {
            return new SmtpClient()
            {
                Host = _config.GetValue<string>("Emails:Smtp:Server"),
                Port = _config.GetValue<int>("Emails:Smtp:Port"),
                EnableSsl = _config.GetValue<bool>("Emails:Smtp:UseSsl"),
                Credentials = new NetworkCredential()
                {
                    UserName = _config.GetValue<string>("Emails:Smtp:User"),
                    Password = _config.GetValue<string>("Emails:Smtp:Password")
                },
            };
        }
    }
}