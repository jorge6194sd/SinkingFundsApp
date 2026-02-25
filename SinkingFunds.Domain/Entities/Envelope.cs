using SinkingFunds.Domain.Enums;
using System;
using System.Collections.ObjectModel;
using System.Transactions;

namespace SinkingFunds.Domain.Entities
{
    public class Envelope
    {
        public string Name { get; private set; }
        public bool IsActive { get; private set; }
        public Guid Id { get; private set; } 

        private readonly Collection<Transaction> transactions;
        private RecurringRule? envelopeDepositRule;

        public Envelope(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            IsActive = true;
            transactions = new Collection<Transaction>();
            envelopeDepositRule = null;
        }

        public void Deposit(string description, decimal amount, DateTime occuredOn)
        {
            var transaction = new Transaction(description, amount, TransactionType.Deposit, occuredOn);
            transactions.Add(transaction);
        }

        public void Withdraw(string description, decimal amount, DateTime occuredOn)
        {
            var transaction = new Transaction(description, amount, TransactionType.Withdrawal, occuredOn);
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
            envelopeDepositRule = rule;
        }
    }
}
