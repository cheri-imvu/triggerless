using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
    }
}
