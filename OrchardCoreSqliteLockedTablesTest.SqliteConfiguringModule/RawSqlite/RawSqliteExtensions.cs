using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

public static class RawSqliteExtensions
{
    public static void AddRawSqliteConfiguration(this IServiceCollection services)
    {
        // Create a YesSql Wrapper around SQL Server per Site
        services.AddSingleton(sp =>
        {
            var shellSettings = sp.GetService<ShellSettings>();
            var shellOptions = sp.GetService<IOptions<ShellOptions>>();
            var option = shellOptions.Value;
            var databaseFolder = Path.Combine(option.ShellsApplicationDataPath, option.ShellsContainerName, shellSettings.Name);
            
            Directory.CreateDirectory(databaseFolder);

            return new RawSqliteSettingsWrapper
            {
                DisposedConnectionString = new SqliteConnectionStringBuilder
                {
                    DataSource = Path.Combine(databaseFolder, "sqlite-disposed.db"),
                    Pooling = false,
                    Cache = SqliteCacheMode.Shared
                }.ToString(),
                PooledConnectionString = new SqliteConnectionStringBuilder
                {
                    DataSource = Path.Combine(databaseFolder, "sqlite-pooled.db"),
                    Pooling = true,
                    Cache = SqliteCacheMode.Shared
                }.ToString(),
                UndisposedConnectionString = new SqliteConnectionStringBuilder
                {
                    DataSource = Path.Combine(databaseFolder, "sqlite-undisposed.db"),
                    Pooling = false,
                    Cache = SqliteCacheMode.Shared
                }.ToString()
            };
        });
    }
}
