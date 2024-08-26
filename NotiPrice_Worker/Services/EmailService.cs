using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NotiPrice_Worker.Models;

namespace NotiPrice_Worker.Services
{
    internal class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string subject, string message, CancellationToken cancellationToken)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

                var smtpClient = new SmtpClient(smtpSettings.Server)
                {
                    Port = smtpSettings.Port,
                    Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.SenderEmail, smtpSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add("luckberonne@gmail.com");

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo.");
            }
        }
    }
}