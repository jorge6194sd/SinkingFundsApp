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

        // Create a new transaction.
        public Transaction(
            string description,
            decimal amount,
            TransactionType direction,
            DateTime occurredOn)
            : this(
                Guid.NewGuid(),
                description,
                amount,
                direction,
                occurredOn)
        {
        }

        // Reconstruct a saved transaction with its existing ID.
        public Transaction(
            Guid id,
            string description,
            decimal amount,
            TransactionType direction,
            DateTime occurredOn)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException(
                    "A description is required.",
                    nameof(description));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Amount must be greater than zero.");
            }

            Id = id;
            Description = description.Trim();
            Amount = amount;
            Direction = direction;
            OccurredOn = occurredOn;
        }
    }
}