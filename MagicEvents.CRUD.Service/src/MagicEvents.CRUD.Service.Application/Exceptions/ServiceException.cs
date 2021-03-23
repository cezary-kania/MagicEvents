using System;

namespace MagicEvents.CRUD.Service.Application.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message = null)
            : base(message)
        {
        }
    }
}