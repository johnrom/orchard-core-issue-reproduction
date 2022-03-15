using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using YesSql;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule
{
    public class TestController : Controller
    {
        private readonly SecondaryYesSqlStoreWrapper _secondaryYesSqlWrapper;
        private readonly RawSqliteSettingsWrapper _rawSqliteSettingsWrapper;

        public TestController(
            SecondaryYesSqlStoreWrapper secondaryYesSqlWrapper,
            RawSqliteSettingsWrapper rawSqliteSettingsWrapper
        )
        {
            _secondaryYesSqlWrapper = secondaryYesSqlWrapper;
            _rawSqliteSettingsWrapper = rawSqliteSettingsWrapper;
        }

        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            //
            // YesSql. Locked and Undisposed.
            //
            var yesSqlViewModel = new TestViewModel
            {
                Name = "yessql.db and yessql2.db",
                Message = @"YesSql does not dispose unpooled connections, so Orchard's db (yessql.db) and 
                    the custom secondary db (yessql2.db) cannot be backed up or modified while the site is running after requesting this page.",

                NewModel = new TestModel
                {
                    Guid = new Guid()
                }
            };

            using (var yesSqlSession = _secondaryYesSqlWrapper.Store.CreateSession())
            {
            yesSqlViewModel.ExistingModels = await yesSqlSession.Query<TestModel>().ListAsync();

            foreach (var existingModel in yesSqlViewModel.ExistingModels)
            {
                yesSqlSession.Delete(existingModel);
            }

            yesSqlSession.Save(yesSqlViewModel.NewModel);

            await yesSqlSession.SaveChangesAsync();
            }

            //
            // Pooled Sqlite connection. Locked and Disposed (Connection is not Closed on Dispose).
            //
            var pooledViewModel = new TestViewModel
            {
                Name = "sqlite-pooled.db",
                Message = @"This connection is pooled, so even though the connection is Disposed, it is not Closed(). 
                    sqlite-pooled.db cannot be backed up / modified while the site is running.",

                NewModel = new TestModel
                {
                    Guid = new Guid()
                }
            };

            using (var pooledConnection = new SqliteConnection(_rawSqliteSettingsWrapper.PooledConnectionString))
            {
                await ApplyTestToConnectionAsync(pooledConnection, pooledViewModel);
            }

            //
            // Correctly disposed Sqlite connection.
            //
            var disposedUnpooledViewModel = new TestViewModel
            {
                Name = "sqlite-disposed.db",
                Message = @"This connection is disposed automatically, so 
                    sqlite-disposed.db can be backed up / modified while the site is running.",

                NewModel = new TestModel
                {
                    Guid = new Guid()
                }
            };

            using (var disposedUnpooledConnection = new SqliteConnection(_rawSqliteSettingsWrapper.DisposedConnectionString))
            {
                await ApplyTestToConnectionAsync(disposedUnpooledConnection, disposedUnpooledViewModel);
            }

            //
            // Incorrectly undisposed Sqlite connection.
            //
            var undisposedViewModel = new TestViewModel
            {
                Message = @"This connection is never disposed even though the Connection String disabled Pooling, 
                    which seems to be how OrchardCore and YesSql use Sqlite by default in .Net 6.0.
                    This file cannot be read / deleted while the site is running.",

                NewModel = new TestModel
                {
                    Guid = new Guid()
                }
            };

            var undisposedConnection = new SqliteConnection(_rawSqliteSettingsWrapper.UndisposedConnectionString);

            await ApplyTestToConnectionAsync(undisposedConnection, undisposedViewModel);

            return View("Test", new List<TestViewModel> {
                yesSqlViewModel,
                disposedUnpooledViewModel,
                pooledViewModel,
                undisposedViewModel
            });
        }

        private static string tableExistsCommandText = @"
            SELECT name 
            FROM sqlite_master 
            WHERE name = $name
        ";

        private static string createTableCommandText = @"
            CREATE TABLE [test] (
                [Id] INT not null, 
                [Guid] TEXT, 
                primary key ( [Id] ) 
            )
        ";

        private static string getExistingCommandText = @"
            SELECT Id,Guid
            FROM test
        ";

        private static string deleteAllCommandText = @"
            DELETE FROM test
        ";

        private static string insertTestCommandText = @"
            INSERT INTO test
            VALUES($id, $guid)
        ";

        private async Task ApplyTestToConnectionAsync(SqliteConnection connection, TestViewModel viewModel)
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync())
            using (var tableExistsCommand = connection.CreateCommand())
            using (var createTableCommand = connection.CreateCommand())
            using (var getExistingCommand = connection.CreateCommand())
            using (var deleteAllCommand = connection.CreateCommand())
            using (var insertTestCommand = connection.CreateCommand())
            {
                tableExistsCommand.CommandText = tableExistsCommandText;
                tableExistsCommand.Parameters.AddWithValue("$name", "test");

                var name = await tableExistsCommand.ExecuteScalarAsync();

                if (name == null)
                {
                    createTableCommand.CommandText = createTableCommandText;
                    await createTableCommand.ExecuteNonQueryAsync();
                }

                getExistingCommand.CommandText = getExistingCommandText;

                var maxId = 0;
                var existingModels = new List<TestModel>();

                using (var reader = await getExistingCommand.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var existingModel = new TestModel
                            {
                                Id = reader.GetInt32(0),
                                Guid = reader.GetGuid(1)
                            };

                            maxId = Math.Max(maxId, existingModel.Id);
                            existingModels.Add(existingModel);
                        }

                        deleteAllCommand.CommandText = deleteAllCommandText;
                        await deleteAllCommand.ExecuteNonQueryAsync();
                    }
                }

                viewModel.ExistingModels = existingModels;
                viewModel.NewModel.Id = maxId + 1;

                insertTestCommand.CommandText = insertTestCommandText;
                insertTestCommand.Parameters.AddWithValue("$id", viewModel.NewModel.Id);
                insertTestCommand.Parameters.AddWithValue("$guid", viewModel.NewModel.Guid);
                await insertTestCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
        }
    }
}
