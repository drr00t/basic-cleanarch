// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Capabilities.Messaging;
using ProductCatalog.Capabilities.Supporting;
using ProductCatalog.Drivers.Messaging.Consumers;
using ProductCatalog.Drivers.Messaging.Services;
using ProductCatalog.Drivers.Supporting;

namespace ProductCatalog;

public static class DependencyInjections
{
    public static void AddSupporting(this IServiceCollection services)
    {
        services.AddSingleton<IConfig, ConfigFromEnvironment>();
    }
    
    
    public static void AddConsumers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageConsumer, ConsumerProductUpsert>();
        services.AddHostedService<ProductUpsertHostedService>();
    }
}