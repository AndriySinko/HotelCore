// This file contains code for GetMyShiftsQuery.
using MediatR;
using HotelCore.Application.StaffManagement.Queries.GetSchedule;

namespace HotelCore.Application.StaffManagement.Queries.GetMyShifts;

public record GetMyShiftsQuery(Guid StaffId) : IRequest<ScheduleDto?>;