namespace MagicEvents.Api.Service.Application.Exceptions
{
    public static class ExceptionMessage
    {
        public static class Org {
            public const string UknownError = "Unkown error occured";
        }
        public static class User 
        {
            public const string UserNotFound = "Invalid user id";
            public const string InvalidCredentials = "Invalid user email or password";
            public const string EmailAlreadyUsed = "EmailAlreadyUsed";
            public const string InvalidRole = "Invalid user role";
            public const string NoPermissionForOp = "Forbidden";
        }
        public static class Event
        {
            public const string EventNotFound = "Invalid event id";
            public const string InvalidEventRole = "Invalid event role";
            public const string UserAlreadyRegisteredForEvent = "User has been already registerd on event";
            public const string UserNotRegisteredForEvent = "User has been not registerd on event";
            public const string OrgCantLeaveEvent = "Organizator can't leave event";
            public const string CantRegisterForEvent = "CantRegisterForEvent";
            public const string EventHasFinished = "EventHasFinished";
        }
    }
}