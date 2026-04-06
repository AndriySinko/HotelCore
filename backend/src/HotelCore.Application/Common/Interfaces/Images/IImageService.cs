using Microsoft.AspNetCore.Http;
using HotelCore.Domain.Entities.Images;
using HotelCore.Domain.Enums;

namespace HotelCore.Application.Common.Interfaces.Images;

public interface IImageService
{
    Task<MyImageGroup> SaveImageAsync(
        IFormFile file, 
        MyImageType imageType,
        int position = 0,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<MyImageGroup>> SaveImagesAsync(
        IEnumerable<SaveImageRequest> requests,
        MyImageType imageType,
        CancellationToken cancellationToken = default);
    
    Task DeleteImageGroupAsync(MyImageGroup imageGroup, CancellationToken cancellationToken = default);
    
    double GetAspectRatio(IFormFile file);
    
    bool ValidateAspectRatioConsistency(IEnumerable<IFormFile> files, double tolerance = 0.01);
}
