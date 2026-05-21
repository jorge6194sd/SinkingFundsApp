using SinkingFunds.Application.Abstractions;
using Xunit;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Application.Services;
using Microsoft.Data.Sqlite;
using SinkingFunds.Infrastructure.Adapters;
using SinkingFunds.Infrastructure.Persistence;

namespace SinkingFunds.Tests.Application
{
    public class EnvelopeServiceTestSqlite
    {
        
        private EnvelopeService _service;
        private SqliteEnvelopeRepository _repo;
        private SqliteSchemaInitializer _initializer;

        public EnvelopeServiceTestSqlite() 
        {
            
            string dbPath = "test-sinkingfunds.db";
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }
            string connString = $"Data Source={dbPath}";
            _initializer = new SqliteSchemaInitializer(connString);
            _initializer.SchemaVerification();
            _repo = new SqliteEnvelopeRepository(connString);
            _service = new EnvelopeService(_repo);
        }
      

        [Fact]
        public void DepositToEnvelope()
        {
            //arrange
            Envelope testEnvelope = _service.CreateEnvelope("app-test-car-fund");
            string despoitDescription = "weekly 20 dollar contribution";
            decimal depositAmount = 20;
            DateTime depositDate = DateTime.Now;

            //act
            _service.DepositToEnvelope(testEnvelope.Id, despoitDescription, depositAmount, depositDate);

            //assert
            Envelope returnedEnvelope = _repo.GetById(testEnvelope.Id);
            Assert.Equal(20m, returnedEnvelope.GetAmount());
        }

        [Fact]
        public void WithdrawFromEnvelope()
        {
            //arrange
            Envelope testEnvelope = _service.CreateEnvelope("app-test-car-fund");
            string despoitDescription = "weekly 20 dollar contribution";
            decimal depositAmount = 20;
            string withdrawDescription = "I need 10";
            decimal withdrawAmount = 10;
            DateTime depositDate = DateTime.Now;
            _service.DepositToEnvelope(testEnvelope.Id, despoitDescription, depositAmount, depositDate);

            //act
            _service.WithdrawFromEnvelope(testEnvelope.Id, withdrawDescription, withdrawAmount, depositDate);

            //assert
            Envelope returnedEnvelope = _repo.GetById(testEnvelope.Id);
            Assert.Equal(10m, returnedEnvelope.GetAmount());
        }
    }
}
