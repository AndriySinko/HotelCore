// This file contains code for UpdateWorkRequestStatusCommand.
using MediatR;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequestStatus;

public record UpdateWorkRequestStatusCommand(
    Guid WorkRequestId,
    WorkRequestStatus NewStatus,
    Guid CurrentUserId) : IRequest;
