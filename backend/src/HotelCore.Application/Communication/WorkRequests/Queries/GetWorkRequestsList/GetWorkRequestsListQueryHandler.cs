// This file contains code for GetWorkRequestsListQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.WorkRequests;
using HotelCore.Application.Communication.WorkRequests.DTOs;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestsList;

public class GetWorkRequestsListQueryHandler(IWorkRequestRepository workRequestRepository)
    : IRequestHandler<GetWorkRequestsListQuery, IReadOnlyList<SimpleWorkRequestDto>>
{
    public async Task<IReadOnlyList<SimpleWorkRequestDto>> Handle(GetWorkRequestsListQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        if (!IsOwnerRequest(request.CurrentUserId, filter.SeekerUserId))
            filter = filter with { ExcludeStatus = WorkRequestStatus.Draft };

        var workRequests = await workRequestRepository.GetWorkRequestsAsync(
            filter,
            request.Page,
            cancellationToken);

        return workRequests
            .Select(wr => new SimpleWorkRequestDto
            {
                Id = wr.Id,
                Title = wr.Title,
                Status = wr.Status,
                Budget = wr.Budget,
                PreferredDate = wr.PreferredDate,
                CategoryId = wr.CategoryId,
                LocationId = wr.LocationId,
                SeekerProfileId = wr.SeekerProfileId,
                CreatedAt = wr.CreatedAt
            })
            .ToList();
    }

    private static bool IsOwnerRequest(Guid? currentUserId, Guid? seekerUserId)
    {
        return currentUserId.HasValue
            && seekerUserId.HasValue
            && currentUserId.Value == seekerUserId.Value;
    }
}
