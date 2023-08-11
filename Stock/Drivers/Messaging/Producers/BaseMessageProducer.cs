// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Confluent.Kafka;
using Confluent.SchemaRegistry;
using DFlow.Validation;
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

        if (configBrokers.IsSucceded == false)
        {
            throw new ArgumentException(EcommerceBrokerEndpoints);
        }
        
        if (!configTopicPublishing.IsSucceded || string.IsNullOrEmpty(configTopicPublishing.Succeded))
        {
            throw new ArgumentException(ecommerceTopicProduct);
        }
        
        if (!schemaRegistryEndpoints.IsSucceded || string.IsNullOrEmpty(schemaRegistryEndpoints.Succeded))
        {
            throw new ArgumentException(SchemaRegistryEndpoints);
        }
        
        TopicDestination = configTopicPublishing.Succeded;
        
        SchemaRegistryConfig = new SchemaRegistryConfig()
        {
            Url = schemaRegistryEndpoints.Succeded
        };
        
        ProducerConfig = new ProducerConfig
        {
            BootstrapServers = configBrokers.Succeded,
            RequestTimeoutMs = 4000,
            RetryBackoffMs = 100, // número de tentativas de entrega
            MessageTimeoutMs = 5000, // timmeout para confirmação de entrega
            
            Acks = Acks.Leader, // can be set to All eleva confiabilidade 
            // EnableIdempotence = true, // propriedade inclui um ID único em cada mensagem, que pode contribuir
            // para semântica exactly-once 
            
            // Debug = "msg", // pode ser "all" em case de necessidade :)
        };
    }
    
    public abstract Task<Result<bool, IReadOnlyList<Failure>>> Produce(TValue change, CancellationToken cancellationToken);
}