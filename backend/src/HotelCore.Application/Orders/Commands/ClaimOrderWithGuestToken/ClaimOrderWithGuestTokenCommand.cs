// This file contains code for ClaimOrderWithGuestTokenCommand.
using MediatR;

namespace HotelCore.Application.Orders.Commands.ClaimOrderWithGuestToken;

public record ClaimOrderWithGuestTokenCommand(
    string AccessToken,
    string GuestAccessToken
) : IRequest;
