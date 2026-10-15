using System;

namespace SinkingFunds.Domain.Exceptions
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException(string message)
            : base(message)
        {
        }
    }
}