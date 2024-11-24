using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Monk_Task.Helpers
{
    public class DataContext
    {
        private DbSettings _dbSettings;
        public DataContext(IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        //public IDbConnection CreateConnection()
        //{
        //    var connectionString = $"Server={_dbSettings.Server}; Database={_dbSettings.Database}; User Id={_dbSettings.UserId}; Password={_dbSettings.Password};TrustServerCertificate=true;";
        //    return new SqlConnection(connectionString);
        //}
        public IDbConnection CreateConnection()
        {
            var connectionString1 = $"Data Source={_dbSettings.Server};Initial Catalog={_dbSettings.Database};Integrated Security=True; TrustServerCertificate=True";
            return new SqlConnection(connectionString1);
        }
    }
}
