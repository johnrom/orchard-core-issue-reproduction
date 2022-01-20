using System.Net.Http;

namespace OrchardCoreHttpClientTest.Module
{
    public class PreviouslyCmsHttpClientRequestingDependency
    {
        private readonly HttpClient _httpClient;

        public PreviouslyCmsHttpClientRequestingDependency(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
