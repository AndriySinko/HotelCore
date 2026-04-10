// This file contains code for MoveGuestOrdersToAccountCommand.
using MediatR;

namespace HotelCore.Application.Orders.Commands.MoveGuestOrdersToAccount;

public record MoveGuestOrdersToAccountCommand(
    string GuestAccessToken,
    Guid UserId
) : IRequest<MoveGuestOrdersToAccountResult>;

public record MoveGuestOrdersToAccountResult(int MovedCount);
