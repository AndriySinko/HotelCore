using MediatR;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Categories.Queries.GetCategories;

namespace HotelCore.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(
    Guid Id,
    CategoryLoadMode Mode = CategoryLoadMode.Direct,
    bool OverpassIsDeleteFilter = false)
    : IRequest<CategoryDto?>;
