// This file contains code for AddOrderImagesCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.AddOrderImages;

public class AddOrderImagesCommandHandler(
    IOrderRepository orderRepository,
    IImageService imageService,
    IImageGroupRepository imageGroupRepository,
    ILogger<AddOrderImagesCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddOrderImagesCommand>
{
    public async Task Handle(AddOrderImagesCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding {ImageCount} images to order {OrderId}", request.Images.Count, request.OrderId);

        var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        if (order.CreatedByUserId != request.CurrentUserId)
        {
             throw new ForbiddenException("You do not have permission to perform this action");
        }

        var imageRequests = request.Images
            .Select((file, index) => new SaveImageRequest(file, index))
            .ToList();

        logger.LogInformation("Saving {ImageCount} images for order {OrderId}", imageRequests.Count, request.OrderId);

        var imageGroups = await imageService.SaveImagesAsync(
            imageRequests,
            MyImageType.Order,
            cancellationToken);

        logger.LogInformation("Successfully saved {GroupCount} image groups for order {OrderId}", imageGroups.Count, request.OrderId);

        foreach (var imageGroup in imageGroups)
        {
            imageGroup.OrderId = order.Id;
            await imageGroupRepository.AddAsync(imageGroup, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully added {ImageGroupCount} image groups to order {OrderId}", imageGroups.Count, request.OrderId);
    }
}
