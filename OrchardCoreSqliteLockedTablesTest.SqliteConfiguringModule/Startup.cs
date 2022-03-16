using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Models;
using OrchardCore.Modules;
using YesSql;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule
{
    /// <summary>
    /// Override the Sqlite Connection Factory to use an unpooled connection.
    /// </summary>
    public class SqliteConfiguringStartup : StartupBase
    {
        // We want to override the Sqlite Connection Factory before any other module can use it.
        public override int Order { get; } = -1000;

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSecondaryYesSqlStore();
            services.AddRawSqliteConfiguration();
        }

        public override void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
        }
    }
}
