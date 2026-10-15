using SinkingFunds.Domain.Entities;
using Xunit;

namespace SinkingFunds.Tests.Domain
{
    public class EnvelopeErrorHandlingTests
    {
        [Fact]
        public void Envelope_MissingName_Throws()
        {
            //Act and Assert
            Assert.Throws<ArgumentException>(() => new Envelope(""));
        }

        [Fact]
        public void Deposit_MissingDescription_Throws()
        {

        }

        [Fact]
        public void Withdraw_MissingDescription_Throws()
        {

        }

        [Fact]
        public void Deposit_ZeroAmount_Throws()
        {

        }

        [Fact]
        public void Withdraw_ZeroAmount_Throws()
        {

        }

        [Fact]
        public void Deposit_NegativeAmount_Throws()
        {

        }

        [Fact]
        public void Withdraw_NegativeAmount_Throws()
        {

        }

        [Fact]
        public void Withdraw_AmountExceedsBalance_Throws()
        {

        }

        [Fact]
        public void SetTargetAmount_ZeroAmount_Throws()
        {

        }

        [Fact]
        public void SetTargetAmount_NegativeAmount_Throws()
        {

        }

        [Fact]
        public void SetTargetAmount_Above1000_Throws()
        {

        }

        [Fact]
        public void SetRecurringRule_NullRule_Throws()
        {

        }
    }
}