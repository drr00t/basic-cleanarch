// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Confluent.Kafka;
using Confluent.SchemaRegistry;
using DFlow.Validation;
using FluentResults;
using Stock.Capabilities;
using Stock.Capabilities.Messaging;
using Stock.Capabilities.Supporting;

namespace Stock.Drivers.Messaging.Producers;

public abstract class BaseMessageProducer<TValue>: IMessageProducer<TValue> where TValue:class
{    
    private const string EcommerceBrokerEndpoints = "STOCK_BROKER_ENDPOINTS";
    private const string SchemaRegistryEndpoints = "SCHEMA_REGISTRY_ENDPOINTS";
    protected ProducerConfig ProducerConfig { get; }
    
    protected SchemaRegistryConfig SchemaRegistryConfig { get; }
    
    protected string TopicDestination { get; }

    protected BaseMessageProducer(IConfig config, string ecommerceTopicProduct)
    {
        var configBrokers = config.FromEnvironment(EcommerceBrokerEndpoints);
        var configTopicPublishing = config.FromEnvironment(ecommerceTopicProduct);
        var schemaRegistryEndpoints = config.FromEnvironment(SchemaRegistryEndpoints);

        if (configBrokers.IsFailed)
        {
            throw new ArgumentException(EcommerceBrokerEndpoints);
        }
        
        if (!configTopicPublishing.IsSuccess || string.IsNullOrEmpty(configTopicPublishing.Value))
        {
            throw new ArgumentException(ecommerceTopicProduct);
        }
        
        if (!schemaRegistryEndpoints.IsSuccess || string.IsNullOrEmpty(schemaRegistryEndpoints.Value))
        {
            throw new ArgumentException(SchemaRegistryEndpoints);
        }
        
        TopicDestination = configTopicPublishing.Value;
        
        SchemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = schemaRegistryEndpoints.Value
        };
        
        ProducerConfig = new ProducerConfig
        {
            BootstrapServers = configBrokers.Value,
            RequestTimeoutMs = 4000,
            RetryBackoffMs = 100, // número de tentativas de entrega
            MessageTimeoutMs = 5000, // timmeout para confirmação de entrega
            
            Acks = Acks.Leader, // can be set to All eleva confiabilidade 
            // EnableIdempotence = true, // propriedade inclui um ID único em cada mensagem, que pode contribuir
            // para semântica exactly-once 
            
            // Debug = "msg", // pode ser "all" em case de necessidade :)
        };
    }
    
    public abstract Task<Result> Produce(TValue change, CancellationToken cancellationToken=default);
}