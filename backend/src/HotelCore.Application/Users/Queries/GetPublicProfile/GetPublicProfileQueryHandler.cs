namespace HotelCore.Application.Users.Queries.GetPublicProfile;

using MediatR;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Mappers;
using HotelCore.Application.Users;
using HotelCore.Application.Users.DTOs;

public class GetPublicProfileQueryHandler(
    IUserRepository userRepository,
    ILogger<GetPublicProfileQueryHandler> logger) 
    : IRequestHandler<GetPublicProfileQuery, PublicProfileDto?>
{
    public async Task<PublicProfileDto?> Handle(GetPublicProfileQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving public profile for user: {UserId}", request.Id);

        var (user, completedOrdersCount, createdOrdersCount) = await userRepository.GetPublicProfileAsync(request.Id, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} was not found for public profile", request.Id);
            return null;
        }

        // Count reviews and rating from profiles
        var reviewsCount = 0;
        decimal rating = 0;

        if (user.WorkerProfile != null)
        {
            reviewsCount += user.WorkerProfile.ReviewsCount;
            rating = user.WorkerProfile.Rating; // Usually we prioritze worker rating if they are both
        }
        else if (user.SeekerProfile != null)
        {
            reviewsCount += user.SeekerProfile.ReviewsCount;
            rating = user.SeekerProfile.Rating;
        }

        return new PublicProfileDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.CreatedAt,
            user.Avatar?.ToDto(),
            reviewsCount,
            completedOrdersCount,
            createdOrdersCount,
            rating
        );
    }
}
