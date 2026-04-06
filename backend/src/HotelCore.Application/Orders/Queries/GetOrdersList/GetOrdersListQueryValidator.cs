using FluentValidation;

namespace HotelCore.Application.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryValidator : AbstractValidator<GetOrdersListQuery>
{
    public GetOrdersListQueryValidator()
    {
        RuleFor(x => x.Filter.Status)
            .IsInEnum()
            .When(x => x.Filter.Status.HasValue)
            .WithMessage("Invalid order status");

        RuleFor(x => x.Filter.SortBy)
            .IsInEnum()
            .WithMessage("Invalid sort field");
    }
}
