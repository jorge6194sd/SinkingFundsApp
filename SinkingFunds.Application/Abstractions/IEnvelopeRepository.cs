using SinkingFunds.Domain.Entities;


namespace SinkingFunds.Application.Abstractions
{
    public interface IEnvelopeRepository
    {
         void Add(Envelope envelope);

         Envelope GetById(Guid lookupId);

        void Save(Envelope envelope);

        IEnumerable<Envelope> GetAll();

    }
}
