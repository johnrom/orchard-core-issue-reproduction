
using System;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using YesSql;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

//
// Summary:
//     Represent a component capable of creating System.Data.Common.DbConnection instances.
public class DebuggingYesSqlConnectionFactory : IConnectionFactory
{
    private readonly IConnectionFactory _underlyingFactory;
    private readonly ILogger<DebuggingYesSqlConnectionFactory> _logger;

    public DebuggingYesSqlConnectionFactory(
        IConnectionFactory underlyingFactory,
        ILogger<DebuggingYesSqlConnectionFactory> logger
    ) {
        _underlyingFactory = underlyingFactory;
        _logger = logger;
    }

    //
    // Summary:
    //     Gets the type of the connection is can create.
    public Type DbConnectionType => _underlyingFactory.DbConnectionType;

    //
    // Summary:
    //     Creates a System.Data.Common.DbConnection instance.
    public DbConnection CreateConnection() {
        return _underlyingFactory.CreateConnection();
    }
}
