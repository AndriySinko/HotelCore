using MediatR;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Application.Communication.WorkRequests.DTOs;
using HotelCore.Domain.Entities.Communication;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestById;

public class GetWorkRequestByIdQueryHandler(IWorkRequestRepository workRequestRepository)
    : IRequestHandler<GetWorkRequestByIdQuery, WorkRequestDto>
{
    public async Task<WorkRequestDto> Handle(
        GetWorkRequestByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var workResult = await workRequestRepository.GetByIdAsync(request.WorkRequestId, cancellationToken)
            ?? throw new NotFoundException(nameof(WorkRequest), request.WorkRequestId);

        if (workResult.Status == WorkRequestStatus.Draft)
        {
            var isOwner = request.CurrentUserId.HasValue
                && workResult.SeekerProfile?.UserId == request.CurrentUserId.Value;
            if (!isOwner)
                throw new NotFoundException(nameof(WorkRequest), request.WorkRequestId);
        }

        return new WorkRequestDto
        {
            Id = workResult.Id,
            Title = workResult.Title,
            Description = workResult.Description,
            SeekerProfileId = workResult.SeekerProfileId,
            CategoryId = workResult.CategoryId,
            LocationId = workResult.LocationId,
            Status = workResult.Status,
            Budget = workResult.Budget,
            PreferredDate = workResult.PreferredDate,
            Tags = workResult.Tags,
            CreatedAt = workResult.CreatedAt,
            UpdatedAt = workResult.UpdatedAt
        };
    }
}
