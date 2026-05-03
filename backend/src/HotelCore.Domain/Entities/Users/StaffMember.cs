// extends the base User with employment info
// ContractHoursPerWeek is checked in AssignShiftCommandHandler to prevent overtime
namespace HotelCore.Domain.Entities.Users;

public class StaffMember : User
{
    // department groups staff for scheduling — e.g. "Cleaning", "Reception", "Kitchen"
    public required string Department { get; set; }
    public required string Position { get; set; }
    // used to validate weekly shift assignments — cant assign more hours than the contract allows
    public int ContractHoursPerWeek { get; set; }
    public DateTime HireDate { get; set; }
}
