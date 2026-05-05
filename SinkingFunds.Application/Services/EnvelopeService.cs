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
    }
}
