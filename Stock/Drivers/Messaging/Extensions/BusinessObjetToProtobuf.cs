// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Google.Protobuf.WellKnownTypes;
using Stock.Capabilities.Persistence.States;
using Stock.Domain.Events;
using Stock.Domain.Events.Exported;

namespace Stock.Drivers.Messaging.Extensions;

public static class BusinessObjetToProtobuf
{
    public static ProductAggregate ToProtobuf(this AggregateState stateChange)
    {
        var exported = new ProductAggregate
        {
            EventProcessingTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(), 
            EventType = stateChange.EventType
        };

        switch (stateChange.EventType)
        {
            case nameof(ProductCreated):
                exported.ProductCreated = new Domain.Events.Exported.ProductCreatedEvent
                {
                    Id = stateChange.EventData.RootElement.GetProperty("Id").GetString(),
                    Name = stateChange.EventData.RootElement.GetProperty("Name").GetString(),
                    Description = stateChange.EventData.RootElement.GetProperty("Description").GetString(),
                    Weight = stateChange.EventData.RootElement.GetProperty("Weight").GetDouble(),
                    Price = stateChange.EventData.RootElement.GetProperty("Price").GetDouble(),
                    EventTime = Timestamp.FromDateTimeOffset(stateChange.EventData.RootElement
                        .GetProperty("When").GetDateTimeOffset())
                };
                break;
            case nameof(ProductUpdated):
                exported.ProductUpdated = new Domain.Events.Exported.ProductUpdatedEvent
                {
                    Id = stateChange.EventData.RootElement.GetProperty("Id").GetString(),
                    Description = stateChange.EventData.RootElement.GetProperty("Description").GetString(),
                    Weight = stateChange.EventData.RootElement.GetProperty("Weight").GetDouble(),
                    Price = stateChange.EventData.RootElement.GetProperty("Price").GetDouble(),
                    EventTime = Timestamp.FromDateTimeOffset(stateChange.EventData.RootElement
                        .GetProperty("When").GetDateTimeOffset())
                };
                break;
        }

        return exported;

    }
    public static ProductAggregate ToProtobuf(this ProductCreated domainEvent)
    {
        return new ProductAggregate
        {
            EventProcessingTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(), 
            EventType = domainEvent.GetType().Name,
            ProductCreated = new Domain.Events.Exported.ProductCreatedEvent
            {
                Description = domainEvent.Description,
                Id = domainEvent.Id.ToString("D"),
                Name = domainEvent.Name,
                Weight = domainEvent.Weight,
                Price = domainEvent.Price,
                EventTime = Timestamp.FromDateTimeOffset(domainEvent.When)
            }
        };
        
    }

    public static ProductAggregate ToProtobuf(this ProductUpdated domainEvent)
    {
        return new ProductAggregate
        {
            EventProcessingTimeMs = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            EventType = domainEvent.GetType().Name,
            ProductUpdated = new Domain.Events.Exported.ProductUpdatedEvent()
            {
                Description = domainEvent.Description,
                Id = domainEvent.Id.ToString("D"),
                Weight = domainEvent.Weight,
                Price = domainEvent.Price,
                EventTime = Timestamp.FromDateTimeOffset(domainEvent.When)
            }
        };
    }
}
