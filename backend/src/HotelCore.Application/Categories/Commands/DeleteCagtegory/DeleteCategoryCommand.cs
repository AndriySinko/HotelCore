using MediatR;

namespace HotelCore.Application.Categories.Commands.DeleteCagtegory;

public record DeleteCategoryCommand(Guid Id) : IRequest;