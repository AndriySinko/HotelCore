using MediatR;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Domain.Constants;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    ICategoryRepository repo,
    IUnitOfWork uow,
    ICategoryCache cache,
    ICategoryIconService categoryIconService
) : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request, 
        CancellationToken cancellationToken = default)
    {
        if (request.ParentId is not null && !await repo.ExistsAsync(request.ParentId.Value, cancellationToken))
        {
            throw new BadRequestException("Parent category not found.");
        }

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
        if (await repo.SlugExistsAsync(normalizedSlug, null, cancellationToken))
            throw new ConflictException("Slug already exists.", ErrorCodes.Category.DuplicateSlug);

        var categoryId = Guid.NewGuid();
        var categorySaved = false;
        FileUploadResult? uploadedIcon = null;

        try
        {
            if (request.IconFile is not null)
            {
                uploadedIcon = await categoryIconService.UploadAsync(
                    categoryId,
                    request.IconFile,
                    cancellationToken);
            }

            var category = new Category
            {
                Id = categoryId,
                Name = request.Name.Trim(),
                Slug = normalizedSlug,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IconUrl = uploadedIcon?.PublicUrl,
                IconStorageKey = uploadedIcon?.Key,
                ParentId = request.ParentId,
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(category, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);
            categorySaved = true;
            await cache.InvalidateAsync(cancellationToken);

            return new CategoryDto(
                category.Id,
                category.Name,
                category.Slug,
                category.Description,
                category.IconUrl,
                category.ParentId,
                category.IsDeleted,
                SubCategories: null
            );
        }
        catch
        {
            if (!categorySaved && uploadedIcon is not null)
            {
                await categoryIconService.DeleteAsync(uploadedIcon.Key, cancellationToken);
            }

            throw;
        }
    }
}
