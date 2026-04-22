using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelCore.Application.Common.DTOs.Products;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Usecases.Restaurant.Products.Queries;

namespace HotelCore.Application.Common.Usecases.Restaurant.Products.QueryHandlers;

public class GetProductsListHandler(IApplicationDbContext db)
    : IRequestHandler<GetProductsListQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
    {
        var query = db.Products
            .Include(p => p.Category)
            .Include(p => p.Image)
            .AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.ProductCategoryId == request.CategoryId.Value);

        return await query
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.IsAvailable,
                p.Image != null ? p.Image.Url : null,
                p.ProductCategoryId,
                p.Category.Name))
            .ToListAsync(cancellationToken);
    }
}
