using System.Collections.Generic;

namespace OrchardCoreSqliteLockedTablesTest.SqliteConfiguringModule;

public class TestViewModel
{
    public string Name { get; set; }
    public string Message { get; set; }
    public IEnumerable<TestModel> ExistingModels { get; set; } = new List<TestModel>();
    public TestModel NewModel { get; set; }
}
