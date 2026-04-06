using MediatR;

namespace HotelCore.Application.Communication.WorkRequests.Commands.UpdateWorkRequest;

public record UpdateWorkRequestCommand(
    Guid WorkRequestId,
    Guid CurrentUserId,
    string? Title,
    string? Description,
    Guid? CategoryId,
    Guid? LocationId,
    decimal? Budget,
    DateTime? PreferredDate,
    string[]? Tags) : IRequest;
