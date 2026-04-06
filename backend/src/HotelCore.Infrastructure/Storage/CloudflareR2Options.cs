namespace HotelCore.Infrastructure.Storage;

public class CloudflareR2Options
{
    public const string SectionName = "CloudflareR2";

    public required string AccountId { get; init; }
    public required string AccessKeyId { get; init; }
    public required string SecretAccessKey { get; init; }
    public required Dictionary<string, BucketConfig> Buckets { get; init; }

    public string Endpoint => $"https://{AccountId}.r2.cloudflarestorage.com";

    public BucketConfig GetBucket(string name) =>
        Buckets.TryGetValue(name, out var config)
            ? config
            : throw new InvalidOperationException($"Bucket '{name}' is not configured");
}

public class BucketConfig
{
    public required string Name { get; init; }

    private string _publicUrl = string.Empty;
    public required string PublicUrl
    {
        get => _publicUrl;
        init => _publicUrl = value.TrimEnd('/');
    }
}
