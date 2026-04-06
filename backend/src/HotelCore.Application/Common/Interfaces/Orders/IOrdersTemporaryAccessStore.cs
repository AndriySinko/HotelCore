namespace HotelCore.Application.Common.Interfaces.Orders;

public interface IOrdersTemporaryAccessStore
{
    Task<string> CreateAccessTokenAsync(Guid orderId, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task<Guid?> GetOrderIdAsync(string accessToken, CancellationToken cancellationToken = default);
    Task DeleteAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);
}
