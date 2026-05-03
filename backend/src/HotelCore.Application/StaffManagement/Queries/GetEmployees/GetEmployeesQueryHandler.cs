// This file contains code for GetEmployeesQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.StaffManagement;

namespace HotelCore.Application.StaffManagement.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IStaffRepository staffRepo)
    : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
{
    public async Task<List<EmployeeDto>> Handle(GetEmployeesQuery query, CancellationToken ct)
    {
        var staff = string.IsNullOrWhiteSpace(query.Department)
            ? await staffRepo.GetAllAsync(ct)
            : await staffRepo.GetByDepartmentAsync(query.Department, ct);

        return staff.Select(s => new EmployeeDto(
            s.Id, s.UserName ?? string.Empty, s.Department, s.Position, s.ContractHoursPerWeek
        )).ToList();
    }
}
