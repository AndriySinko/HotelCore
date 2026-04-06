using FluentValidation;

namespace HotelCore.Application.Orders.Queries.GetOrderImages;

public class GetOrderImagesQueryValidator : AbstractValidator<GetOrderImagesQuery>
{
    public GetOrderImagesQueryValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");
    }
}
