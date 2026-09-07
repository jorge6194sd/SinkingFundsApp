using System;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;

namespace SinkingFunds.Application.Services
{
    public class EnvelopeService
    {
        IEnvelopeRepository repoType;
        public EnvelopeService(IEnvelopeRepository targetRepo)
        {
            repoType = targetRepo;
        }

        //helper class for grid presentation
        public class EnvelopeSummary
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }
            public decimal? TargetAmount { get; set; }
            public decimal? RuleAmount { get; set; }

            public string MonthsRemaining { get; set; }
        }

        public Envelope CreateEnvelope(string targetName)
        {
            Envelope newEnvelope = new Envelope(targetName);
            repoType.Add(newEnvelope);
            return newEnvelope;
        }
        public void DepositToEnvelope(Guid envelopeId, string depositDescription, decimal depositQuantity, DateTime depositDate)
        {
            Envelope envelope = repoType.GetById(envelopeId);
            envelope.Deposit(depositDescription, depositQuantity, depositDate);
            repoType.Save(envelope);
        }

        public void WithdrawFromEnvelope(Guid envelopeId, string withdrawDescription, decimal withdrawQuantity, DateTime withdrawDate)
        {
            Envelope envelope = repoType.GetById(envelopeId);
            envelope.Withdraw(withdrawDescription, withdrawQuantity, withdrawDate);
            repoType.Save(envelope);
        }
        public decimal GetEnvelopeBalance(Guid envelopeId)
        {
            Envelope envelope = repoType.GetById(envelopeId);
            return envelope.GetAmount();
        }

        public IEnumerable<EnvelopeSummary> GetAllEnvelopeSummaries()
        {
            IEnumerable<Envelope> envelopesList = repoType.GetAll();
            return GetEnvelopeSummaries(envelopesList);
        }

        private IEnumerable<EnvelopeSummary> GetEnvelopeSummaries(IEnumerable<Envelope> listOfEnvelopes)
        {
            List<EnvelopeSummary> returnedEnvelopes = new List<EnvelopeSummary>();

            foreach (Envelope envelope in listOfEnvelopes)
            {
                string monthsRemaining = "Not set";

                if (envelope.TargetAmount.HasValue &&
                    envelope.GetRecurringRule() != null)
                {
                    decimal remaining =
                        envelope.TargetAmount.Value - envelope.GetAmount();

                    if (remaining <= 0)
                    {
                        monthsRemaining = "Funded";
                    }
                    else
                    {
                        int months = (int)Math.Ceiling(
                            remaining / envelope.GetRecurringRule().RuleAmount
                        );

                        monthsRemaining =
                            months <= 12
                                ? $"{months} months"
                                : "> 1 year";
                    }
                }

                EnvelopeSummary summary = new EnvelopeSummary
                {
                    Id = envelope.Id,
                    Name = envelope.Name,
                    Balance = envelope.GetAmount(),
                    TargetAmount = envelope.TargetAmount,
                    RuleAmount = envelope.GetRecurringRule()?.RuleAmount,
                    MonthsRemaining = monthsRemaining
                };

                returnedEnvelopes.Add(summary);
            }

            return returnedEnvelopes;
        }

        public void DeleteEnvelope(Guid id)
        {
            repoType.Delete(id);
        }
        public void UpdateTargetAmount(Guid envelopeId, decimal amt)
        {
            Envelope targetEnvelope = repoType.GetById(envelopeId);
            targetEnvelope.SetTargetAmount(amt);
            repoType.Save(targetEnvelope);
        }

        public void SetMonthlyContribution(Guid envelopeId, decimal amount)
        {
            Envelope envelope = repoType.GetById(envelopeId);

            RecurringRule monthlyRule = new RecurringRule(
                frequency: 1,
                unit: Domain.Enums.FrequencyUnits.Months,
                ruleAmount: amount,
                nextDueDate: DateTime.Today.AddMonths(1)
                );

            envelope.SetRecurringRule(monthlyRule);

            repoType.Save(envelope);
        }
    }
}
