using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Triggerless.Models;

namespace Triggerless.Services.Client
{
    public class Store: IDisposable
    {
        protected string _folder;
        public const string DB_FILE_NAME = "imvuItems.litedb";
        protected LiteDatabase _litedb;

        public Store(string folder)
        {
            _folder = folder;
        }

        public Store(): this(DefaultFolder)
        {
        }

        public static string DefaultFolder
        {
            get
            {
                var specialFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var defaultFolder = Path.Combine(specialFolder, @"Triggerless");
                if (!Directory.Exists(defaultFolder)) Directory.CreateDirectory(defaultFolder);
                return defaultFolder;
            }
        }

        protected virtual string GetFileName()
        {
            return Path.Combine(_folder, DB_FILE_NAME);
        }

        public LiteDatabase DB
        {
            get
            {
                var dbName = GetFileName();
                if (!Directory.Exists(Path.GetDirectoryName(dbName)))
                    Directory.CreateDirectory(Path.GetDirectoryName(dbName));

                var connstr = $"Filename={dbName};Connection=shared";
                if (_litedb == null)
                {
                    _litedb = new LiteDatabase(connstr);
                }
                return _litedb;
            }
        }

        public async Task<ImvuUser> GetUser(long userId)
        {
            var coll = DB.GetCollection<ImvuUser>();
            var users = coll.Query().Where(u => u.Id == userId);
            var user = users.FirstOrDefault();
            if (user == null)
            {
                var client = new TriggerlessApiClient();
                user = await client.GetUser(userId);
                if (user != null)
                {
                    coll.Insert(user);
                    coll.EnsureIndex(u => u.Id);
                }
            }
            return user;
            
        }

        public void Dispose()
        {
            _litedb?.Dispose();
        }
    }
}
