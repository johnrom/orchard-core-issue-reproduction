# Orchard Core Issue Reproduction

## YesSql File Locking [#11387](https://github.com/OrchardCMS/OrchardCore/issues/11387)

Since updating to .Net 6.0, I have been unable to run a backup process on my Sqlite Dbs because Orchard Core and YesSql no longer close connections after accessing them.

Sqlite pools connections starting in `.Net 6.0`. However, when switching to Pooling it seems like Orchard's YesSql integration stopped supporting unpooled connections for Sqlite -- that is, it stopped disposing some connections altogether. When Sqlite in `.Net 6.0` is configured with pooling, disposing a connection may or may not `Close` the connection. However, it should always be `Dispose`d so that `Microsoft.Data.Sqlite` can handle the connection lifecycle. `yessql2.db` works below, so I'm thinking there is a specific Orchard Core process which isn't disposing the connection -- but I haven't been able to pinpoint it.

This repo has five yessql dbs which are configured slightly differently.

1. `yessql.db` Configured automatically by installing Orchard. This does not support unpooled connections and the db cannot be backed up.
1. `yessql2.db` Configured in code through a secondary YesSql wrapper. See `AddSecondaryYesSqlDb()`. This supports unpooled connections and the db can be backed up.
1. `sqlite-disposed.db` Configured manually with a disposed `SqliteConnection`. See `AddRawSqliteConfiguration()`. This configuration supports unpooled connections and the db can be backed up.
1. `sqlite-pooled.db` Configured manually with a pooled `SqliteConnection`. See `AddRawSqliteConfiguration()`. This has pooling, so connections are reused and the db cannot be backed up.
1. `sqlite-undisposed.db` Configured manually with a pooled `SqliteConnection`. See `AddRawSqliteConfiguration()`. This intentionally never disposes connection and the db cannot be backed up. This is how Orchard Core + YesSql work when Pooling is set to False.

## Reproducing the issue

- Pull this repo
- Open in VSCode
- Run launch task `.Net Core Launch (web)`
- Install Orchard
- Enable Module `OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule`.
- Navigate to `https://localhost:5001/test`.
- Without stopping the server, try to delete the Sqlite files under `OrchardCoreSqliteLockedTablesTest.Web/App_Data/Sites/Default`
- Only `yessql2.db` and `sqlite-disposed.db` can be deleted.
