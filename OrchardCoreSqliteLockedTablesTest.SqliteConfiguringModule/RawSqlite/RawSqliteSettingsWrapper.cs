namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

public class RawSqliteSettingsWrapper
{
    public string DisposedConnectionString { get; set; }
    public string PooledConnectionString { get; set; }
    public string UndisposedConnectionString { get; set; }
}
