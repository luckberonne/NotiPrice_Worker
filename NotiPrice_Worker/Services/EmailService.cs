// Services/EmailService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using MailKit;

public class EmailService : IEmailService
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
        var smtpSettings = _configuration.GetSection("EmailSettings:Smtp");
        var smtpServer = smtpSettings["Server"];
        var smtpPort = int.Parse(smtpSettings["Port"]);
        var smtpEmail = smtpSettings["Email"];
        var smtpPassword = smtpSettings["Password"];

        try
        {
            using var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpEmail, smtpPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpEmail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add("luckberonne@gmail.com"); // Reemplaza con el destinatario

            await smtpClient.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Correo enviado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar el correo.");
        }
    }

    public async Task CheckForNewEmailsAsync(CancellationToken cancellationToken)
    {
        var imapSettings = _configuration.GetSection("EmailSettings:Imap");
        var imapServer = imapSettings["Server"];
        var imapPort = int.Parse(imapSettings["Port"]);
        var imapEmail = imapSettings["Email"];
        var imapPassword = imapSettings["Password"];

        try
        {
            using var client = new ImapClient();

            // Conectar al servidor IMAP
            await client.ConnectAsync(imapServer, imapPort, SecureSocketOptions.SslOnConnect, cancellationToken);

            // Autenticarse
            await client.AuthenticateAsync(imapEmail, imapPassword, cancellationToken);

            // Seleccionar la bandeja de entrada
            var inbox = client.Inbox;
            await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly, cancellationToken);

            _logger.LogInformation("Número de mensajes en la bandeja de entrada: {Count}", inbox.Count);

            // Recuperar los mensajes
            var messages = await inbox.FetchAsync(0, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, cancellationToken);

            foreach (var messageSummary in messages)
            {
                var message = await inbox.GetMessageAsync(messageSummary.UniqueId, cancellationToken);
                _logger.LogInformation("Asunto del correo: {Subject}", message.Subject);
                _logger.LogInformation("Cuerpo del correo: {Body}", message.TextBody);
            }

            // Desconectar del servidor
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer los correos electrónicos.");
        }
    }
}
