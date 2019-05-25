using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace SEV.Samples.DAL
{
    public class TestDbConfiguration : DbConfiguration
    {
        public TestDbConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}