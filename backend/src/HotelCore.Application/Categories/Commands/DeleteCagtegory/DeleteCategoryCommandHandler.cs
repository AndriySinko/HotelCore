using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Domain.Constants;
using HotelCore.Domain.Entities.Categories;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Categories.Commands.DeleteCagtegory;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork uow,
    ICategoryCache cache
) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = await categoryRepository.GetByIdAsync(request.Id, false, cancellationToken);
        if (entity is null)
            throw new NotFoundException(nameof(Category), request.Id);

        if (await categoryRepository.HasChildrenAsync(request.Id, cancellationToken))
            throw new ConflictException("Cannot delete category that has subcategories.", ErrorCodes.Category.HasChildren);

        categoryRepository.Delete(entity);
        await uow.SaveChangesAsync(cancellationToken);
        
        await cache.InvalidateAsync(cancellationToken);
    }
}
