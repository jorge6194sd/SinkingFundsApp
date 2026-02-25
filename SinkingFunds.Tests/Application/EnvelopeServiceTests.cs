using SinkingFunds.Application.Abstractions;
using Xunit;
using SinkingFunds.Domain.Entities;
using SinkingFunds.Application.Services;

namespace SinkingFunds.Tests.Application
{
    public class EnvelopeServiceTests
    {
        private FakeEnvelopeRepository _repo;
        private EnvelopeService _service;
       
        public EnvelopeServiceTests()
        {
            _repo = new FakeEnvelopeRepository();
            _service = new EnvelopeService(_repo);
        }

        private class FakeEnvelopeRepository : IEnvelopeRepository
        {
            List<Envelope> tempList = new List<Envelope>();
            public void Add(Envelope envelope)
            {
                tempList.Add(envelope);
            }

            public Envelope GetById(Guid id)
            {
                foreach (var envelope in tempList)
                {
                    if (envelope.Id == id)
                    {
                        return envelope;
                    }
                }

                throw new KeyNotFoundException(
                    $"Envelope with Id '{id}' was not found."
                );
            }

        }

        [Fact]
        public void DepositToEnvelope_LoadsById_AndCallsDeposit()
        {
            //arrange
            Envelope carFund = new Envelope("Car Fund");
            _repo.Add(carFund);
            string despoitDescription = "weekly 20 dollar contribution";
            decimal depositAmount = 20;
            DateTime depositDate = DateTime.Now;

            //act
            _service.DepositToEnvelope(carFund.Id, despoitDescription, depositAmount, depositDate);

            //assert
            Envelope returnedEnvelope = _repo.GetById(carFund.Id);
            Assert.Equal(20m, returnedEnvelope.GetAmount());


        }
    }
}
