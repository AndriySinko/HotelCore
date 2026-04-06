using Microsoft.AspNetCore.Http;
using HotelCore.Application.Common.Images;

namespace HotelCore.Application.Common.Interfaces.Images;

public interface IImageProcessor
{
    IReadOnlyList<ImageProcessingResult> Process(
        IFormFile file, 
        IReadOnlyList<ImageSizeDefinition> sizeDefinitions,
        int quality = 80,
        double aspectRatioTolerance = 0.01);

    ImageDimensions? IdentifyDimensions(IFormFile file);

    bool IsValidImageFormat(IFormFile file);
}

public sealed record ImageDimensions(int Width, int Height);
