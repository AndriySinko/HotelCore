using MediatR;
using Microsoft.AspNetCore.Http;

namespace HotelCore.Application.Orders.Commands.AddOrderImages;

public record AddOrderImagesCommand(
    Guid OrderId,
    Guid CurrentUserId,
    IReadOnlyList<IFormFile> Images) : IRequest;
