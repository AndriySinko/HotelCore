// This file contains code for GetWorkRequestsListQuery.
using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Application.Communication.WorkRequests.DTOs;
using HotelCore.Application.Communication.WorkRequests.Models;

namespace HotelCore.Application.Communication.WorkRequests.Queries.GetWorkRequestsList;

public record GetWorkRequestsListQuery(
    WorkRequestsFilter Filter,
    PageRequest? Pagination = null,
    Guid? CurrentUserId = null
) : IRequest<IReadOnlyList<SimpleWorkRequestDto>>
{
    public PageRequest Page => Pagination ?? PageRequest.Default;
}
