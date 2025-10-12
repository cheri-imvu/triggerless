using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            if (String.IsNullOrWhiteSpace(post.djName)) post.djName = "Cheri";

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
                                Status = TriggerbotLyricsEntry.EntryStatus.Success,
                                Id = reader.GetInt64(0),
                                Lyrics = reader.GetString(1),
                                Modified = reader.GetDateTime(2),
                                Version = reader.GetInt32(3)
                            };
                        }
                        else
                        {
                            response.Id = productId;
                            response.Status = TriggerbotLyricsEntry.EntryStatus.NotFound;
                            response.Lyrics = "404 Not found";
                        }
                    }
                }
                catch (Exception exc)
                {

                    //burn it for now
                    response.Id = productId;
                    response.Status = TriggerbotLyricsEntry.EntryStatus.Error;
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

        public async Task<TriggerlessRadioSong> GetLastSong(string djName)
        {
            _log?.Debug($"{nameof(BootstersDbClient)}.{nameof(GetLastSong)} begin");
            _log?.Debug($"Post: djname = {djName}");

            var response = new TriggerlessRadioSong { djName = djName };

            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "bootsters.triggerless_radio_get_last_song";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@djname", djName);
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            response.title = reader.GetString(0);
                        } 
                        else
                        {
                            response.title = "Not Available";
                        }
                    }
                    _log.Debug($"Success - 1 song returned");

                }
                catch (Exception e)
                {
                    _log.Error($"Failed - {e.Message}", e);
                    response.title = $"failed; {e.Message}";
                }
                return response;
            }
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

        public async Task<TriggerbotLyricsEntry.EntryStatus> SaveLyrics(long productId, string lyrics)
        {
            var result = TriggerbotLyricsEntry.EntryStatus.Empty;
            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "[bootsters].[AddProductLyrics]";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Lyrics", lyrics);
                int version = 1;
                var pNewVersion = cmd.Parameters.Add("@NewVersion", SqlDbType.Int);
                pNewVersion.Direction = ParameterDirection.Output;

                try
                {
                    int sqlResult = await cmd.ExecuteNonQueryAsync();
                    version = (int)pNewVersion.Value;
                    result = (version > 0) ? 
                        TriggerbotLyricsEntry.EntryStatus.Success : 
                        TriggerbotLyricsEntry.EntryStatus.NotFound;
                }
                catch (Exception exc)
                {
                    result = TriggerbotLyricsEntry.EntryStatus.Error;
                    Debug.WriteLine(exc.ToString());
                    //burn it for now
                }
            }
            return result;
        }

        // ===== Triggerbot Events

        /*
         * 
INSERT INTO bootsters.triggerbot_event_list (EventId, Name) VALUES
(0, 'Unknown'), (1, 'AppInstall'), (2, 'AppUninstall'), (3, 'AppStart'), (4, 'AppCrash'), (5, 'AppCleanExit'),
(6, 'CutTune'), (7, 'PlayTune'), (8, 'LyricsSaved')
         * */
        public enum EventType : short
        {
            Empty = 0,
            AppInstall = 1,
            AppUninstall = 2,
            AppStart = 3,
            AppCrash = 4,
            AppCleanExit = 5,
            CutTune = 6,
            PlayTune = 7,
            LyricsSaved = 8,
            DiscordSent = 9,
        }

        public enum EventResultType : byte
        {
            Empty = 0,
            Success,
            Fail
        }

        public class EventResult
        {
            public EventResultType Type { get; set; }
            public string Message { get; set; }
        }

        public class Event
        {
            public long Id { get; set; } // autoincrement
            public DateTime Time { get; set; } = DateTime.UtcNow; // also defaults in DB
            public EventType Type { get; set; } // stored as short
            public long Cid { get; set; } // customer id
            public string JsonText { get; set; } // json text with details that can be deserialized later.

        }

        private string SqlTextOrNull(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "NULL";
            return "N'" + text.Replace("'", "''") + "'";
        }

        public async Task<EventResult> SaveEventAsync(EventType type, long cid, string jsonText)
        {
            var result = new EventResult();
            using (var conn = await BootstersDbConnection.Get())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO [bootsters].[triggerbot_event] " +
                    $"(EventId, Cid, JsonText) VALUES ({(short)type}, {cid}, {SqlTextOrNull(jsonText)});";

                try
                {
                    int i = cmd.ExecuteNonQuery();
                    result.Message = "Saved";
                    result.Type = EventResultType.Success;
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                    result.Type = EventResultType.Fail;
                }
            }

            return result;
        }


    }
}
