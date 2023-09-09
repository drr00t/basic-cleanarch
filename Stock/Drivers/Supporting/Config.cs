// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using FluentResults;
using Stock.Capabilities.Supporting;

namespace Stock.Drivers.Supporting;

public class Config: IConfig
{
    public Result<string> FromEnvironment(string configKey)
    {
        var result =  Environment.GetEnvironmentVariable(configKey);

        if (!string.IsNullOrEmpty(result))
        {
            string value = result;
            return Result.Ok(value);
        }

        throw new ArgumentException(configKey);
    }
}