using System.Buffers.Text;
using System.Security.Cryptography;
using StackExchange.Redis;
using HotelCore.Application.Common.Interfaces.Orders;

namespace HotelCore.Infrastructure.Caching;

public class OrdersTemporaryAccessStore(IConnectionMultiplexer redis) : IOrdersTemporaryAccessStore
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string KeyPrefix = "orders:temp-access:";

    public async Task<string> CreateAccessTokenAsync(Guid orderId, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        var accessToken = GenerateSecureToken();

        await _db.StringSetAsync(GetKey(accessToken), orderId.ToString(), expiration);

        return accessToken;
    }

    public async Task<Guid?> GetOrderIdAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var value = await _db.StringGetAsync(GetKey(accessToken));

        return !value.IsNullOrEmpty && Guid.TryParse(value.ToString(), out var orderId)
            ? orderId
            : null;
    }

    public async Task DeleteAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        await _db.KeyDeleteAsync(GetKey(accessToken));
    }

    private static string GetKey(string accessToken) => $"{KeyPrefix}{accessToken}";

    private static string GenerateSecureToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64Url.EncodeToString(bytes);
    }
}
