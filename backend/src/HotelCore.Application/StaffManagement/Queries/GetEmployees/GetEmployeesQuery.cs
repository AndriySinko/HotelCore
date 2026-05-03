// This file contains code for GetEmployeesQuery.
using MediatR;

namespace HotelCore.Application.StaffManagement.Queries.GetEmployees;

public record GetEmployeesQuery(string? Department = null) : IRequest<List<EmployeeDto>>;

public record EmployeeDto(
    Guid Id,
    string UserName,
    string Department,
    string Position,
    int ContractHoursPerWeek);
