// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.Extensions.DependencyInjection;
using Stock.Capabilities.Operations;

namespace Stock.Business;

public static class DependencyInjections
{
    public static void AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<ProductCreate, Guid>, ProductCreateHandler>();
        services.AddScoped<ICommandHandler<ProductUpdate, Guid>, ProductUpdateHandler>();
    }
}