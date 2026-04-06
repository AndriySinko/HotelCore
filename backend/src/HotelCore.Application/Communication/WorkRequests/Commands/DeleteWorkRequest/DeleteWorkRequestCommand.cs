using MediatR;

namespace HotelCore.Application.Communication.WorkRequests.Commands.DeleteWorkRequest;

public record DeleteWorkRequestCommand(Guid WorkRequestId, Guid CurrentUserId) : IRequest;
