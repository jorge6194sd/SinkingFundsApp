using SinkingFunds.Domain.Entities;
using SinkingFunds.Infrastructure.Adapters;
using Xunit;

namespace SinkingFunds.Tests.Infrastructure
{
    public class InMemoryAdapter
    {

        [Fact]
        public void Add_ThenGetById()
        {
            //Arrange
            Envelope fakeEnvelope = new Envelope("sample envelope");
            InMemoryDb fakeInMemoryAdapter = new InMemoryDb();
            fakeInMemoryAdapter.Add(fakeEnvelope);

            //Act
            fakeInMemoryAdapter.Add(fakeEnvelope);

            //Assert
            Assert.Same(fakeEnvelope, fakeInMemoryAdapter.GetById(fakeEnvelope.Id));

        }

    }
}
