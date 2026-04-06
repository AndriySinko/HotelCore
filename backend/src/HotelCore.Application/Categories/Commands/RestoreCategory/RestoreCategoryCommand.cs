using MediatR;

namespace HotelCore.Application.Categories.Commands.RestoreCategory;

public record RestoreCategoryCommand(Guid Id) : IRequest;
