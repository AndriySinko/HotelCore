using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Mappers;
using HotelCore.Domain.Entities.Categories;

namespace HotelCore.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler(
    ICategoryRepository repo,
    ICategoryCache cache,
    ILogger<GetCategoriesQueryHandler> logger
) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<CategoryDto>? cached = null;

        try
        {
            cached = await cache.GetAsync(request.ParentId, request.Mode, request.OverpassIsDeleteFilter, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Category cache get failed");
        }

        if (cached is not null) return cached;

        List<CategoryDto> result;

        if (request.Mode == CategoryLoadMode.Direct)
        {
            var entities = await repo.GetDirectChildrenAsync(
                request.ParentId,
                request.OverpassIsDeleteFilter,
                cancellationToken);

            result = entities.Select(CategoryMapper.ToDtoNoChildren).ToList();
        }
        else
        {
            var all = await repo.GetAllAsync(
                request.OverpassIsDeleteFilter,
                cancellationToken);

            var lookup = CategoryMapper.BuildLookup(all);

            var rootParentId = request.ParentId;

            var roots = lookup[rootParentId];

            result = roots.Select(x => CategoryMapper.BuildTree(x, lookup)).ToList();
        }

        try
        {
            await cache.SetAsync(request.ParentId, request.Mode, request.OverpassIsDeleteFilter, result, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Category cache set failed");
        }

        return result;
    }

}
