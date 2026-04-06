using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Commands.CreateWorkRequest;

public class CreateWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateWorkRequestCommand, Guid>
{
    public async Task<Guid> Handle(CreateWorkRequestCommand createWorkRequestCommandRequest, CancellationToken cancellationToken)
    {
        var workRequest = new WorkRequest
        {
            Title = createWorkRequestCommandRequest.Title,
            Description = createWorkRequestCommandRequest.Description,
            SeekerProfileId = createWorkRequestCommandRequest.SeekerProfileId,
            CategoryId = createWorkRequestCommandRequest.CategoryId,
            LocationId = createWorkRequestCommandRequest.LocationId,
            Budget = createWorkRequestCommandRequest.Budget,
            PreferredDate = createWorkRequestCommandRequest.PreferredDate,
            Tags = createWorkRequestCommandRequest.Tags,
            Status = WorkRequestStatus.Draft 
        };

        workRequestRepository.Add(workRequest);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return workRequest.Id;
    }
}
