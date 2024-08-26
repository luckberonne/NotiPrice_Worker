public interface IEmailService
{
    Task SendEmailAsync(string subject, string message, CancellationToken cancellationToken);
    Task CheckForNewEmailsAsync(CancellationToken cancellationToken);
}
