// This file contains code for UpdateCategoryCommandHandler.
using MediatR;
using HotelCore.Application.Categories.DTOs;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICategoryCache categoryCache
) : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, false, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        if (request.ParentId == request.Id)
            throw new BadRequestException("Category cannot be its own parent.");

        if (request.ParentId is not null && !await categoryRepository.ExistsAsync(request.ParentId.Value, cancellationToken))
            throw new BadRequestException("Parent category not found.");

        await EnsureNoCircularParentAsync(entity.Id, request.ParentId, cancellationToken);

        entity.Name = request.Name.Trim();
        entity.Slug = request.Slug.Trim();
        entity.Description = request.Description?.Trim();
        entity.IconUrl = request.IconUrl?.Trim();
        entity.ParentId = request.ParentId;
        entity.UpdatedAt = DateTime.UtcNow;

        categoryRepository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await categoryCache.InvalidateAsync(cancellationToken);

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

    private async Task EnsureNoCircularParentAsync(Guid categoryId, Guid? newParentId, CancellationToken cancellationToken)
    {
        var currentId = newParentId;
        var visited = new HashSet<Guid>();

        while (currentId is not null)
        {
            var parentId = currentId.Value;
            if (!visited.Add(parentId)) break;
            if (parentId == categoryId)
                throw new BadRequestException("Parent category cannot be a descendant of the category.");

            var parent = await categoryRepository.GetByIdAsync(parentId, false, cancellationToken);
            currentId = parent?.ParentId;
        }
    }
}
