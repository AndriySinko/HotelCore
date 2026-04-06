using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Images;

public record ImageProcessingResult(
    ImageSizeType SizeType,
    Stream Stream,
    int Width,
    int Height,
    double AspectRatio,
    string ContentType);
