// This file contains code for RestoreCategoryCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Domain.Constants;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Categories.Commands.RestoreCategory;

public class RestoreCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ICategoryCache cache
) : IRequestHandler<RestoreCategoryCommand>
{
    public async Task Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, true, cancellationToken)
            ?? throw new NotFoundException(nameof(Category), request.Id);

        if (!entity.IsDeleted)
            throw new BadRequestException("Category is not deleted.");

        if (entity.ParentId is not null && !await categoryRepository.ExistsAsync(entity.ParentId.Value, cancellationToken))
            throw new BadRequestException("Parent category not found.");

        if (await categoryRepository.SlugExistsAsync(entity.Slug, entity.Id, cancellationToken))
            throw new ConflictException("Slug already exists.", ErrorCodes.Category.DuplicateSlug);

        entity.IsDeleted = false;
        entity.DeletedAt = null;
        entity.UpdatedAt = DateTime.UtcNow;

        categoryRepository.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await cache.InvalidateAsync(cancellationToken);
    }
}
