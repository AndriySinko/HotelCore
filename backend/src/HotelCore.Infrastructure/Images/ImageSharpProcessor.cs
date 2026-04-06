using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using HotelCore.Application.Common.Images;
using HotelCore.Application.Common.Interfaces.Images;

namespace HotelCore.Infrastructure.Images;

public class ImageSharpProcessor(ILogger<ImageSharpProcessor> logger) : IImageProcessor
{
    private static readonly HashSet<string> SupportedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    ];

    public IReadOnlyList<ImageProcessingResult> Process(
        IFormFile file,
        IReadOnlyList<ImageSizeDefinition> sizeDefinitions,
        int quality = 75,
        double aspectRatioTolerance = 0.01)
    {
        logger.LogInformation("Processing image {FileName} with {SizeCount} size definitions", file.FileName, sizeDefinitions.Count);

        using var originalImage = LoadImage(file);
        var originalWidth = originalImage.Width;
        var originalHeight = originalImage.Height;
        var originalAspectRatio = (double)originalWidth / originalHeight;

        var results = new List<ImageProcessingResult>(sizeDefinitions.Count);

        foreach (var sizeDefinition in sizeDefinitions)
        {
            var constraint = sizeDefinition.EffectiveAspectRatio;
            var targetAspectRatio = constraint.GetTargetRatio(originalAspectRatio);
            var shouldCrop = Math.Abs(targetAspectRatio - originalAspectRatio) > aspectRatioTolerance;

            var (targetWidth, targetHeight) = CalculateTargetDimensions(
                originalWidth, 
                originalHeight, 
                targetAspectRatio, 
                sizeDefinition);

            var processedImage = originalImage.Clone(ctx =>
            {
                if (shouldCrop)
                {
                    var cropRect = CalculateCropRectangle(originalWidth, originalHeight, targetAspectRatio);
                    ctx.Crop(cropRect);
                }

                ctx.Resize(targetWidth, targetHeight);
            });

            var outputStream = new MemoryStream();
            processedImage.Save(outputStream, new JpegEncoder { Quality = quality });
            outputStream.Position = 0;

            results.Add(new ImageProcessingResult(
                sizeDefinition.Type,
                outputStream,
                targetWidth,
                targetHeight,
                targetAspectRatio,
                "image/jpeg"));

            processedImage.Dispose();
        }

        logger.LogInformation("Finished processing image {FileName}", file.FileName);
        return results;
    }

    public ImageDimensions? IdentifyDimensions(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var info = Image.Identify(stream);
            return info is null ? null : new ImageDimensions(info.Width, info.Height);
        }
        catch
        {
            return null;
        }
    }

    public bool IsValidImageFormat(IFormFile file)
    {
        if (!SupportedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            return false;

        return IdentifyDimensions(file) is not null;
    }

    private static Image LoadImage(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return Image.Load(stream);
    }

    private static Rectangle CalculateCropRectangle(int width, int height, double targetRatio)
    {
        var currentRatio = (double)width / height;

        if (Math.Abs(currentRatio - targetRatio) < 0.01)
            return new Rectangle(0, 0, width, height);

        int cropWidth, cropHeight, x, y;

        if (currentRatio > targetRatio)
        {
            cropHeight = height;
            cropWidth = (int)(height * targetRatio);
            x = (width - cropWidth) / 2;
            y = 0;
        }
        else
        {
            cropWidth = width;
            cropHeight = (int)(width / targetRatio);
            x = 0;
            y = (height - cropHeight) / 2;
        }

        return new Rectangle(x, y, cropWidth, cropHeight);
    }

    private static (int Width, int Height) CalculateTargetDimensions(
        int originalWidth,
        int originalHeight,
        double aspectRatio,
        ImageSizeDefinition sizeDefinition)
    {
        int targetWidth;
        int targetHeight;

        if (sizeDefinition.Width.HasValue && sizeDefinition.Height.HasValue)
        {
            targetWidth = sizeDefinition.Width.Value;
            targetHeight = sizeDefinition.Height.Value;
        }
        else if (sizeDefinition.Width.HasValue)
        {
            targetWidth = sizeDefinition.Width.Value;
            targetHeight = (int)(sizeDefinition.Width.Value / aspectRatio);
        }
        else if (sizeDefinition.Height.HasValue)
        {
            targetHeight = sizeDefinition.Height.Value;
            targetWidth = (int)(sizeDefinition.Height.Value * aspectRatio);
        }
        else
        {
            targetWidth = originalWidth;
            targetHeight = originalHeight;
        }

        targetWidth = Math.Min(targetWidth, originalWidth);
        targetHeight = Math.Min(targetHeight, originalHeight);

        return (Math.Max(1, targetWidth), Math.Max(1, targetHeight));
    }
}
