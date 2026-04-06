using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Images;

public static class ImageSizeConfiguration
{
    public static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private static readonly Dictionary<MyImageType, ImageTypeConstraints> _configurations = new()
    {
        [MyImageType.Order] = new(
            Sizes:
            [
                new(ImageSizeType.Thumbnail, Width: 150, Height: 150, AspectRatio: AspectRatioConstraint.Fixed.Square),
                new(ImageSizeType.Small, Width: 300, Height: null),
                new(ImageSizeType.Medium, Width: 600, Height: null),
                new(ImageSizeType.Large, Width: 1200, Height: null)
            ],
            MaxPixelDimension: 8000),

        [MyImageType.ProfilePicture] = new(
            Sizes:
            [
                new(ImageSizeType.Thumbnail, Width: 100, Height: 100, AspectRatio: AspectRatioConstraint.Fixed.Square),
                new(ImageSizeType.Small, Width: 200, Height: 200, AspectRatio: AspectRatioConstraint.Fixed.Square),
                new(ImageSizeType.Medium, Width: 400, Height: 400, AspectRatio: AspectRatioConstraint.Fixed.Square)
            ],
            MaxPixelDimension: 4000),

        [MyImageType.Project] = new(
            Sizes:
            [
                new(ImageSizeType.Thumbnail, Width: 200, Height: 200, AspectRatio: AspectRatioConstraint.Fixed.Square),
                new(ImageSizeType.Small, Width: 400, Height: null),
                new(ImageSizeType.Medium, Width: 800, Height: null),
                new(ImageSizeType.Large, Width: 1600, Height: null)
            ],
            MaxPixelDimension: 8000),

        [MyImageType.Chat] = new(
            Sizes:
            [
                new(ImageSizeType.Thumbnail, Width: 100, Height: 100, AspectRatio: AspectRatioConstraint.Fixed.Square),
                new(ImageSizeType.Medium, Width: 500, Height: null)
            ],
            MaxPixelDimension: 6000),

        [MyImageType.Certificate] = new(
            Sizes:
            [
                new(ImageSizeType.Thumbnail, Width: 150, Height: null),
                new(ImageSizeType.Medium, Width: 600, Height: null),
                new(ImageSizeType.Large, Width: 1200, Height: null)
            ],
            MaxPixelDimension: 8000)
    };

    public static ImageTypeConstraints Get(MyImageType imageType)
    {
        return _configurations.TryGetValue(imageType, out var constraints)
            ? constraints
            : throw new ArgumentException($"No configuration found for {imageType}", nameof(imageType));
    }

    public static IReadOnlyDictionary<MyImageType, ImageTypeConstraints> All => _configurations;
}
