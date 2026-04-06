using FluentValidation;
using HotelCore.Application.Common.Images;

namespace HotelCore.Application.Orders.Commands.AddOrderImages;

public class AddOrderImagesCommandValidator : AbstractValidator<AddOrderImagesCommand>
{
    public AddOrderImagesCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Order ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image is required")
            .Must(images => images.Count <= 20)
            .WithMessage("Cannot upload more than 20 images at once");

        RuleForEach(x => x.Images)
            .Must(file => file.Length > 0)
            .WithMessage("Image file cannot be empty")
            .Must(file =>
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return ImageSizeConfiguration.AllowedExtensions.Contains(extension);
            })
            .WithMessage($"Only the following image formats are allowed: {string.Join(", ", ImageSizeConfiguration.AllowedExtensions)}");
    }
}
