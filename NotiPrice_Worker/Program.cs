var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IWebScrapingService, WebScrapingService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
