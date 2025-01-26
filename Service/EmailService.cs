using System;
using System.Net;
using System.Net.Mail;
using dotenv.net;
using WebApplication1.Interface;

namespace WebApplication1.Service
{
    public class EmailService : IMail
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;

        public EmailService()
        {
            // Load environment variables from .env file
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));

            // Read SMTP host
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST")
                ?? throw new InvalidOperationException("SMTP_HOST is not configured");


            // Read SMTP port and validate it
            var smtpPortEnv = Environment.GetEnvironmentVariable("SMTP_PORT");
            if (string.IsNullOrEmpty(smtpPortEnv) || !int.TryParse(smtpPortEnv, out _smtpPort))
            {
                throw new InvalidOperationException("SMTP_PORT is not configured or invalid.");
            }


            // Read SMTP user
            _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")
                ?? throw new InvalidOperationException("SMTP_USER is not configured");



            // Read SMTP password (don't log this value for security reasons)
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                ?? throw new InvalidOperationException("SMTP_PASSWORD is not configured");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
                EnableSsl = true // TLS enabled for port 587
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send mail.", ex);
            }
        }
    }
}
