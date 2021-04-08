using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Triggerless.Models;

namespace Triggerless.Services.Server
{
    public class BootstersDbService
    {
        public static async Task<TriggerlessPostResponse> PostSong(TriggerlessRadioSong post)
        {
            var response = new TriggerlessPostResponse();

            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "triggerless_radio_add_song";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@title", post.title);
                try
                {
                    var recordCount = await cmd.ExecuteNonQueryAsync();
                    response.status = "success; " + (recordCount == 1 ? "added" : "exists");

                }
                catch (Exception e)
                {
                    response.status = $"failed; {e.Message}";
                }
            }

            return response;
        }

        public static async Task<TriggerlessRadioSongs> GetSongs(double hours)
        {
            var response = new TriggerlessRadioSongs();

            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "triggerless_radio_get_songs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@hours", hours);
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var songs = new List<string>();
                        while (reader.Read())
                        {
                            songs.Add(reader.GetString(0));
                        }
                        response.titles = songs.ToArray();
                    }
                    
                    response.status = $"success; {response.titles.Length} song titles";

                }
                catch (Exception e)
                {
                    response.status = $"failed; {e.Message}";
                }
            }

            return response;

        }

        public static async void SaveRipInfo(int productId, string ipAddress, DateTime date)
        {
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"INSERT INTO rip_log (productId, ipAddress, date) VALUES ({productId}, '{ipAddress}', '{date:yyyy-MM-dd HH:mm:ss}')";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<IEnumerable<RipCountEntry>> IpAddresses ()
        {
            var result = new List<RipCountEntry>();
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"select ipAddress, count(ipAddress) as [Count] FROM [rip_log] where productId != 32678253 and ipAddress !='73.115.184.179' group by ipAddress order by [Count] desc";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync()) {
                        result.Add(new RipCountEntry
                        {
                            IpAddress = reader.GetString(0),
                            Count = reader.GetInt32(1)
                        });
                    }
                }
            }
            return result;
        }

        public static async Task<IEnumerable<RipEntry>> LogEntriesByIp(string ipAddress)
        {
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"select ipAddress, date, productId, id FROM [rip_log] where ipAddress = '{ipAddress}' order by date desc";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                var result = new List<RipEntry>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new RipEntry
                        {
                            IpAddress = reader.GetString(0),
                            Date = reader.GetDateTime(1),
                            ProductId = reader.GetInt64(2),
                            Id = reader.GetInt64(3)
                        });
                    }
                }
                return result;
            }
        }

    }
}
