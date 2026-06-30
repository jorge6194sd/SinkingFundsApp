using Microsoft.Data.Sqlite;
using SinkingFunds.Application.Abstractions;
using SinkingFunds.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SinkingFunds.Infrastructure.Adapters
{
    public class InMemoryDb : IEnvelopeRepository
    {
        private List<Envelope> envelopes { get; set; } = new List<Envelope>();
    
        public void Add(Envelope envelope)
        {
            envelopes.Add(envelope);
        }

        public void Save(Envelope envelope)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Envelope> GetAll()
        {
            throw new NotImplementedException();
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
