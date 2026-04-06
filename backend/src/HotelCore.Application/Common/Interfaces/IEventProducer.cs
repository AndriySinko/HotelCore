namespace HotelCore.Application.Common.Interfaces;

public interface IEventProducer
{
    Task ProduceAsync<T>(
        T @event, 
        CancellationToken cancellationToken = default) where T : class;
}
