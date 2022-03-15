using System;
using YesSql;
namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

public class SecondaryYesSqlStoreWrapper : IDisposable
{
    public IStore Store { get; }

    public SecondaryYesSqlStoreWrapper(IStore store)
    {
        this.Store = store;
    }

    public void Dispose()
    {
        this.Store.Dispose();
    }
}
