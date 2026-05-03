// This file contains code for MyImage.
using HotelCore.Domain.Enums;

namespace HotelCore.Domain.Entities.Images;

public class MyImage : BaseEntity
{
    public required string StorageKey { get; set; }
    public required string Url { get; set; }

    public required int Width { get; set; }
    public required int Height { get; set; }
    public required long SizeBytes { get; set; }

    public double AspectRatio { get; set; } = 1;

    public Product? Product { get; set; }
}
