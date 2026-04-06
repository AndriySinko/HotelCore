using MediatR;
using Microsoft.AspNetCore.Http;
using HotelCore.Application.Categories.DTOs;

namespace HotelCore.Application.Categories.Commands.PatchCategory;

public record PatchCategoryCommand(
    Guid Id,
    string? Name,
    string? Slug,
    string? Description,
    Guid? ParentId,
    bool RemoveParent,
    bool RemoveIcon,
    IFormFile? IconFile
) : IRequest<CategoryDto>;
