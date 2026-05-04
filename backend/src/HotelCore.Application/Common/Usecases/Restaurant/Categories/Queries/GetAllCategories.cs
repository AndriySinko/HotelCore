using MediatR;
using HotelCore.Application.Common.DTOs.Categories;

namespace HotelCore.Application.Common.Usecases.Restaurant.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<List<CategoryResponse>>;
