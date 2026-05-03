// This file contains code for IStaffRepository.
using HotelCore.Domain.Entities.Users;

namespace HotelCore.Application.Common.Interfaces.StaffManagement;

public interface IStaffRepository : IBaseRepository<StaffMember>
{
    Task<StaffMember?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<StaffMember>> GetByDepartmentAsync(string department, CancellationToken ct = default);
    Task<List<StaffMember>> GetAvailableStaffAsync(DateTime date, string role, CancellationToken ct = default);
    Task<List<StaffMember>> GetAllAsync(CancellationToken ct = default);
}
