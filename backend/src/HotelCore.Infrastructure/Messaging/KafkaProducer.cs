using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HotelCore.Application.Common.Interfaces;

namespace HotelCore.Infrastructure.Messaging;

public class KafkaProducer : IEventProducer, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly string _topic = "user-created";

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;
        
        var connectionString = configuration.GetConnectionString("kafka");
        
        var config = new ProducerConfig
        {
            BootstrapServers = connectionString,
            ClientId = "workers-api-producer",
            Acks = Acks.All
        };

        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync<T>(
        T @event, 
        CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var value = JsonSerializer.Serialize(@event);
            
            // For simplicity, we hardcode the topic for now or derive it.
            // Since we only have one event:
            var message = new Message<Null, string>
            {
                Value = value
            };
            
            await _producer.ProduceAsync(
                _topic, 
                message, 
                cancellationToken);
            
            _logger.LogInformation("Produced event to topic {Topic}: {Event}", _topic, value);
        }
        catch (ProduceException<Null, string> ex)
        {
            _logger.LogError(ex, "Failed to produce message: {Reason}", ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}
