using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Services.Server
{
    public class BootstersDbConnection
    {
        public static async Task<SqlConnection> Get()
        {
            var connStr = "Data Source=yew.arvixe.com; Initial Catalog=bootsters_db; User=bootsters; Password=$bootsters$";
            var result = new SqlConnection(connStr);
            await result.OpenAsync();


            return result;
        }
    }
}
