using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Categories.Queries.GetCategories;

namespace HotelCore.Application.Common.Interfaces.Categories;

public interface ICategoryCache
{
    Task<List<CategoryDto>?> GetAsync(Guid? parentId, CategoryLoadMode mode, bool overpassIsDeleteFilter, CancellationToken cancellationToken);
    Task SetAsync(Guid? parentId, CategoryLoadMode mode, bool overpassIsDeleteFilter, List<CategoryDto> data, CancellationToken cancellationToken);
    Task InvalidateAsync(CancellationToken cancellationToken);
}
