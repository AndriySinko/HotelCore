using MediatR;
using HotelCore.Application.Users.DTOs;

namespace HotelCore.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery() : IRequest<UserDto?>;
