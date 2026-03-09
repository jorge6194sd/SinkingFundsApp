using SinkingFunds.Domain.Entities;
using Xunit;

namespace SinkingFunds.Tests.Domain
{
    public class EnvelopeTests
    {

        [Fact]
        public void Envelope_ShouldStartAtZero()
        {
            //Arrange
            Envelope testEnvelope = new Envelope("test starter envelope");

            //Act

            //Assert
            Assert.Equal(0m, testEnvelope.GetAmount());
            
        }

        [Fact]
        public void Envelope_DepositShouldIncreaseBalance()
        {
            //Arrange
            Envelope testEnvelope = new Envelope("test starter envelope");

            //Act
            testEnvelope.Deposit("test amount", 10, DateTime.Now);

            //Assert
            Assert.Equal(10m, testEnvelope.GetAmount());

        }

        [Fact]
        public void Envelope_WithdrawShouldDecreaseBalance()
        {
            //Arrange
            Envelope testEnvelope = new Envelope("test starter envelope");
            testEnvelope.Deposit("test amount", 10, DateTime.Now);

            //Act
            testEnvelope.Withdraw("gonna take out now", 6, DateTime.Now);

            //Assert
            Assert.Equal(4m, testEnvelope.GetAmount());

        }

    }
}
