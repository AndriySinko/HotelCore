using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using HotelCore.Application.Common.Interfaces.Storage;

namespace HotelCore.Infrastructure.Storage;

public class CloudflareR2StorageService : IFileStorageService, IDisposable
{
    private readonly AmazonS3Client _client;
    private readonly CloudflareR2Options _options;
    private bool _disposed;

    public CloudflareR2StorageService(IOptions<CloudflareR2Options> options)
    {
        _options = options.Value;

        var config = new AmazonS3Config
        {
            ServiceURL = _options.Endpoint,
            ForcePathStyle = true
        };

        _client = new AmazonS3Client(
            _options.AccessKeyId,
            _options.SecretAccessKey,
            config);
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        StorageLocation location,
        CancellationToken cancellationToken = default)
    {
        var bucket = _options.GetBucket(location.Bucket);
        var key = location.BuildKey(fileName);

        var streamLength = stream.Length;

        var request = new PutObjectRequest
        {
            BucketName = bucket.Name,
            Key = key,
            InputStream = stream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        var response = await _client.PutObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode is not HttpStatusCode.OK and not HttpStatusCode.Created)
        {
            throw new InvalidOperationException($"Failed to upload file '{key}' to bucket '{bucket.Name}'. Status: {response.HttpStatusCode}");
        }

        return new FileUploadResult(key, $"{bucket.PublicUrl}/{key}", streamLength);
    }

    public async Task DeleteAsync(
        string key,
        StorageLocation location,
        CancellationToken cancellationToken = default)
    {
        var bucket = _options.GetBucket(location.Bucket);

        var request = new DeleteObjectRequest
        {
            BucketName = bucket.Name,
            Key = key
        };

        await _client.DeleteObjectAsync(request, cancellationToken);
    }

    public async Task DeleteManyAsync(
        IEnumerable<string> keys,
        StorageLocation location,
        CancellationToken cancellationToken = default)
    {
        var keyList = keys.ToList();

        if (keyList.Count == 0)
            return;

        var bucket = _options.GetBucket(location.Bucket);

        var request = new DeleteObjectsRequest
        {
            BucketName = bucket.Name,
            Objects = keyList.Select(k => new KeyVersion { Key = k }).ToList()
        };

        await _client.DeleteObjectsAsync(request, cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _client.Dispose();
        _disposed = true;
    }
}
