// This file contains code for GetMasterProjectListQuerieHandler.
using MediatR;

using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Application.Common.Mappers;
using HotelCore.Application.MastersProjects.DTOs;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectList;

public class GetMasterProjectListQueryHandler(
    IWorkerPortfolioItemRepository workerPortfolioItemRepository)
    : IRequestHandler<GetMasterProjectListQuery, IReadOnlyList<MasterProjectListDto>>
{
    public async Task<IReadOnlyList<MasterProjectListDto>> Handle(
        GetMasterProjectListQuery request,
        CancellationToken cancellationToken)
    {
        var masterProjects = await workerPortfolioItemRepository.GetMasterProjectsAsync(
            request.Filter,
            request.Page,
            cancellationToken);

        return masterProjects.Select(mp => new MasterProjectListDto(
            mp.Id,
            mp.WorkerProfileId,
            mp.Title,
            mp.CompletionDate,
            mp.Images.ToDto())).ToList();
    }
}
