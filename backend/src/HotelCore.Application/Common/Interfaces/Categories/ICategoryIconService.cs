// This file contains code for ICategoryIconService.
using Microsoft.AspNetCore.Http;
using HotelCore.Application.Common.Interfaces.Storage;

namespace HotelCore.Application.Common.Interfaces.Categories;

public interface ICategoryIconService
{
    Task<FileUploadResult> UploadAsync(
        Guid categoryId,
        IFormFile file,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}
