using System.Net.Http;

namespace OrchardCoreHttpClientTest.Web
{
    public class CmsHttpClientRequestingDependency
    {
        private readonly HttpClient httpClient;

        public CmsHttpClientRequestingDependency(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
