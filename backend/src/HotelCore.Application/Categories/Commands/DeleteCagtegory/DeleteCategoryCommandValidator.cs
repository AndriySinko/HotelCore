// This file contains code for DeleteCategoryCommandValidator.
using FluentValidation;

namespace HotelCore.Application.Categories.Commands.DeleteCagtegory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID cannot be empty");
    }
}
