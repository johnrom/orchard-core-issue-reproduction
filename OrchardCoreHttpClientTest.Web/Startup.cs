using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OrchardCoreHttpClientTest.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // this is fine
            services.AddHttpClient<CmsHttpClientRequestingDependency>();
            services.AddOrchardCms();
        }
        
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseOrchardCore();
        }
    }
}
