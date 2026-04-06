using MediatR;
using HotelCore.Application.Users.DTOs;

namespace HotelCore.Application.Users.Queries.GetUsersList;

public record GetUsersListQuery(int Page = 1, int PageSize = 20) : IRequest<List<UserDto>>;