using System.Net.Http;
using System.Threading;
using Polly.CircuitBreaker;

namespace CircuitBreaker.Services
{
    public class FlakyService : IFlakyService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FlakyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public HttpResponseMessage GetMessage()
        {
            var httpMessage = new HttpResponseMessage();

            try{
                var httpClient = _httpClientFactory.CreateClient("FlakyService");
                httpMessage = httpClient.GetAsync("/").Result;
            }
            catch(BrokenCircuitException e)
            {
                Thread.Sleep(3500);
            }

            return httpMessage;

        }
    }
}