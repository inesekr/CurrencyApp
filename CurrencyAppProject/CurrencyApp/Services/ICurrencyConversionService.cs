namespace CurrencyApp.Services
{
    public interface ICurrencyConversionService
    {
        Task<object> ConvertCurrency(string fromCurrencyCode, string toCurrencyCode, decimal amount);
    }
}