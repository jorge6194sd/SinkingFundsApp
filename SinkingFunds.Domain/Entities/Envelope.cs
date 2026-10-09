using SinkingFunds.Domain.Enums;
using System;
using System.Collections.ObjectModel;
using SinkingFunds.Domain.Exceptions;

namespace SinkingFunds.Domain.Entities
{
    public class Envelope
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public Guid Id { get; private set; }
        public decimal? TargetAmount { get; private set; }

        private readonly Collection<Transaction> transactions;
        private RecurringRule? envelopeDepositRule;

        public Envelope(string name)
        {
            this.Id = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "An envelope name is required.",
                    nameof(name));
            }
            this.Name = name.Trim();
            this.IsActive = true;
            this.transactions = new Collection<Transaction>();
            this.envelopeDepositRule = null;
        }

        public Envelope(
    Guid id,
    string name,
    bool isActive,
    decimal? targetAmount)
        {
            this.Id = id;

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "An envelope name is required.",
                    nameof(name));
            }

            this.Name = name.Trim();
            this.IsActive = isActive;

            // Validate a saved target if one exists.
            if (targetAmount.HasValue)
            {
                SetTargetAmount(targetAmount.Value);
            }

            this.transactions = new Collection<Transaction>();
            this.envelopeDepositRule = null;
        }

        public void Deposit(string description, decimal amount, DateTime occuredOn)
        {
            var transaction = new Transaction(description, amount, TransactionType.Deposit, occuredOn);
            transactions.Add(transaction);
        }

        public void Withdraw(
        string description,
        decimal amount,
        DateTime occuredOn)
        {
            var transaction = new Transaction(
                description,
                amount,
                TransactionType.Withdrawal,
                occuredOn);
            if (amount > GetAmount())
            {
                throw new DomainValidationException(
                    "Withdrawal cannot exceed the envelope balance.");
            }
            

            transactions.Add(transaction);
        }
        public void AddExistingTransaction(Transaction transaction)
        {
            transactions.Add(transaction);
        }

        public decimal GetAmount()
        {
            decimal total = 0;

            foreach (var transaction in transactions)
            {
                if (transaction.Direction == TransactionType.Deposit)
                {
                    total += transaction.Amount;
                }
                else
                {
                    total -= transaction.Amount;
                }
            }

            return total;
        }

        public void SetRecurringRule(RecurringRule rule)
        {
            this.envelopeDepositRule = rule
                ?? throw new ArgumentNullException(nameof(rule));
        }
        public IEnumerable<Transaction> GetTransactions()
        {
            return transactions; 
        }
        public RecurringRule? GetRecurringRule()
        {
            return envelopeDepositRule;
        }
        public void SetTargetAmount(decimal targetAmount)
        {
            if (targetAmount <= 0 || targetAmount > 1000m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(targetAmount),
                    "Target amount must be greater than zero and at most $1,000.");
            }

            this.TargetAmount = targetAmount;
        }
    }
}
