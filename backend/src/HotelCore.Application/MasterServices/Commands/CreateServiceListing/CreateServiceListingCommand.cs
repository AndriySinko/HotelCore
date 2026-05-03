// This file contains code for CreateServiceListingCommand.
using MediatR;

namespace HotelCore.Application.MasterServices.Commands.CreateServiceListing;

public record CreateServiceListingCommand(
    string Title,
    string Description,
    Guid CategoryId,
    decimal? StartingPrice,
    Guid? LocationId,
    string[] Tags
) : IRequest<Guid>;
