using System;
using System.ComponentModel;
using System.Text;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;
using System.Collections.Generic;

namespace SinkingFunds.Infrastructure.Adapters
{
    public class InMemoryDb : IEnvelopeRepository
    {
        private List<Envelope> envelopes { get; set; } = new List<Envelope>();
    
        public void Add(Envelope envelope)
        {
            envelopes.Add(envelope);
        }

        public Envelope GetById(Guid id)
        {
            foreach (var envelope in envelopes) 
            {
                if(envelope.Id == id)
                {
                    return envelope;
                }
            }

            throw new KeyNotFoundException(
                $"Envelope with Id '{id}' was not found."    
            );

        }
        
    }
}
