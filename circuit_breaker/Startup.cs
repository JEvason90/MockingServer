using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using CircuitBreaker.Services;
using Polly;
using System.Net.Http;
using System;

[assembly: FunctionsStartup(typeof(circuit_breaker.Startup))]

namespace circuit_breaker
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddTransient<IFlakyService, FlakyService>();

            var basicCircuitBreakerPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(3));

            builder.Services.AddHttpClient("FlakyService", client =>
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(basicCircuitBreakerPolicy);

        }

        private void OnHalfOpen()
        {
            Console.WriteLine("Circuit in test mode, one request will be allowed.");
        }

        private void OnReset()
        {
            Console.WriteLine("Circuit closed, requests flow normally.");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Console.WriteLine("Circuit cut, requests will not flow.");
        }
    }
}