using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.Services;
using Triggerless.Services.Client;

namespace Triggerless.Tests
{
    [TestFixture]
    public class StoreTests
    {
        private TestStore _store;

        [SetUp]
        public void SetUp() {
            _store = new TestStore();
            
        }

        [TearDown]
        public void TearDown() {
            if (_store != null)
            {
                var filename = _store.FileName;
                _store.Dispose();
                _store = null;
                if (File.Exists(filename)) File.Delete(filename);
            }

        }

        [Test]
        public void Init()
        {
            Assert.IsNotNull(_store);
        }

        [Test]
        public void AddAndTestDTO()
        {
            long id = new Random().Next();
            var dto = new StoreTestDTO
            {
                Id = id,
                Name = "Some String",
                PossibleMoney = 1.2M,
                Date = DateTime.Now,
                Choices = new[] {"First", "Second", "Third"}
            };

            var returnDto = _store.InsertDTO(dto);


        }


    }

    public class TestStore: Store
    {
        public const string TEST_DIR = @"D:\Temp\Triggerless";
        protected override string GetFileName()
        {
            return Path.Combine(TEST_DIR, "StoreTest.litedb");
        }

        public TestStore(): base(TEST_DIR)
        {

        }

        public string FileName => GetFileName();

        public StoreTestDTO InsertDTO(StoreTestDTO dto)
        {
            var dtoColl = DB.GetCollection<StoreTestDTO>(nameof(StoreTestDTO));
            dtoColl.Insert(dto);
            dtoColl.EnsureIndex(d => d.Id);
            return dto;
        }

        public long CountDTO()
        {
            var dtoColl = DB.GetCollection<StoreTestDTO>();
            return dtoColl.Count();
        }

        public StoreTestDTO GetDTO(long id)
        {
            var dtoColl = _litedb.GetCollection<StoreTestDTO>();
            var matches = dtoColl.Query().Where(d => d.Id == id);
            return matches.First();
        }

    }

    public class StoreTestDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal? PossibleMoney { get; set; }
        public DateTime Date { get; set; }
        public string[] Choices { get; set; }


    }
}
