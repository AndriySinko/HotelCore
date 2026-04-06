using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Domain.Entities.Workers;

namespace HotelCore.Application.MasterServices.Commands.CreateServiceListing;

/// <summary>
/// Handler for creating a service advertisement from a master.
/// </summary>
public class CreateServiceListingCommandHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateServiceListingCommand, Guid>
{
    public async Task<Guid> Handle(CreateServiceListingCommand request, CancellationToken cancellationToken)
    {
        // We would get WorkerProfileId from the current user context
        // and validate that the worker has a profile (for example)
        
        var listing = new WorkerServiceListing
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            StartingPrice = request.StartingPrice,
            LocationId = request.LocationId,
            Tags = request.Tags,
            IsActive = true
            // WorkerProfileId would be set here
        };

        // repository.Add(listing);
        // await unitOfWork.SaveChangesAsync(cancellationToken);

        return Guid.NewGuid();
    }
}
