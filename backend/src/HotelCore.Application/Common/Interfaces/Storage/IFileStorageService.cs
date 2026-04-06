namespace HotelCore.Application.Common.Interfaces.Storage;

public interface IFileStorageService
{
    Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        StorageLocation location,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string key,
        StorageLocation location,
        CancellationToken cancellationToken = default);

    Task DeleteManyAsync(
        IEnumerable<string> keys,
        StorageLocation location,
        CancellationToken cancellationToken = default);
}
