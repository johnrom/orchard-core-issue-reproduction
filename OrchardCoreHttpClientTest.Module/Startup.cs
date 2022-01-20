using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCoreHttpClientTest.Module
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // this is fine
            services.AddHttpClient<CmsHttpClientRequestingDependency>();
            services.AddHttpClient<ModuleHttpClientRequestingDependency>();
        }
    }
}
