using MediatR;
using HotelCore.Application.Categories.DTOs;

namespace HotelCore.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    string? IconUrl,
    Guid? ParentId
) : IRequest<CategoryDto>;