using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCoreHttpClientTest.Module
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // this raises a Null Reference Exception
            services.AddHttpClient<ModuleHttpClientRequestingDependency>();
        }
    }
}
