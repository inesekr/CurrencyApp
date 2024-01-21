using CurrencyApp.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CurrencyApp.Tests
{
    [TestClass]
    public class CurrencyConversionServiceTests
    {
        [TestMethod]
        public async Task ConvertCurrency_ShouldReturnCorrectResult()
        {
            // Arrange
            var response = new CurrencyConversionService.ExchangeRateApiResponse
            {
                Rates = new Dictionary<string, decimal>
                {
                    { "USD", 1.2m }, 
                    { "EUR", 1.0m },
                }
            };

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["ApiKey"]).Returns("TestApiKey");

            var httpClientFactory = new TestHttpClientFactory(new HttpResponseMessage
            {
                Content = new ObjectContent<CurrencyConversionService.ExchangeRateApiResponse>(response, new System.Net.Http.Formatting.JsonMediaTypeFormatter()),
                StatusCode = System.Net.HttpStatusCode.OK
            });

            var service = new CurrencyConversionService(httpClientFactory, configurationMock.Object);

            // Act
            var result = await service.ConvertCurrency("EUR", "USD", 100);

            // Assert
            var resultProperties = result.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(result));

            Assert.AreEqual(120.0m, resultProperties["Result"]); 
            Assert.AreEqual("Euro", resultProperties["FromCurrency"]);
            Assert.AreEqual("United States Dollar", resultProperties["ToCurrency"]);
        }
    }
    public class TestHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpResponseMessage _response;

        public TestHttpClientFactory(HttpResponseMessage response)
        {
            _response = response;
        }

        public HttpClient CreateClient(string name)
        {
            var handler = new MockHttpMessageHandler(_response);
            var httpClient = new HttpClient(handler);
            return httpClient;
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }
}
