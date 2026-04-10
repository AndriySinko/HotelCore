// This file contains code for GetMasterProjectByIdQueryHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Application.Common.Mappers;
using HotelCore.Application.MastersProjects.DTOs;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.MastersProjects.Queries.GetMasterProjectById;

public class GetMasterProjectByIdQueryHandler(
    IWorkerPortfolioItemRepository workerPortfolioItemRepository) 
    : IRequestHandler<GetMasterProjectByIdQuery, MasterProjectDto>
{
    public async Task<MasterProjectDto> Handle(
        GetMasterProjectByIdQuery getMasterProjectByIdQueryRequest,
        CancellationToken cancellationToken)
    {
        var masterProject = await workerPortfolioItemRepository.GetByIdWithImagesAsync(
            getMasterProjectByIdQueryRequest.masterProjectId,
            cancellationToken)
            ?? throw new NotFoundException(nameof(WorkerPortfolioItem), getMasterProjectByIdQueryRequest.masterProjectId);

        return new MasterProjectDto(
            masterProject.Id,
            masterProject.WorkerProfileId,
            masterProject.Title,
            masterProject.Description,
            masterProject.CompletionDate,
            masterProject.Images.ToDto());
    }
}
