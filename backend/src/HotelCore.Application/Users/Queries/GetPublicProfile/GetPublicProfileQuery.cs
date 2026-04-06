namespace HotelCore.Application.Users.Queries.GetPublicProfile;

using MediatR;
using DTOs;

public record GetPublicProfileQuery(Guid Id) : IRequest<PublicProfileDto?>;
