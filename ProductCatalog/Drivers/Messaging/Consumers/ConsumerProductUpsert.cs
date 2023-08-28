// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using DFlow.Validation;
using Microsoft.Extensions.Logging;
using ProductCatalog.Capabilities;
using ProductCatalog.Capabilities.Messaging;
using ProductCatalog.Capabilities.Persistence;
using ProductCatalog.Capabilities.Supporting;
using ProductCatalog.Drivers.Persistence;
using Stock.Domain.Events.Exported;

namespace ProductCatalog.Drivers.Messaging.Consumers;

public class ConsumerProductUpsert: IMessageConsumer
{
    private const string EcommerceBrokerEndpoints = "BROKERS_ENDPOINTS";
    private const string EcommerceTopicProductChangelog = "TOPIC_STOCK_PRODUCT_CHANGELOG";
    private const string ConsumerGroupProductChangelog = "ProductChangelog";

    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topicDestination;
    private readonly ILogger<ConsumerProductUpsert> _logger;
    private readonly IDao<ProductStock> _productDao;
    public ConsumerProductUpsert(IConfig config, IDao<ProductStock> productDao, ILogger<ConsumerProductUpsert> logger, IServiceProvider services)
    {
        _logger = logger;
        _productDao = productDao;
        var configBrokers = config.FromEnvironment(EcommerceBrokerEndpoints);
        var configTopicConsuming = config.FromEnvironment(EcommerceTopicProductChangelog);
        
      
        if (configBrokers.IsSucceded == false)
        {
            throw new ArgumentException(EcommerceBrokerEndpoints);
        }

        if (!configTopicConsuming.IsSucceded || string.IsNullOrEmpty(configTopicConsuming.Succeded))
        {
            throw new ArgumentException(EcommerceTopicProductChangelog);
        }

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configBrokers.Succeded,
            ClientId = $"{ConsumerGroupProductChangelog}-{Guid.NewGuid().ToString("N")}",
            GroupId = ConsumerGroupProductChangelog, // grupo de instancia até o limite de partições
            AutoOffsetReset = AutoOffsetReset.Earliest,  //começar da mensagem mais antiga
            
            // at-least once - let kafka cliente manage the commits, but I do control the offsetstore
            // https://docs.confluent.io/kafka-clients/dotnet/current/overview.html#store-offsets
            EnableAutoCommit = true, // commit é controlado pelo kafka cliente
            AutoCommitIntervalMs = 5000,  // update to kafka - pode estar alinhado com o volume de mensagens
            EnableAutoOffsetStore = false, // em falhas da aplicação sem processar mensagens o offsetstore não avança
                                            // a mensagem é reenviada
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky, // em caso de rebalance o broker 
                                                                                        // redistribuir para os membros
                                                                                        // desbalanciados
                                                                                        
            // we can call consumer.StoreOffset(result) to update last message processed
            //this gave to us a at-lest once garantee
            IsolationLevel = IsolationLevel.ReadCommitted // apenas ler mensagens gravadas, principalmente das que 
                                                        // em transações, enquanto não houver um commit da transçaão
                                                        // a mensagem não será recebida

        };

        _topicDestination = configTopicConsuming.Succeded;
    }

    private async Task<Result<bool, IReadOnlyList<Failure>>> ProcessMessage(
        Message<string, ProductAggregate> message, CancellationToken cancellationToken)
    {
        if(!cancellationToken.IsCancellationRequested)
        {
            var data = message.Value;

            _logger.LogInformation($"Mensagem recebida {data.EventType} {data.ProductCreated.Name}");
            
            var product = Populate(data);

            var prd = await _productDao.GetBy(product.ProductId);
            if (prd != null)
            {
                await _productDao.Insert(product);    
            }
            
            return Succeded<bool>.SucceedFor(true);
        }

        ProductStock Populate(ProductAggregate aggregate)
        {
            return aggregate.EventType switch
            {
                nameof(ProductCreatedEvent) => new ProductStock(
                        Guid.Parse(aggregate.ProductCreated.Id),
                    aggregate.ProductCreated.Name,
                    aggregate.ProductCreated.Description,
                    aggregate.ProductCreated.Weight,
                        aggregate.ProductCreated.Price,
                        aggregate.ProductCreated.Quantity
                        ),
                nameof(ProductUpdatedEvent) => new ProductStock( 
                    Guid.Parse(aggregate.ProductCreated.Id),
                    aggregate.ProductCreated.Name,
                    aggregate.ProductCreated.Description,
                    aggregate.ProductCreated.Weight,
                    aggregate.ProductCreated.Price,
                    aggregate.ProductCreated.Quantity
                ),
                null => throw new ArgumentException(nameof(aggregate))
            };
        } 
        
        return Failed<IReadOnlyList<Failure>>(Failure.For("",""));
    }
    

    public async Task<Result<bool, IReadOnlyList<Failure>>> Consume(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, ProductAggregate>(this._consumerConfig)
            .SetValueDeserializer(new ProtobufDeserializer<ProductAggregate>().AsSyncOverAsync())
            .SetErrorHandler((_, e) =>  _logger.LogError($"Error: {e.Reason}", e))
            .Build();

        consumer.Subscribe(_topicDestination);
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {

                var result = consumer.Consume(cancellationToken);

                if (result != null)
                {
                    var processed = await ProcessMessage(result.Message, cancellationToken);

                    if (processed.IsSucceded)
                    {
                        consumer.StoreOffset(result);
                        _logger.LogDebug(message: $"Atualizando offset @{result.TopicPartitionOffset}", result.TopicPartitionOffset);
                    }
                }
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError($"Atualizando offset {ex.Message}", ex);
            }
        }

        consumer.Close();

        return Succeded<bool>.SucceedFor(true);
    }

}