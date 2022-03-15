using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrchardCoreSqliteLockedTablesTest.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOrchardCms();
        }
        
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseOrchardCore();
        }
    }
}
