namespace HotelCore.Domain.Entities.Users
{
    public class Guest : User
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
