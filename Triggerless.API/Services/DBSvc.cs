using System.Configuration;
using System.Data.SqlClient;

namespace Triggerless.API.Services
{
    public class DBSvc
    {
        public static SqlConnection GetConnection() {
            var connstr = ConfigurationManager.ConnectionStrings["bootsters_db"].ConnectionString;
            return new SqlConnection(connstr);
        }
    }


}