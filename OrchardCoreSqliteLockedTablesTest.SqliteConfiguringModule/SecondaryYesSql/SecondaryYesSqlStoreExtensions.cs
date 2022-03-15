using System.Data;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using YesSql;
using YesSql.Provider.Sqlite;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

public static class SecondaryYesSqlStoreExtensions
{
    public static void AddSecondaryYesSqlStore(this IServiceCollection services)
    {
        // Create a YesSql Wrapper around SQL Server per Site
        services.AddSingleton(sp =>
        {
            IConfiguration storeConfiguration = new YesSql.Configuration();

            var shellSettings = sp.GetService<ShellSettings>();
            var shellOptions = sp.GetService<IOptions<ShellOptions>>();
            var option = shellOptions.Value;
            var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, shellSettings.Name);
            var databaseFile = Path.Combine(databaseFolder, "yessql2.db");

            Directory.CreateDirectory(databaseFolder);
            
            storeConfiguration
                .UseSqLite($"Data Source={databaseFile};Cache=Shared;Pooling=False", IsolationLevel.ReadUncommitted)
                .UseDefaultIdGenerator();

            var store = StoreFactory.CreateAndInitializeAsync(storeConfiguration).GetAwaiter().GetResult();

            return new SecondaryYesSqlStoreWrapper(store);
        });
    }
}
