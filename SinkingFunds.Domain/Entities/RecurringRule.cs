using System;
using SinkingFunds.Domain.Enums;

namespace SinkingFunds.Domain.Entities
{
    public class RecurringRule
    {
        public int Frequency { get; private set; }
        public FrequencyUnits Unit { get; private set; }
        public decimal RuleAmount { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime NextDueDate { get; private set; }

        public RecurringRule(
            int frequency,
            FrequencyUnits unit,
            decimal ruleAmount,
            DateTime nextDueDate)
        {
            if (frequency <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(frequency),
                    "Frequency must be greater than zero.");
            }

            if (ruleAmount <= 0 || ruleAmount > 1000m)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ruleAmount),
                    "Contribution must be greater than zero and at most $1,000.");
            }

            Frequency = frequency;
            Unit = unit;
            RuleAmount = ruleAmount;
            NextDueDate = nextDueDate;
            IsActive = true;
        }
    }
}