// This file contains code for CreateMasterProjectCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Application.Common.Interfaces.WorkersPortfolio;
using HotelCore.Application.Common.Mappers;
using HotelCore.Application.MastersProjects.DTOs;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Entities.Workers;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.MastersProjects.Commands.CreateMasterProject;

public class CreateMasterProjectCommandHandler(
    IWorkerPortfolioItemRepository workerPortfolioItemRepository,
    IImageService imageService,
    IImageGroupRepository imageGroupRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMasterProjectCommand, MasterProjectDto>
{
    public async Task<MasterProjectDto> Handle(
        CreateMasterProjectCommand request,
        CancellationToken cancellationToken)
    {
        var portfolioItem = new WorkerPortfolioItem
        {
            WorkerProfileId = request.WorkerProfileId,
            Title = request.Title,
            Description = request.Description,
            CompletionDate = request.CompletionDate
        };

        await workerPortfolioItemRepository.AddAsync(portfolioItem, cancellationToken);

        IReadOnlyList<MyImageGroup> imageGroups = [];

        if (request.Images.Count > 0)
        {
            var imageRequests = request.Images
                .Select((file, index) => new SaveImageRequest(file, index))
                .ToList();

            imageGroups = await imageService.SaveImagesAsync(
                imageRequests,
                MyImageType.Project,
                cancellationToken);

            foreach (var imageGroup in imageGroups)
            {
                imageGroup.WorkerPortfolioItemId = portfolioItem.Id;
                await imageGroupRepository.AddAsync(imageGroup, cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new MasterProjectDto(
            portfolioItem.Id,
            portfolioItem.WorkerProfileId,
            portfolioItem.Title,
            portfolioItem.Description,
            portfolioItem.CompletionDate,
            imageGroups.ToDto());
    }
}
