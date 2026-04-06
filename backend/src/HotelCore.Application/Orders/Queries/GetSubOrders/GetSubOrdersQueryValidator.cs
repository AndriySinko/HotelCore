using FluentValidation;

namespace HotelCore.Application.Orders.Queries.GetSubOrders;

public class GetSubOrdersQueryValidator : AbstractValidator<GetSubOrdersQuery>
{
    public GetSubOrdersQueryValidator()
    {
        RuleFor(x => x.ParentOrderId)
            .NotEmpty()
            .WithMessage("Parent order ID is required");
    }
}
