using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Triggerless.Models;

namespace Triggerless.Services.Server
{
    public class BootstersDbClient
    {
        private ILog _log;

        public BootstersDbClient(ILog log = null)
        {
            _log = log;
        }

        public async Task<TriggerlessPostResponse> PostSong(TriggerlessRadioSong post)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(PostSong)} begin");
            _log?.Debug($"Post: djname = {post.djName}, title = {post.title}");
            var response = new TriggerlessPostResponse();

            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "bootsters.triggerless_radio_add_song";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@djname", post.djName);
                cmd.Parameters.AddWithValue("@title", post.title);
                try
                {
                    var recordCount = await cmd.ExecuteNonQueryAsync();
                    response.status = "success; " + (recordCount == 1 ? "added" : "exists");
                    _log?.Debug($"Success - Upsert worked, Records updated: {recordCount}");
                }
                catch (Exception e)
                {
                    _log?.Error("The following exception occurred.", e);
                    response.status = $"failed; {e.Message}";
                }
            }
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(PostSong)} finished");
            return response;
        }

        public async Task<TriggerbotLyricsEntry> GetLyrics(long productId)
        {
            _log?.Debug($"Get Lyrics: {productId}");
            var response = new TriggerbotLyricsEntry();
            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "[bootsters].[GetProductLyrics]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            // ProductId, Lyrics, ModifiedDate, Version
                            response = new TriggerbotLyricsEntry
                            {
                                Id = reader.GetInt64(0),
                                Lyrics = reader.GetString(1),
                                Modified = reader.GetDateTime(2),
                                Version = reader.GetInt32(3)
                            };
                        }
                        else
                        {
                            response.Id = 0;
                            response.Lyrics = "404 Not found";
                        }
                    }
                }
                catch (Exception exc)
                {

                    //burn it for now
                    response.Id = 0;
                    response.Lyrics = $"{exc.GetType().Name}: {exc.Message}";
                }
            }
            return response;
        }

        
        public async Task<TriggerlessRadioSongs> GetSongs(string djName, int count)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(GetSongs)} begin");
            _log?.Debug($"Post: djname = {djName}, count = {count}");

            var response = new TriggerlessRadioSongs();

            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "bootsters.triggerless_radio_get_songs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@djname", djName);
                cmd.Parameters.AddWithValue("@count", count);
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
                    _log.Debug($"Success - {response.titles.Length} songs returned");
                    response.status = $"success; {response.titles.Length} song titles";

                }
                catch (Exception e)
                {
                    _log.Error($"Failed - {e.Message}", e);
                    response.status = $"failed; {e.Message}";
                }
            }
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(GetSongs)} finish");

            return response;

        }

        public async void SaveRipInfo(int productId, string ipAddress, DateTime date)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(SaveRipInfo)} - productId = {productId}, ipAddress = {ipAddress}");
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"INSERT INTO bootsters.rip_log (productId, ipAddress, date) VALUES ({productId}, '{ipAddress}', '{date:yyyy-MM-dd HH:mm:ss}')";
                _log?.Debug($"SQL : {sql}");
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                var count = await cmd.ExecuteNonQueryAsync();
                _log?.Debug($"Success - {count} records");
            }
        }

        public async Task<IEnumerable<RipCountEntry>> RipLogSummary ()
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(RipLogSummary)}");
            var result = new List<RipCountEntry>();
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"select ipAddress, count(ipAddress) as [Count] FROM bootsters.rip_log where productId != 32678253 and ipAddress !='73.115.184.179' group by ipAddress order by [Count] desc";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync()) {
                        result.Add(new RipCountEntry
                        {
                            IpAddress = reader.GetString(0),
                            Count = reader.GetInt32(1),
                            URI = $"https://triggerless.com/api/riplog/ip/{reader.GetString(0)}/",
                            URIWithProduct = $"https://triggerless.com/api/riplog/ipx/{reader.GetString(0)}/"
                        });
                    }
                }
            }
            return result;
        }

        public async Task<IEnumerable<RipEntry>> RipLogEntriesByIp(string ipAddress)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(RipLogEntriesByIp)}");
            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"select ipAddress, date, productId, id FROM bootsters.rip_log where ipAddress = '{ipAddress}' order by date desc";
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
                            UtcDate = reader.GetDateTime(1),
                            ProductId = reader.GetInt64(2),
                            Id = reader.GetInt64(3)
                        });
                    }
                }
                return result;
            }
        }

        public async Task<IEnumerable<RipEntry>> RipLogEntriesByUtcDate(string dateString, int hours = 24)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(RipLogEntriesByUtcDate)}");
            if (!DateTime.TryParse(dateString, out var startDate)) return new List<RipEntry>();

            using (var cxn = await BootstersDbConnection.Get())
            {
                var sqlDate = startDate.ToString("yyyy-MM-dd HH:mm:ss");

                var sql =
                    $"select ipAddress, date, productId, id FROM bootsters.rip_log where date between '{sqlDate}' and DATEADD(hour, {hours}, '{sqlDate}') order by date asc";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                var result = new ConcurrentBag<RipEntry>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new RipEntry
                        {
                            IpAddress = reader.GetString(0),
                            UtcDate = reader.GetDateTime(1),
                            ProductId = reader.GetInt64(2),
                            Id = reader.GetInt64(3)
                        });
                    }
                }

                return result.Reverse();
            }
        }


        public async Task<IEnumerable<RipEntryExt>> RipLogEntriesByIpExt(string ipAddress)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(RipLogEntriesByIpExt)} IP: {ipAddress}");

            using (var cxn = await BootstersDbConnection.Get())
            {
                var sql =
                    $"select ipAddress, date, productId, id FROM bootsters.rip_log where ipAddress = '{ipAddress}' order by date desc";
                var cmd = cxn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                var result = new List<RipEntryExt>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        result.Add(new RipEntryExt
                        {
                            IpAddress = reader.GetString(0),
                            UtcDate = reader.GetDateTime(1),
                            ProductId = reader.GetInt64(2),
                            Id = reader.GetInt64(3)
                        });
                    }
                }

                using (var client = new ImvuApiClient())
                {
                    var productList = await client.GetProducts(result.Select(e => e.ProductId).Distinct());
                    foreach (var product in productList.Products)
                    {
                        var entries = result.Where(e => e.ProductId == product.Id);
                        foreach (var entry in entries)
                        {
                            entry.CreatorName = product.CreatorName;
                            entry.CreatorId = product.CreatorId;
                            entry.IsPurchaseable = product.IsPurchasable;
                            entry.IsVisible = product.IsVisible;
                            entry.ProductImage = product.ProductImage;
                            entry.ProductName = product.Name;
                            entry.ProductPage = product.ProductPage;
                            entry.Rating = product.Rating;
                            entry.RetailPrice = product.Price;
                            entry.Message = product.Message;
                        }
                    }

                }
                return result;
            }
        }

    }
}
