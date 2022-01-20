using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCoreHttpClientTest.Module
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // this is fine
            services.AddHttpClient<PreviouslyCmsHttpClientRequestingDependency>();
            services.AddHttpClient<ModuleHttpClientRequestingDependency>();
        }

        public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
            routes.MapControllers();
        }
    }
}
