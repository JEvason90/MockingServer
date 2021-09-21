using System.Net.Http;

namespace CircuitBreaker.Services
{
    public class FlakyService : IFlakyService
    {
        private readonly HttpClient _client;

        public FlakyService(HttpClient client)
        {
            _client = client;
        }

        public HttpResponseMessage GetMessage()
        {
            return _client.GetAsync("http://localhost:8080").Result;
        }
    }
}