using NotiPrice_Worker.Services;

namespace NotiPrice_Worker
{
    public class Worker : BackgroundService
    {
        private readonly IWebScrapingService _webScrapingService;
        private readonly IEmailService _emailService;
        private readonly ILogger<Worker> _logger;

        public Worker(IWebScrapingService webScrapingService, IEmailService emailService, ILogger<Worker> logger)
        {
            _webScrapingService = webScrapingService;
            _emailService = emailService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Ejecutando Worker para obtener precio y enviar email.");

                var price = await _webScrapingService.GetPriceAsync(stoppingToken);

                if (!string.IsNullOrEmpty(price))
                {
                    var subject = "Precio Actualizado";
                    var message = $"El precio actual es: {price}";

                    await _emailService.SendEmailAsync(subject, message, stoppingToken);
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}