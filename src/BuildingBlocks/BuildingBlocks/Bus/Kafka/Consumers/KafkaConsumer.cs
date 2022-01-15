using Ardalis.GuardClauses;
using Avro.Generic;
using BuildingBlocks.Bus.Kafka.SchemaRegistry;
using BuildingBlocks.Domain.Events;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Bus.Kafka.Consumers;

public class KafkaConsumer : IBusSubscriber
{
    private readonly KafkaConsumerConfig _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<KafkaConsumer> _logger;

    public KafkaConsumer(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaConsumer> logger)
    {
        _serviceScopeFactory = Guard.Against.Null(serviceScopeFactory, nameof(serviceScopeFactory));
        _logger = Guard.Against.Null(logger, nameof(logger));
        Guard.Against.Null(configuration, nameof(configuration));

        _config = configuration.GetKafkaConsumerConfig();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kafka consumer started");

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            // Note: you can specify more than one schema registry url using the
            // schema.registry.url property for redundancy (comma separated list).
            // The property name is not plural to follow the convention set by
            // the Java implementation.
            Url = _config.SchemaRegistryUrl
        };

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer = new ConsumerBuilder<string, GenericRecord>(_config)
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
            .SetValueDeserializer(new AvroDeserializer<GenericRecord>(schemaRegistry).AsSyncOverAsync())
            .Build();

        consumer.Subscribe(_config.Topics);

        try
        {
            while (cancellationToken.IsCancellationRequested == false)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(3));

                    if (result is null)
                        continue;

                    var fullSchemaName = result.Message.Value.Schema.SchemaName.Fullname;
                    var genericRecord = result.Message.Value;
                    var bytes = await genericRecord.SerializeAsync(schemaRegistry);
                    var @event = await _config.EventResolver?.Invoke(fullSchemaName, bytes, schemaRegistry)!;

                    _logger.LogInformation(
                        $"Received {result.Message?.Key!}-{result.Message?.Value?.GetType().FullName!} message.");
                    if (@event is INotification)
                    {
                        await mediator.Publish(@event, cancellationToken);
                        _logger.LogInformation($"Dispatched {@event.GetType()?.FullName} event to internal handler.");
                    }

                    consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    Console.Write(ex);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // commit final offsets and leave the group.
            consumer.Close();
        }

        consumer.Unsubscribe();
    }
}
