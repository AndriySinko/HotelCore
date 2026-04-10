// This file contains code for ICategoryRepository.
using HotelCore.Application.Categories.DTOs;
using HotelCore.Domain.Entities.Categories;

namespace HotelCore.Application.Common.Interfaces.Categories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<List<Category>> GetDirectChildrenAsync(Guid? parentId, bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default);
    Task<List<Category>> GetAllAsync(bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Category category, CancellationToken cancellationToken);
    void Update(Category category);
    Task<Category?> GetByIdAsync(Guid id, bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid requestParentId, CancellationToken cancellationToken);
}
