using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule
{
    public class SqliteConfiguringStartup : StartupBase
    {
        public override int Order { get; } = -1000;

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSecondaryYesSqlStore();
            services.AddRawSqliteConfiguration();
        }
    }
}
