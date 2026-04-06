using MediatR;
using HotelCore.Application.Common.Interfaces;
using HotelCore.Application.Common.Interfaces.Orders;
using HotelCore.Domain.Entities.Orders;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Application.Orders.Commands.CreateSubOrder;

public class CreateSubOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrderAccessPolicy accessPolicy,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSubOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateSubOrderCommand request, CancellationToken cancellationToken)
    {
        var parent = await orderRepository.GetByIdAsync(request.ParentOrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.ParentOrderId);

        if (!accessPolicy.IsOwner(parent, request.CurrentUserId))
            throw new ForbiddenException("You do not have permission to perform this action");

        if (parent.Type != OrderType.Regular)
            throw new InvalidOperationException("Sub-orders cannot be nested under another sub-order");

        var subOrder = new Order
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            PaymentType = request.PaymentType ?? OrderPaymentType.Prepay,
            Status = OrderStatus.Draft,
            Type = OrderType.Sub,
            ParentOrderId = parent.Id,
            CreatedByUserId = request.CurrentUserId
        };

        orderRepository.Add(subOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return subOrder.Id;
    }
}
