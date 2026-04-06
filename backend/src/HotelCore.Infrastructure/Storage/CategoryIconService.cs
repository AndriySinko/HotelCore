using Microsoft.AspNetCore.Http;
using HotelCore.Application.Common.Interfaces.Categories;
using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Domain.Exceptions;

namespace HotelCore.Infrastructure.Storage;

public sealed class CategoryIconService(
    IFileStorageService fileStorageService,
    IImageProcessor imageProcessor) : ICategoryIconService
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<FileUploadResult> UploadAsync(
        Guid categoryId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ValidateFile(file);

        var extension = ResolveExtension(file);
        var fileName = $"{categoryId}/{Guid.NewGuid():N}{extension}";

        await using var stream = file.OpenReadStream();

        return await fileStorageService.UploadAsync(
            stream,
            fileName,
            file.ContentType,
            StorageLocations.Categories,
            cancellationToken);
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        return fileStorageService.DeleteAsync(
            storageKey,
            StorageLocations.Categories,
            cancellationToken);
    }

    private void ValidateFile(IFormFile file)
    {
        if (file.Length <= 0)
            throw new BadRequestException("Category icon file is empty.");

        if (file.Length > MaxFileSizeBytes)
            throw new BadRequestException("Category icon must not exceed 5 MB.");

        if (!imageProcessor.IsValidImageFormat(file))
            throw new BadRequestException("Unsupported category icon format.");
    }

    private static string ResolveExtension(IFormFile file)
    {
        var contentType = file.ContentType.ToLowerInvariant();

        return contentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            "image/gif" => ".gif",
            _ => Path.GetExtension(file.FileName).ToLowerInvariant()
        };
    }
}
