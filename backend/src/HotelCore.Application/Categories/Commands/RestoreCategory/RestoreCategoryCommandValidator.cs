// This file contains code for RestoreCategoryCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Categories.Commands.RestoreCategory;

public class RestoreCategoryCommandValidator : AbstractValidator<RestoreCategoryCommand>
{
    public RestoreCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID cannot be empty");
    }
}
