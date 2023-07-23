using LiteDB;
using System;
using Triggerless.Models;

namespace Triggerless.Services.Server
{
    internal class ProductDbService: IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<ImvuProduct> _coll;
        private const string DEFAULT_CONNECTION = @"";
        public ProductDbService(string connstr = DEFAULT_CONNECTION) { 
            _db = new LiteDatabase(connstr);
            _coll = _db.GetCollection<ImvuProduct>("ImvuProduct");
            _coll.EnsureIndex(x => x.Id);
        }

        public void Insert(ImvuProduct product)
        {
            _coll.Insert(product);
        }

        //public ImvuProduct Find(long id)
        //{

        //}

        public void Dispose()
        {
            if (_db != null) { _db.Dispose(); }
        }
    }
}
