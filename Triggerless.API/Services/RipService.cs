using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Triggerless.API.Services
{
    public class RipService
    {
        public static void SaveRipInfo(int productId, string ipAddress, DateTime date) {
            using (var cxn = DBSvc.GetConnection()) {
                cxn.Open();
                var sql =
                    $"INSERT INTO rip_log (productId, ipAddress, date) VALUES ({productId}, '{ipAddress}', '{date:yyyy-MM-dd HH:mm:ss}')";
                new SqlCommand(sql, cxn).ExecuteNonQuery();
            }
        }
    }
}