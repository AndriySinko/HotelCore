using MediatR;
using HotelCore.Application.Users.DTOs;

namespace HotelCore.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;