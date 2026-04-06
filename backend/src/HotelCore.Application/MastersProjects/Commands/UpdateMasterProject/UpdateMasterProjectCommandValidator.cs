using FluentValidation;
using HotelCore.Application.Common.Images;

namespace HotelCore.Application.MastersProjects.Commands.UpdateMasterProject;

public class UpdateMasterProjectCommandValidator : AbstractValidator<UpdateMasterProjectCommand>
{
    public UpdateMasterProjectCommandValidator()
    {
        RuleFor(x => x.MasterProjectId)
            .NotEmpty()
            .WithMessage("Master project ID is required");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty()
            .WithMessage("Current user ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null)
            .WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Images)
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
