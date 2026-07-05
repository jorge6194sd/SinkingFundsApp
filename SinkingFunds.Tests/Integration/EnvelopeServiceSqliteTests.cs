using SinkingFunds.Application.Services;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Infrastructure.Adapters;
using SinkingFunds.Infrastructure.Persistence;
using System;
using Xunit;

namespace SinkingFunds.Tests.Integration
{
    public class EnvelopeServiceSqliteTests
    {
        private readonly SqliteEnvelopeRepository _repository;
        private readonly EnvelopeService _service;

        public EnvelopeServiceSqliteTests() 
        {
            //db config setup
            string dbPath = "test-sinkingfunds.db";
            string connString = $"Data Source={dbPath}";

            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            //schema initializer
            SqliteSchemaInitializer initializer = new SqliteSchemaInitializer(connString);
            initializer.SchemaVerification();

            //repository initializer
            _repository = new SqliteEnvelopeRepository(connString);

            //inject repo into envelopeService for app use cases
            _service = new EnvelopeService(_repository);
            
            

        }

        [Fact]
        public void CreateEnvelope_DepositThenDelete_EnvelopeIsRemoved()
        {
            //Arrange
            //using application project to orchestrate the envelope creation
            Envelope testEnvelope = _service.CreateEnvelope("test-envelope");
            _service.DepositToEnvelope(testEnvelope.Id, "dummy-desposit", 40m, DateTime.Now);

            //Act
            _service.DeleteEnvelope(testEnvelope.Id);

            //Assert
            Assert.Throws<KeyNotFoundException>(() => _repository.GetById(testEnvelope.Id));
        }


    
    }
}
