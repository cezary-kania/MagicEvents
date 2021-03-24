namespace MagicEvents.CRUD.Service.Application.Exceptions
{
    public static class ExceptionMessage
    {
        public static class User 
        {
            public const string UserNotFound = "Invalid user id";
            public const string InvalidRole = "Invalid user role";
        }
        public static class Event
        {
            public const string EventNotFound = "Invalid event id";
            public const string InvalidEventRole = "Invalid event role";
            public const string UserAlreadyRegisteredForEvent = "User has been already registerd on event";
            public const string UserNotRegisteredForEvent = "User has been not registerd on event";
        }
    }
}