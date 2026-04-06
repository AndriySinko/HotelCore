using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Images;
using HotelCore.Application.Common.Interfaces.Images;
using HotelCore.Application.Common.Interfaces.Storage;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Enums;
using HotelCore.Domain.Exceptions;
using static HotelCore.Application.Common.Interfaces.Storage.StorageLocations;

namespace HotelCore.Infrastructure.Images;

public sealed class ImageService(
    IImageProcessor imageProcessor,
    IFileStorageService storageService,
    ILogger<ImageService> logger) : IImageService
{
    private const int DefaultQuality = 75;

    public async Task<MyImageGroup> SaveImageAsync(
        IFormFile file,
        MyImageType imageType,
        int position = 0,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Saving image {FileName} of type {ImageType} at position {Position}", file.FileName, imageType, position);

        var constraints = ImageSizeConfiguration.Get(imageType);
        ValidateFile(file, constraints);

        var sizeDefinitions = constraints.Sizes;
        var imageGroup = CreateImageGroup(file.FileName, imageType, position);
        var storageLocation = GetStorageLocation(imageType);
        var processedImages = imageProcessor.Process(file, sizeDefinitions, DefaultQuality);

        try
        {
            var uploadTasks = processedImages.Select(processed =>
                UploadAndCreateImage(processed, imageGroup, storageLocation, cancellationToken));

            var images = await Task.WhenAll(uploadTasks);

            foreach (var image in images)
            {
                imageGroup.Images.Add(image);
            }

            return imageGroup;
        }
        finally
        {
            foreach (var processed in processedImages)
            {
                processed.Stream.Dispose();
            }
        }
    }

    public async Task<IReadOnlyList<MyImageGroup>> SaveImagesAsync(
        IEnumerable<SaveImageRequest> requests,
        MyImageType imageType,
        CancellationToken cancellationToken = default)
    {
        var results = new List<MyImageGroup>();

        foreach (var request in requests)
        {
            var imageGroup = await SaveImageAsync(
                request.File,
                imageType,
                request.Position,
                cancellationToken);

            results.Add(imageGroup);
        }

        return results;
    }

    public async Task DeleteImageGroupAsync(
        MyImageGroup imageGroup,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting image group {ImageGroupId} of type {ImageType}", imageGroup.Id, imageGroup.Type);
        if (imageGroup?.Images is null || imageGroup.Images.Count == 0)
            return;

        var storageLocation = GetStorageLocation(imageGroup.Type);
        var keys = imageGroup.Images
            .Select(img => img.StorageKey)
            .ToList();

        await storageService.DeleteManyAsync(keys, storageLocation, cancellationToken);
        logger.LogInformation("Deleted image group {ImageGroupId} with {ImageCount} images from storage", imageGroup.Id, keys.Count);
    }

    public double GetAspectRatio(IFormFile file)
    {
        var dimensions = imageProcessor.IdentifyDimensions(file);
        if (dimensions is null) return -1;

        return (double)dimensions.Width / dimensions.Height;
    }

    public bool ValidateAspectRatioConsistency(IEnumerable<IFormFile> files, double tolerance = 0.001)
    {
        double? referenceRatio = null;

        foreach (var file in files)
        {
            var ratio = GetAspectRatio(file);

            if (ratio <= 0)
            {
                logger.LogWarning("File {FileName} has invalid aspect ratio {AspectRatio}", file.FileName, ratio);
                throw new BadRequestException("Invalid image format");
            }

            referenceRatio ??= ratio;

            if (Math.Abs(referenceRatio.Value - ratio) >= tolerance)
                return false;
        }

        return true;
    }

    private void ValidateFile(IFormFile file, ImageTypeConstraints constraints)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!ImageSizeConfiguration.AllowedExtensions.Contains(extension))
        {
            logger.LogWarning("File {FileName} has unsupported extension {Extension}", file.FileName, extension);
            throw new BadRequestException($"Image format '{extension}' is not allowed. Allowed formats: {string.Join(", ", ImageSizeConfiguration.AllowedExtensions)}");
        }

        var dimensions = imageProcessor.IdentifyDimensions(file);

        if (dimensions is null)
        {
            logger.LogWarning("File {FileName} could not be identified as a valid image", file.FileName);
            throw new BadRequestException("Invalid or corrupted image file");
        }

        if (dimensions.Width > constraints.MaxPixelDimension || dimensions.Height > constraints.MaxPixelDimension)
        {
            logger.LogWarning("File {FileName} has dimensions {Width}x{Height} which exceed the maximum allowed {MaxDimension}", file.FileName, dimensions.Width, dimensions.Height, constraints.MaxPixelDimension);
            throw new BadRequestException($"Image dimensions ({dimensions.Width}x{dimensions.Height}) exceed the maximum allowed ({constraints.MaxPixelDimension}x{constraints.MaxPixelDimension})");
        }
    }

    private static MyImageGroup CreateImageGroup(string fileName, MyImageType type, int position)
    {
        return new MyImageGroup
        {
            Title = Path.GetFileNameWithoutExtension(fileName),
            Type = type,
            Position = position
        };
    }

    private StorageLocation GetStorageLocation(MyImageType imageType)
    {
        var location = imageType switch
        {
            MyImageType.Order => StorageLocations.OrderImages,
            MyImageType.ProfilePicture => StorageLocations.ProfileImages,
            MyImageType.Project => StorageLocations.ProjectImages,
            MyImageType.Chat => StorageLocations.ChatImages,
            MyImageType.Certificate => StorageLocations.Certificates,
            _ => StorageLocations.Other
        };

        if (location == StorageLocations.Other)
        {
            logger.LogWarning("Unknown ImageType {ImageType}, using Other location", imageType);
        }

        return location;
    }

    private async Task<MyImage> UploadAndCreateImage(
        ImageProcessingResult processed,
        MyImageGroup imageGroup,
        StorageLocation storageLocation,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Uploading image for group {ImageGroupId} with size type {SizeType}", imageGroup.Id, processed.SizeType);
        var fileName = $"{imageGroup.Id}_{processed.SizeType.ToString().ToLowerInvariant()}.jpg";

        processed.Stream.Position = 0;

        var uploadResult = await storageService.UploadAsync(
            processed.Stream,
            fileName,
            processed.ContentType,
            storageLocation,
            cancellationToken);

        logger.LogInformation("Uploaded image for group {ImageGroupId} with size type {SizeType} to storage key {StorageKey}", imageGroup.Id, processed.SizeType, uploadResult.Key);

        return new MyImage
        {
            StorageKey = uploadResult.Key,
            Url = uploadResult.PublicUrl,
            Width = processed.Width,
            Height = processed.Height,
            SizeBytes = uploadResult.SizeBytes,
            Type = processed.SizeType,
            AspectRatio = processed.AspectRatio,
            ImageGroupId = imageGroup.Id,
            ImageGroup = imageGroup
        };
    }
}
