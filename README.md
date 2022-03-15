# Orchard Core Issue Reproduction

## YesSql File Locking [#11387](https://github.com/OrchardCMS/OrchardCore/issues/11387)

Since updating to .Net 6.0, I have been unable to run a backup process on my Sqlite Dbs because Orchard Core and YesSql no longer close connections after accessing them.

Sqlite pools connections starting in `.Net 6.0`. However, when switching to Pooling it seems like Orchard's YesSql integration stopped supporting unpooled connections for Sqlite -- that is, it stopped disposing some connections altogether. When Sqlite in `.Net 6.0` is configured with pooling, disposing a connection may or may not `Close` the connection. In this reproduction, I tried to circumvent the Pooling by replacing the `YesSql.Configuration.ConnectionFactory` with one which does not pool. However, I think it's too late in the process and it's possible a Pooled connection has already been created and not closed at that time.

This repo has five yessql dbs which are configured slightly differently.

1. `yessql.db` Configured automatically by installing Orchard. I replaced the ConnectionFactory with an unpooled connection string (see `SqliteConfiguringStartup.Configure`), but it doesn't seem to work and the db cannot be backed up.
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
