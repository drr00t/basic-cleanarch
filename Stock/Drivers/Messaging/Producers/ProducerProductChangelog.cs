// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using DFlow.Validation;
using FluentResults;
using Microsoft.Extensions.Logging;
using Stock.Capabilities;
using Stock.Capabilities.Persistence.States;
using Stock.Capabilities.Supporting;
using Stock.Domain.Events.Exported;
using Stock.Drivers.Messaging.Extensions;

namespace Stock.Drivers.Messaging.Producers;

public class ProducerProductChangelog: BaseMessageProducer<AggregateState>
{
    private const string EcommerceTopicProductCreated = "STOCK_TOPIC_PRODUCT_CHANGELOG";
    private readonly ILogger<ProducerProductChangelog> _logger;
    
    public ProducerProductChangelog(IConfig config, ILogger<ProducerProductChangelog> logger)
    :base(config,EcommerceTopicProductCreated)
    {
        _logger = logger;
    }
    
    public override async Task<Result> Produce(AggregateState change, CancellationToken cancellationToken = default)
    {
        var outboxMessage = MessageFrom(change);
        var ok = false;
        try
        {
            using var schemaRegistry = new CachedSchemaRegistryClient(SchemaRegistryConfig);
            using var producer = new ProducerBuilder<string, ProductAggregate>(ProducerConfig)
                .SetValueSerializer(new ProtobufSerializer<ProductAggregate>(schemaRegistry))
                .Build();
            
            var dr = await producer
                .ProduceAsync(TopicDestination, outboxMessage, cancellationToken);
            _logger.LogInformation(message: "New topic offset from published message: {0}", dr.TopicPartitionOffset);
            ok = dr.Status == PersistenceStatus.Persisted;
        }
        catch (KafkaException ex)
        {
            _logger.LogError("Erro na publicação da mensagem ",ex);
            return Result.Fail(ex.Error.Reason);
        }
        
        return Result.Ok();
    }
    
    private Message<string, ProductAggregate> MessageFrom(AggregateState change)
    {
        var headerId = change.Id.ToByteArray();  // it's a good pratice to use a simple type as key, but a complexy
                                                     //type it's possible with a serializer registration
        var correlationRequestId = Guid.NewGuid().ToByteArray();
        var topicKey = change.AggregateId.ToString("D"); //product id
        var eventProcessingTimeMs = Timestamp.Default;
        
        return new Message<string, ProductAggregate>
        {
            Key = topicKey,
            Value = change.ToProtobuf(),
            //use hear to include a releavant informa for pre-processing
            Headers = new Headers {
                new Header("EVENT_ID" ,headerId), 
                new Header("CORRELATION_ID", correlationRequestId)
            }, 
            Timestamp = eventProcessingTimeMs
        };        
    }
}