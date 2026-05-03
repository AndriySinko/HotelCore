// This file contains code for MoveGuestOrdersToAccountCommandHandler.
using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;

namespace HotelCore.Application.Orders.Commands.MoveGuestOrdersToAccount;

public class MoveGuestOrdersToAccountCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<MoveGuestOrdersToAccountCommand, MoveGuestOrdersToAccountResult>
{
    public async Task<MoveGuestOrdersToAccountResult> Handle(
        MoveGuestOrdersToAccountCommand request,
        CancellationToken cancellationToken)
    {
        var movedCount = await orderRepository.MoveGuestOrdersToUserAsync(
            request.GuestAccessToken,
            request.UserId,
            cancellationToken);

        if (movedCount > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new MoveGuestOrdersToAccountResult(movedCount);
    }
}
