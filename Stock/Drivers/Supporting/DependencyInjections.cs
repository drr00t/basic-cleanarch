// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.Extensions.DependencyInjection;
using Stock.Capabilities.Supporting;

namespace Stock.Drivers.Supporting;

public static class DependencyInjections
{
    public static void AddSupporting(this IServiceCollection services)
    {
        services.AddScoped<IConfig, Config>();
    }
}