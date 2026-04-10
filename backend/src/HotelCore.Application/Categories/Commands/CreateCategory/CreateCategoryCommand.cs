// This file contains code for CreateCategoryCommand.
using MediatR;
using Microsoft.AspNetCore.Http;
using HotelCore.Application.Categories.DTOs;

namespace HotelCore.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    IFormFile? IconFile,
    Guid? ParentId
) : IRequest<CategoryDto>;
