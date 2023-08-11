// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.Extensions.DependencyInjection;
using Stock.Capabilities.Messaging;
using Stock.Capabilities.Persistence.States;
using Stock.Drivers.Messaging.Producers;

namespace Stock.Drivers.Messaging;

public static class DependencyInjections
{
    public static void AddProducers(this IServiceCollection services)
    {
        services.AddScoped<IMessageProducer<AggregateState>, ProducerProductChangelog>();
    }
}