// This file contains code for CreateServiceListingCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Domain.Entities.Workers;

namespace HotelCore.Application.MasterServices.Commands.CreateServiceListing;




public class CreateServiceListingCommandHandler(
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateServiceListingCommand, Guid>
{
    public async Task<Guid> Handle(CreateServiceListingCommand request, CancellationToken cancellationToken)
    {
        
        
        
        var listing = new WorkerServiceListing
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            StartingPrice = request.StartingPrice,
            LocationId = request.LocationId,
            Tags = request.Tags,
            IsActive = true
            
        };

        
        

        return Guid.NewGuid();
    }
}
