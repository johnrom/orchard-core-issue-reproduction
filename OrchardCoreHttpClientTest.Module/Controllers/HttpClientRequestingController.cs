using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OrchardCoreHttpClientTest.Module
{
    public class HttpClientRequestingController : Controller
    {
        private readonly PreviouslyCmsHttpClientRequestingDependency _previouslyCmsHttpClientRequestingDependency;
        private readonly ModuleHttpClientRequestingDependency _moduleHttpClientRequestingDependency;
        private readonly ILogger<HttpClientRequestingController> _logger;

        public HttpClientRequestingController(
            PreviouslyCmsHttpClientRequestingDependency previouslyCmsHttpClientRequestingDependency, 
            ModuleHttpClientRequestingDependency moduleHttpClientRequestingDependency,
            ILogger<HttpClientRequestingController> logger)
        {
            _previouslyCmsHttpClientRequestingDependency = previouslyCmsHttpClientRequestingDependency;
            _moduleHttpClientRequestingDependency = moduleHttpClientRequestingDependency;
            _logger = logger;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            _logger.LogInformation($"{_previouslyCmsHttpClientRequestingDependency.GetType().FullName}, {_moduleHttpClientRequestingDependency.GetType().FullName}");
            return new OkResult();
        }
    }
}