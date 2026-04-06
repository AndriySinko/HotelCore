using MediatR;

namespace HotelCore.Application.Orders.Commands.GenerateOrderAccessToken;

public record GenerateOrderAccessTokenCommand(Guid OrderId) : IRequest<GenerateOrderAccessTokenResult>;

public record GenerateOrderAccessTokenResult(string AccessToken, DateTimeOffset ExpiresAt);
