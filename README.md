# Orchard Core Issue Reproduction

## YesSql File Unlocked [#11387](https://github.com/OrchardCMS/OrchardCore/issues/11387)

By replacing the YesSql Store completely using a Connection String with `Pooling=False` from the start (see `OrchardCoreSqliteLockedTablesTest.Web.Startup.ReplaceYesSqlSqliteStore()`), I was able to remove the lock from the `yessql.db` file.

## Testing

- Pull this repo
- Open in VSCode
- Run launch task `.Net Core Launch (web)`
- Install Orchard
- Enable Module `OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule`.
- Navigate to `https://localhost:5001/test`.
- Without stopping the server, try to delete the Sqlite files under `OrchardCoreSqliteLockedTablesTest.Web/App_Data/Sites/Default`
- `yessql.db`, `yessql2.db` and `sqlite-disposed.db` can now be deleted.
