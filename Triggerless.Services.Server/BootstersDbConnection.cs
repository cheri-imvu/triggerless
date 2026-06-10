using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;

namespace Triggerless.Services.Server
{
    public class BootstersDbConnection
    {

        public static async Task<SqlConnection> Get()
        {
            var connStr = ConfigurationManager.ConnectionStrings["bootsters_db"].ConnectionString;
            var result = new SqlConnection(connStr);
            await result.OpenAsync();
            return result;
        }

        public static SqlConnection GetSync()
        {
            var connStr = ConfigurationManager.ConnectionStrings["bootsters_db"].ConnectionString;
            var result = new SqlConnection(connStr);
            result.Open();
            return result;
        }
}
}
