// Services/WebScrapingService.cs
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

public class WebScrapingService : IWebScrapingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WebScrapingService> _logger;
    private const string Url = "https://www.mercadolibre.com.ar/adaptador-conversor-hdmi-14-a-rca-noga-full-hd-smart-tv/p/MLA25957329#polycard_client=recommendations_home_navigation-recommendations&reco_backend=machinalis-homes-univb-equivalent-offer&wid=MLA1479732918&reco_client=home_navigation-recommendations&reco_item_pos=2&reco_backend_type=function&reco_id=99cb60f6-2977-4b2f-be4b-8bdc9651cc81&sid=recos&c_id=/home/navigation-recommendations/element&c_uid=25283fa2-7dff-4480-99ea-8a7f9be8ef21"; // URL de la página web a scrapear

    public WebScrapingService(HttpClient httpClient, ILogger<WebScrapingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetPriceAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Realizar la solicitud GET para obtener el contenido HTML de la página
            var response = await _httpClient.GetAsync(Url, cancellationToken);
            response.EnsureSuccessStatusCode(); // Asegurarse de que la solicitud fue exitosa

            // Leer el contenido HTML de la respuesta
            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Cargar el contenido HTML en HtmlAgilityPack
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Buscar el elemento con la clase específica usando XPath
            var amountNode = htmlDocument.DocumentNode.SelectSingleNode("//span[@class='andes-money-amount__fraction' and @aria-hidden='true']");

            if (amountNode != null)
            {
                var price = amountNode.InnerText.Trim();
                _logger.LogInformation("Precio encontrado: {Price}", price);
                return price;
            }
            else
            {
                _logger.LogInformation("No se encontró el precio.");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar el scraping.");
            return null;
        }
    }
}
