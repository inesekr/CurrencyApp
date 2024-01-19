using Microsoft.AspNetCore.Mvc;

namespace CurrencyApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        // Static mapping
        private static readonly Dictionary<string, Currency> CurrencyMappings = new Dictionary<string, Currency>
        {
            { "USD", new Currency("USD", "United States Dollar", "$") },
            { "EUR", new Currency("EUR", "Euro", "€") },
            { "GBP", new Currency("GBP", "British Pounds", "£") },
            { "JPY", new Currency("JPY", "Japanese Jen", "¥") },
            { "CHF", new Currency("CHF", "Swiss Franc", "CHF") }

        };
        private readonly IHttpClientFactory _httpClientFactory;

        public CurrencyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ConvertCurrency(string fromCurrencyCode, string toCurrencyCode, decimal amount)
        {
            try
            {
                Currency fromCurrency = CurrencyMappings.GetValueOrDefault(fromCurrencyCode, new Currency(fromCurrencyCode, "Unknown", ""));
                Currency toCurrency = CurrencyMappings.GetValueOrDefault(toCurrencyCode, new Currency(toCurrencyCode, "Unknown", ""));
                string apiKey = "5782c482331b0ac5766abdf4";
                string apiUrl = $"https://api.exchangerate-api.com/v4/latest/{fromCurrencyCode}";

                using (var client = _httpClientFactory.CreateClient())
                {
                    var response = await client.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl);

                    if (response != null && response.Rates.ContainsKey(toCurrencyCode))
                    {
                        decimal exchangeRate = response.Rates[toCurrencyCode];
                        decimal result = amount * exchangeRate;

                        return Ok(new
                        {
                            FromCurrency = fromCurrency.Name,
                            ToCurrency = toCurrency.Name,
                            Amount = amount,
                            ExchangeRate = exchangeRate,
                            Result = result
                            //FromCurrencySymbol = fromCurrency.Symbol
                        });
                    }
                }

                return BadRequest("Invalid from or to currency.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class ExchangeRateApiResponse
    {
        public decimal? ConversionRate { get; set; }
  
        public Dictionary<string, decimal> Rates { get; set; }
    }
}