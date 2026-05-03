// This file contains code for GetMasterProjectListQuerie.
using MediatR;
using HotelCore.Application.Common.Models;
using HotelCore.Application.MastersProjects.DTOs;
using HotelCore.Application.MastersProjects.Models;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectList;

public record GetMasterProjectListQuery(
    MasterProjectFilter Filter,
    PageRequest? Pagination = null)
    : IRequest<IReadOnlyList<MasterProjectListDto>>
{
    public PageRequest Page => Pagination ?? PageRequest.Default;
}
