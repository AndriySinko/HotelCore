using StackExchange.Redis;
using HotelCore.Application.EmailVerification.Interfaces;
using HotelCore.Application.EmailVerification.Models;

namespace HotelCore.Infrastructure.Persistence;

public sealed class RedisEmailVerificationStore(IConnectionMultiplexer redis) : IEmailVerificationStore
{
    private const string KeyPrefix = "email-verification:";
    private const string EmailField = "email";
    private const string UserIdField = "userId";
    private const string CodeField = "code";
    private const string TokenField = "token";
    private const string AttemptsField = "attempts";
    private const string CreatedAtField = "createdAt";
    private const string ExpiresAtField = "expiresAt";

    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<EmailVerificationRecord?> GetAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(email);
        var entries = await _db.HashGetAllAsync(key);
        if (entries.Length == 0)
        {
            return null;
        }

        var values = entries.ToDictionary(entry => entry.Name.ToString(), entry => entry.Value.ToString());
        if (!values.TryGetValue(EmailField, out var storedEmail)
            || !values.TryGetValue(CodeField, out var code)
            || !values.TryGetValue(TokenField, out var token)
            || !values.TryGetValue(AttemptsField, out var attemptsRaw)
            || !values.TryGetValue(CreatedAtField, out var createdRaw)
            || !values.TryGetValue(ExpiresAtField, out var expiresRaw))
        {
            return null;
        }

        if (!int.TryParse(attemptsRaw, out var attempts))
        {
            return null;
        }

        if (!long.TryParse(createdRaw, out var createdSeconds)
            || !long.TryParse(expiresRaw, out var expiresSeconds))
        {
            return null;
        }

        values.TryGetValue(UserIdField, out var userId);
        if (string.IsNullOrWhiteSpace(userId))
        {
            userId = null;
        }

        return new EmailVerificationRecord
        {
            Email = storedEmail,
            UserId = userId,
            Code = code,
            Token = token,
            Attempts = attempts,
            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(createdSeconds),
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresSeconds)
        };
    }

    public async Task SaveAsync(EmailVerificationRecord record, TimeSpan ttl, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(record.Email);
        var entries = new HashEntry[]
        {
            new(EmailField, record.Email),
            new(UserIdField, record.UserId ?? string.Empty),
            new(CodeField, record.Code),
            new(TokenField, record.Token),
            new(AttemptsField, record.Attempts),
            new(CreatedAtField, record.CreatedAt.ToUnixTimeSeconds()),
            new(ExpiresAtField, record.ExpiresAt.ToUnixTimeSeconds())
        };

        await _db.HashSetAsync(key, entries);
        await _db.KeyExpireAsync(key, ttl);
    }

    public Task<long> IncrementAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(email);
        return _db.HashIncrementAsync(key, AttemptsField);
    }

    public Task DeleteAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var key = BuildKey(email);
        return _db.KeyDeleteAsync(key);
    }

    private static string BuildKey(string email) =>
        $"{KeyPrefix}{NormalizeEmail(email)}";

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
