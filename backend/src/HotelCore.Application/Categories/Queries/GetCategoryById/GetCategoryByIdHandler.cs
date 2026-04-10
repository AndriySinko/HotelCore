// This file contains code for GetCategoryByIdHandler.
using MediatR;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Categories.Queries.GetCategories;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Mappers;
using HotelCore.Domain.Entities.Categories;

namespace HotelCore.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler(ICategoryRepository repo)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Mode == CategoryLoadMode.Direct)
        {
            var entity = await repo.GetByIdAsync(
                request.Id,
                request.OverpassIsDeleteFilter,
                cancellationToken);

            return entity is null ? null : CategoryMapper.ToDtoNoChildren(entity);
        }

        var all = await repo.GetAllAsync(
            request.OverpassIsDeleteFilter,
            cancellationToken);
        var root = all.FirstOrDefault(x => x.Id == request.Id);
        if (root is null) return null;

        var lookup = CategoryMapper.BuildLookup(all);

        return CategoryMapper.BuildTree(root, lookup);
    }
}
 
