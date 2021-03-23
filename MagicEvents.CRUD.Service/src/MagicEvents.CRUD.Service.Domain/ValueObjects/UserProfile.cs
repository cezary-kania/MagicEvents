namespace MagicEvents.CRUD.Service.Domain.ValueObjects
{
    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Informations { get; set; }
        public UserProfileImage Image { get; set; }
    }
}