using System.Data;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrchardCore.Data;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Models;
using YesSql;
using YesSql.Indexes;
using YesSql.Provider.Sqlite;

namespace OrchardCoreSqliteLockedTablesTest.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOrchardCms()
                .ConfigureServices(ReplaceYesSqlSqliteStore, 1000);
        }
        
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseOrchardCore();
        }

        private void ReplaceYesSqlSqliteStore(IServiceCollection services)
        {
            // Replace YesSql Sqlite database with a non-pooling version.
            var existingStore = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IStore));

            if (existingStore != null)
            {
                services.Remove(existingStore);
                services.AddSingleton(sp =>
                {
                    var logger = sp.GetService<ILogger<Startup>>();
                    var shellSettings = sp.GetService<ShellSettings>();

                    // Before the setup a 'DatabaseProvider' may be configured without a required 'ConnectionString'.
                    if (shellSettings.State == TenantState.Uninitialized || shellSettings["DatabaseProvider"] != "Sqlite")
                    {
                        logger.LogError("Sqlite could not be initialized.");
                        return null;
                    }

                    IConfiguration storeConfiguration = new YesSql.Configuration();

                    var shellOptions = sp.GetService<IOptions<ShellOptions>>();
                    var option = shellOptions.Value;
                    var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, shellSettings.Name);
                    var databaseFile = Path.Combine(databaseFolder, "yessql.db");
                    Directory.CreateDirectory(databaseFolder);

                    storeConfiguration
                        .UseSqLite($"Data Source={databaseFile};Cache=Shared;Pooling=False", IsolationLevel.ReadUncommitted)
                        .UseDefaultIdGenerator();

                    if (!string.IsNullOrWhiteSpace(shellSettings["TablePrefix"]))
                    {
                        storeConfiguration = storeConfiguration.SetTablePrefix(shellSettings["TablePrefix"] + "_");
                    }

                    var store = StoreFactory.CreateAndInitializeAsync(storeConfiguration).GetAwaiter().GetResult();
                    var options = sp.GetService<IOptions<StoreCollectionOptions>>().Value;
                    foreach (var collection in options.Collections)
                    {
                        store.InitializeCollectionAsync(collection).GetAwaiter().GetResult();
                    }

                    var indexes = sp.GetServices<IIndexProvider>();

                    store.RegisterIndexes(indexes);

                    return store;
                });
            }
        }
    }
}
