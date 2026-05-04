using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Categories;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Categories.Queries;

namespace HotelCore.Application.Common.Usecases.Restaurant.Categories.QueryHandlers;

public class GetAllCategoriesHandler(IApplicationDbContext db)
    : IRequestHandler<GetAllCategoriesQuery, List<CategoryResponse>>
{
    public async Task<List<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await db.ProductCategories
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync(cancellationToken);
    }
}
