// This file contains code for GetCategoriesQuery.
using MediatR;
using HotelCore.Application.Categories.DTOs;

namespace HotelCore.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    Guid? ParentId,
    CategoryLoadMode Mode = CategoryLoadMode.Direct,
    bool OverpassIsDeleteFilter = false)
    : IRequest<List<CategoryDto>>;
