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

        public RecurringRule(int frequency, FrequencyUnits unit, decimal ruleAmount, DateTime nextDueDate)
        {
            Frequency = frequency;
            Unit = unit;
            RuleAmount = ruleAmount;
            NextDueDate = nextDueDate;
            IsActive = true;
        }
    }
}
