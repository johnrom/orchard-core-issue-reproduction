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
            var store = serviceProvider.GetService<IStore>();
            var shellSettings = serviceProvider.GetService<ShellSettings>();

            // If we're not using Sqlite DatabaseProvider, don't worry about a thing.
            if (shellSettings.State == TenantState.Uninitialized || shellSettings["DatabaseProvider"] != "Sqlite")
            {
                return;
            }

            var shellOptions = serviceProvider.GetService<IOptions<ShellOptions>>().Value;
            var databaseFolder = Path.Combine(shellOptions.ShellsApplicationDataPath, shellOptions.ShellsContainerName, shellSettings.Name);
            var databaseFile = Path.Combine(databaseFolder, "yessql.db");
            var connectionString = $"Data Source={databaseFile};Cache=Shared;Pooling=False";

            store.Configuration.ConnectionFactory = new DebuggingYesSqlConnectionFactory(
                new DbConnectionFactory<SqliteConnection>(connectionString),
                serviceProvider.GetService<ILogger<DebuggingYesSqlConnectionFactory>>()
            );
        }
    }
}
