using System;

namespace MagicEvents.Api.Service.Application.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message = null)
            : base(message)
        {
        }
    }
}