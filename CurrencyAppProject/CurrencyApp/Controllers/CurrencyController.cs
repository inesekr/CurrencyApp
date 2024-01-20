using CurrencyApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConversionService _currencyConversionService;

        public CurrencyController(ICurrencyConversionService currencyConversionService)
        {
            _currencyConversionService = currencyConversionService;
        }

        [HttpGet]
        public async Task<IActionResult> ConvertCurrency(string fromCurrencyCode, string toCurrencyCode, decimal amount)
        {
            try
            {
                object result = await _currencyConversionService.ConvertCurrency(fromCurrencyCode, toCurrencyCode, amount);

                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Invalid from or to currency.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}