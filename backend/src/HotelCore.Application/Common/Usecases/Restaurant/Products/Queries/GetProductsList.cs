using MediatR;
using HotelCore.Application.Common.DTOs.Products;

namespace HotelCore.Application.Common.Usecases.Restaurant.Products.Queries;

public record GetProductsListQuery(Guid? CategoryId) : IRequest<List<ProductResponse>>;
