// This file contains code for PatchCategoryCommandHandler.
using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Domain.Constants;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Categories.Commands.PatchCategory;

public sealed class PatchCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICategoryCache categoryCache,
    ICategoryIconService categoryIconService,
    ILogger<PatchCategoryCommandHandler> logger
) : IRequestHandler<PatchCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(PatchCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, false, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        var oldIconStorageKey = entity.IconStorageKey;
        var categorySaved = false;
        FileUploadResult? uploadedIcon = null;

        try
        {
            var targetParentId = ResolveParentId(request, entity);
            if (targetParentId == entity.Id)
                throw new BadRequestException("Category cannot be its own parent.");

            if (targetParentId is not null && !await categoryRepository.ExistsAsync(targetParentId.Value, cancellationToken))
                throw new BadRequestException("Parent category not found.");

            await EnsureNoCircularParentAsync(entity.Id, targetParentId, cancellationToken);

            var targetSlug = entity.Slug;
            if (request.Slug is not null)
            {
                targetSlug = request.Slug.Trim().ToLowerInvariant();

                if (await categoryRepository.SlugExistsAsync(targetSlug, entity.Id, cancellationToken))
                    throw new ConflictException("Slug already exists.", ErrorCodes.Category.DuplicateSlug);
            }

            if (request.IconFile is not null)
            {
                uploadedIcon = await categoryIconService.UploadAsync(entity.Id, request.IconFile, cancellationToken);
            }

            if (request.Name is not null)
                entity.Name = request.Name.Trim();

            if (request.Slug is not null)
                entity.Slug = targetSlug;

            if (request.Description is not null)
                entity.Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim();

            entity.ParentId = targetParentId;

            if (request.RemoveIcon)
            {
                entity.IconUrl = null;
                entity.IconStorageKey = null;
            }

            if (uploadedIcon is not null)
            {
                entity.IconUrl = uploadedIcon.PublicUrl;
                entity.IconStorageKey = uploadedIcon.Key;
            }

            entity.UpdatedAt = DateTime.UtcNow;

            categoryRepository.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            categorySaved = true;
            await categoryCache.InvalidateAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldIconStorageKey) &&
                (request.RemoveIcon || uploadedIcon is not null) &&
                oldIconStorageKey != uploadedIcon?.Key)
            {
                try
                {
                    await categoryIconService.DeleteAsync(oldIconStorageKey, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(
                        ex,
                        "Failed to delete previous icon for category {CategoryId}. Storage key: {StorageKey}",
                        entity.Id,
                        oldIconStorageKey);
                }
            }

            return new CategoryDto(
                entity.Id,
                entity.Name,
                entity.Slug,
                entity.Description,
                entity.IconUrl,
                entity.ParentId,
                entity.IsDeleted,
                SubCategories: null
            );
        }
        catch
        {
            if (!categorySaved && uploadedIcon is not null)
                await categoryIconService.DeleteAsync(uploadedIcon.Key, cancellationToken);

            throw;
        }
    }

    private static Guid? ResolveParentId(PatchCategoryCommand request, Category entity)
    {
        if (request.RemoveParent)
            return null;

        return request.ParentId ?? entity.ParentId;
    }

    private async Task EnsureNoCircularParentAsync(Guid categoryId, Guid? newParentId, CancellationToken cancellationToken)
    {
        var currentId = newParentId;
        var visited = new HashSet<Guid>();

        while (currentId is not null)
        {
            var parentId = currentId.Value;
            if (!visited.Add(parentId))
                break;

            if (parentId == categoryId)
                throw new BadRequestException("Parent category cannot be a descendant of the category.");

            var parent = await categoryRepository.GetByIdAsync(parentId, false, cancellationToken);
            currentId = parent?.ParentId;
        }
    }
}
