using System.Net.Http;

namespace OrchardCoreHttpClientTest.Module
{
    public class ModuleHttpClientRequestingDependency
    {
        private readonly HttpClient _httpClient;

        public ModuleHttpClientRequestingDependency(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
