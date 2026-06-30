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

        public class EnvelopeSummary
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }

        public Envelope CreateEnvelope( string targetName)
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
            IEnumerable<Envelope> envelopesList =  repoType.GetAll();
            return GetEnvelopeSummaries(envelopesList);
        }

        private IEnumerable<EnvelopeSummary> GetEnvelopeSummaries(IEnumerable<Envelope> listOfEnvelopes)
        {
            List<EnvelopeSummary> returnedEnvelopes = new List<EnvelopeSummary>();
            foreach (Envelope envelope in listOfEnvelopes)
            {
                EnvelopeSummary summary = new EnvelopeSummary
                {
                    Id = envelope.Id,
                    Name = envelope.Name,
                    Balance = envelope.GetAmount()
                };
                returnedEnvelopes.Add(summary);
            }
            return returnedEnvelopes;
        }
    }
}
