// This file contains code for CreateWorkRequestCommand.
using MediatR;

namespace HotelCore.Application.Communication.WorkRequests.Commands.CreateWorkRequest;

public record CreateWorkRequestCommand(
    string Title,
    string Description,
    Guid SeekerProfileId,
    Guid CategoryId,
    Guid LocationId,
    decimal? Budget,
    DateTime? PreferredDate,
    string[] Tags
) : IRequest<Guid>;
