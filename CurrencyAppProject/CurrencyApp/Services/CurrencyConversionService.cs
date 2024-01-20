namespace CurrencyApp.Services
{
    public class CurrencyConversionService : ICurrencyConversionService
    {
        private static readonly Dictionary<string, Currency> CurrencyMappings = new Dictionary<string, Currency>
        {
            { "USD", new Currency("USD", "United States Dollar") },
            { "EUR", new Currency("EUR", "Euro") },
            { "GBP", new Currency("GBP", "British Pounds") },
            { "JPY", new Currency("JPY", "Japanese Jen") },
            { "CHF", new Currency("CHF", "Swiss Franc") }
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CurrencyConversionService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<object> ConvertCurrency(string fromCurrencyCode, string toCurrencyCode, decimal amount)
        {
            try
            {
                string apiKey = _configuration["ApiKey"]; 
                string apiUrl = $"https://api.exchangerate-api.com/v4/latest/{fromCurrencyCode}";

                using (var client = _httpClientFactory.CreateClient())
                {
                    var response = await client.GetFromJsonAsync<ExchangeRateApiResponse>(apiUrl);

                    if (response != null && response.Rates.ContainsKey(toCurrencyCode))
                    {
                        decimal exchangeRate = response.Rates[toCurrencyCode];
                        decimal result = amount * exchangeRate;

                        return CreateResponseObject(fromCurrencyCode, toCurrencyCode, amount, exchangeRate, result);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private object CreateResponseObject(string fromCurrencyCode, string toCurrencyCode, decimal amount, decimal exchangeRate, decimal result)
        {
            Currency fromCurrency = CurrencyMappings.GetValueOrDefault(fromCurrencyCode, new Currency(fromCurrencyCode, "Unknown"));
            Currency toCurrency = CurrencyMappings.GetValueOrDefault(toCurrencyCode, new Currency(toCurrencyCode, "Unknown"));

            return new
            {
                FromCurrency = fromCurrency.Name,
                ToCurrency = toCurrency.Name,
                Amount = amount,
                ExchangeRate = exchangeRate,
                Result = result
            };
        }
        public class ExchangeRateApiResponse
        {
            public decimal? ConversionRate { get; set; }

            public Dictionary<string, decimal> Rates { get; set; }
        }
    }
}
