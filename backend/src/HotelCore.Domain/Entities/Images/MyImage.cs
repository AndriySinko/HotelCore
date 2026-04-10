// This file contains code for MyImage.
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Images;

public class MyImage
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string StorageKey { get; init; }
    public required string Url { get; init; }

    public required int Width { get; init; }
    public required int Height { get; init; }
    public required long SizeBytes { get; init; }

    public double AspectRatio { get; init; } = 1;
}
