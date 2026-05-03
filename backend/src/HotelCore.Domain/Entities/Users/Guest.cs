// This file contains code for Guest.
namespace HotelCore.Domain.Entities.Users;






public class Guest : User
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }
    public string? RoomNumber { get; set; }
    public string? IdType { get; private set; }
    public string? IdNumber { get; private set; }
    public DateTime? IdExpiry { get; private set; }
    public bool IsIdentityVerified { get; private set; } = false;

    
    public string GetFullName() => $"{FirstName} {LastName}";

    
    
    
    
    public void VerifyIdentity(string idType, string idNumber, DateTime idExpiry)
    {
        if (idExpiry < DateTime.UtcNow)
            throw new ArgumentException("ID document is expired");

        IdType = idType;
        IdNumber = idNumber;
        IdExpiry = idExpiry;
        IsIdentityVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
