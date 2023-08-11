// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.AspNetCore.Builder;

namespace Stock.ApiEndpoints;

// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0
public static class StockHealthchecks
{
    public static void HealthChecksApis(WebApplication app)
    {
        app.MapHealthChecks("/healthz");
    }
}