// This file contains code for StaffMember.
namespace HotelCore.Domain.Entities.Users;




public class StaffMember : User
{
    public required string Department { get; set; }
    public required string Position { get; set; }
    public int ContractHoursPerWeek { get; set; }
    public DateTime HireDate { get; set; }
}
