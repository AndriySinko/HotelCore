using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Mappers;
using HotelCore.Application.Users;
using HotelCore.Application.Users.DTOs;

namespace HotelCore.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    public async Task<UserDto?> Handle(
        GetCurrentUserQuery request, 
        CancellationToken cancellationToken)
    {
        var userIdString = currentUserService.UserId;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return null;
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
            return null;

        return new UserDto(
            user.Id,
            user.Email ?? string.Empty,
            user.PhoneNumber ?? string.Empty,
            user.FirstName,
            user.LastName,
            user.Role,
            user.EmailConfirmed,
            user.PhoneNumberConfirmed,
            user.Avatar?.ToDto(),
            user.SeekerProfile != null ? new Profiles.DTOs.SeekerProfileDto(user.SeekerProfile.Id, user.SeekerProfile.Bio, user.SeekerProfile.Rating, user.SeekerProfile.ReviewsCount, user.SeekerProfile.IsVerified, user.SeekerProfile.DefaultLocationId) : null,
            user.WorkerProfile != null ? new Profiles.DTOs.WorkerProfileDto(user.WorkerProfile.Id, user.WorkerProfile.Bio, user.WorkerProfile.Website, user.WorkerProfile.Rating, user.WorkerProfile.ReviewsCount, user.WorkerProfile.IsVerified, user.WorkerProfile.Tags?.ToArray() ?? [], user.WorkerProfile.LocationId) : null
        );
    }
}
