
using System;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Domain.Exceptions;
using Xunit;

namespace SinkingFunds.Tests.Domain
{
    public class EnvelopeErrorHandlingTests
    {
        [Fact]
        public void Envelope_MissingName_Throws()
        {
            // Act and Assert
            Assert.Throws<ArgumentException>(() => new Envelope(""));
        }

        [Fact]
        public void Deposit_MissingDescription_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                testEnvelope.Deposit("", 20, DateTime.Now));
        }

        [Fact]
        public void Withdraw_MissingDescription_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentException>(() =>
                testEnvelope.Withdraw("", 20, DateTime.Now));
        }

        [Fact]
        public void Deposit_ZeroAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.Deposit("zero-amount", 0, DateTime.Now));
        }

        [Fact]
        public void Withdraw_ZeroAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.Withdraw("zero-amount", 0, DateTime.Now));
        }

        [Fact]
        public void Deposit_NegativeAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.Deposit("negative-amount", -5, DateTime.Now));
        }

        [Fact]
        public void Withdraw_NegativeAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.Withdraw("negative-amount", -5, DateTime.Now));
        }

        [Fact]
        public void Withdraw_AmountExceedsBalance_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");
            testEnvelope.Deposit("initial deposit", 20, DateTime.Now);

            // Act and Assert
            Assert.Throws<DomainValidationException>(() =>
                testEnvelope.Withdraw("excessive withdrawal", 30, DateTime.Now));
        }

        [Fact]
        public void SetTargetAmount_ZeroAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.SetTargetAmount(0));
        }

        [Fact]
        public void SetTargetAmount_NegativeAmount_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.SetTargetAmount(-5));
        }

        [Fact]
        public void SetTargetAmount_Above1000_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                testEnvelope.SetTargetAmount(1001));
        }

        [Fact]
        public void SetRecurringRule_NullRule_Throws()
        {
            // Arrange
            Envelope testEnvelope = new Envelope("test-envelope");

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() =>
                testEnvelope.SetRecurringRule(null!));
        }
    }
}
