using System.Net.Http;

namespace OrchardCoreHttpClientTest.Module
{
    public class CmsHttpClientRequestingDependency
    {
        private readonly HttpClient _httpClient;

        public CmsHttpClientRequestingDependency(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
