using System;

namespace MagicEvents.Api.Service.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException() : base() {}
        public DomainException(string message) : base(message) {}
    }
}