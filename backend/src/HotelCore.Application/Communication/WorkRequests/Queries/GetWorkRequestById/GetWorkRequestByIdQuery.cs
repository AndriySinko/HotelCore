using MediatR;
using HotelCore.Application.Communication.WorkRequests.DTOs;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestById;

public record GetWorkRequestByIdQuery(
    Guid WorkRequestId,
    Guid? CurrentUserId = null) : IRequest<WorkRequestDto>;
