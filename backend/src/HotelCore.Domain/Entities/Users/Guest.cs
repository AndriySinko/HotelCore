// guest is a registered user who can make reservations and request hotel services
// identity fields (IdType, IdNumber, IdExpiry) are null until the guest checks in - reception fills them in at arrival
namespace HotelCore.Domain.Entities.Users;

public class Guest : User
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Phone { get; set; }

    // private setters - identity can only be set through VerifyIdentity so the flag stays consistent
    public string? IdType { get; private set; }
    public string? IdNumber { get; private set; }
    public DateTime? IdExpiry { get; private set; }
    public bool IsIdentityVerified { get; private set; } = false;

    public string GetFullName() => $"{FirstName} {LastName}";

    // called during check-in - verifies the guest presented a valid, non-expired document
    // if the id is expired the check-in is blocked even if the reservation exists
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
