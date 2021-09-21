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
using System.Net.Http;
using System.Net;

namespace circuit_breaker
{
    public class CircuitBreaker
    {
        private readonly IFlakyService _flakyService;
        private readonly IBreaker _breaker;

        public CircuitBreaker(IFlakyService flakyService, IBreaker breaker)
        {
            _flakyService = flakyService;
            _breaker = breaker;
        }

        [FunctionName("CircuitBreaker")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            log.LogInformation($"Breaker Error Count: {_breaker.GetErrorCount()}");

            if(_breaker.GetCircuitState().IsPaused == true)
            {
                log.LogInformation("App is currently paused, please unpause to carry on execution");
                log.LogInformation($"Breaker Error Count: {_breaker.GetErrorCount()}");
                
                if(_breaker.GetErrorCount() > 2) //try to retry every ten times
                {
                    var response = DependentService(log);
                    _breaker.ResetErrorCount();
                    _breaker.UpdateCircuitState(false, "Circuit Reset");
                    return response;
                }
                else {
                    _breaker.AddErrorCount();
                    throw new Exception("App is Currently Paused");
                }
            }
            else{
                
                log.LogInformation($"Calling the Dependent Service from the ELSE clause");
               return DependentService(log);
            }
        }

        private IActionResult DependentService(ILogger log)
        {
            
            log.LogInformation($"Calling the Dependency");

            try{
                
                var response = _flakyService.GetMessage();
                var content = response.Content.ReadAsStringAsync(); 

                if(response.StatusCode >= HttpStatusCode.InternalServerError)
                {
                    log.LogInformation($"Error from Upstream Service");

                    _breaker.AddErrorCount();
                    _breaker.UpdateCircuitState(true, "Error from Upstream Service");
                    throw new Exception("Error from Upstream Service");
                }

                var messageFromService = new {content = content, appState = $"App Circuit Pause State: {_breaker.GetCircuitState().IsPaused}"};

                log.LogInformation($"App Circuit Pause State: {_breaker.GetCircuitState().IsPaused}");
                log.LogInformation($"App Error Count: {_breaker.GetErrorCount()}");
                
                return new OkObjectResult(messageFromService);

            }
            catch(Exception e)
            {
                log.LogInformation($"App Circuit Pause State: {_breaker.GetCircuitState().IsPaused}");
                log.LogInformation($"App Error Count: {_breaker.GetErrorCount()}");
                _breaker.AddErrorCount();
                _breaker.UpdateCircuitState(true, e.Message);

                throw e;
            }
        }
    }
}
