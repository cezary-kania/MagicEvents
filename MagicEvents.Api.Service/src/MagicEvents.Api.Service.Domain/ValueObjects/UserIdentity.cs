namespace MagicEvents.Api.Service.Domain.ValueObjects
{
    public class UserIdentity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}