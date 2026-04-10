// This file contains code for PatchCategoryCommandValidator.
using System.Text.RegularExpressions;
using FluentValidation;

namespace HotelCore.Application.Categories.Commands.PatchCategory;

public sealed class PatchCategoryCommandValidator : AbstractValidator<PatchCategoryCommand>
{
    private static readonly Regex SlugRegex = new("^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.IgnoreCase);

    public PatchCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID cannot be empty");

        RuleFor(x => x.Name)
            .Must(name => name is null || !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name must not be empty when provided")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Slug)
            .Must(slug => slug is null || !string.IsNullOrWhiteSpace(slug))
            .WithMessage("Slug must not be empty when provided")
            .MaximumLength(150)
            .WithMessage("Slug must not exceed 150 characters")
            .Matches(SlugRegex)
            .When(x => !string.IsNullOrWhiteSpace(x.Slug))
            .WithMessage("Slug must contain only letters, numbers, and hyphens");

        RuleFor(x => x)
            .Must(x => !(x.RemoveIcon && x.IconFile is not null))
            .WithMessage("Cannot upload and remove category icon in the same request.");

        RuleFor(x => x)
            .Must(x => !(x.RemoveParent && x.ParentId.HasValue))
            .WithMessage("Cannot set and remove parent category in the same request.");

        RuleFor(x => x.IconFile)
            .Must(file => file is null || file.Length > 0)
            .WithMessage("Category icon file is empty.");
    }
}
