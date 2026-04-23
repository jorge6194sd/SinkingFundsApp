using System;
using SinkingFunds.Domain.Enums;

namespace SinkingFunds.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Direction { get; private set; }
        public DateTime OccurredOn { get; private set; }

        public Transaction(string description, decimal amount, TransactionType direction, DateTime occuredOn)
        {
            Id = Guid.NewGuid();
            Description = description;
            Amount = amount;
            Direction = direction;
            OccurredOn = occuredOn;
        }
    }
}
