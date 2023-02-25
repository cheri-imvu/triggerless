using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    }
}
