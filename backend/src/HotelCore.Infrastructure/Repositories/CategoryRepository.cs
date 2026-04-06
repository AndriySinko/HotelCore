using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Infrastructure.Persistence;

namespace HotelCore.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext db) :BaseRepository<Category>(db), ICategoryRepository
{
    private IQueryable<Category> GetCategoriesQuery(bool overpassIsDeleteFilter)
    {
        var query = db.Categories.AsQueryable();
        return overpassIsDeleteFilter ? query.IgnoreQueryFilters() : query;
    }
    public async Task<Category?> GetByIdAsync(Guid id, bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default)
    {
        return await GetCategoriesQuery(overpassIsDeleteFilter)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<List<Category>> GetDirectChildrenAsync(Guid? parentId, bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default)
    {
        return await GetCategoriesQuery(overpassIsDeleteFilter)
            .AsNoTracking()
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Category>> GetAllAsync(bool overpassIsDeleteFilter = false, CancellationToken cancellationToken = default)
    {
        return await GetCategoriesQuery(overpassIsDeleteFilter)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return db.Categories.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken)
    {
        return db.Categories.AnyAsync(x => x.ParentId == id, cancellationToken);
    }

    public Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken cancellationToken)
    {
       
        var normalized = slug.Trim().ToLowerInvariant();

        return db.Categories.AnyAsync(x =>
            x.Slug == normalized &&
            (excludeId == null || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await db.Categories.AddAsync(category, cancellationToken);
    }

    public void Update(Category category)
        => db.Categories.Update(category);
}
