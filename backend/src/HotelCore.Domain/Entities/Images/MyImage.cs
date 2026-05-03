using HotelCore.Domain.Common;
using HotelCore.Domain.Entities.Users.Restaurant;

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
