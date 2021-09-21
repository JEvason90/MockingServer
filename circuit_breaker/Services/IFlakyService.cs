using System.Net.Http;

namespace CircuitBreaker.Services
{
    public interface IFlakyService
    {
        HttpResponseMessage GetMessage();
    }
}