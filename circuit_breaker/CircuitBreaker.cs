using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CircuitBreaker.Services;
using Polly.CircuitBreaker;
using System.Threading;

namespace circuit_breaker
{
    public class CircuitBreaker
    {
        private readonly IFlakyService _flakyService;

        public CircuitBreaker(IFlakyService flakyService)
        {
            _flakyService = flakyService;
        }

        [FunctionName("CircuitBreaker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var response = _flakyService.GetMessage();
            var content = response.Content.ReadAsStringAsync();
            return new OkObjectResult(content);

        }

    }
}
