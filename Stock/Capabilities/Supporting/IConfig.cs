// Copyright (C) 2023  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using DFlow.Validation;

namespace Stock.Capabilities.Supporting;

public interface IConfig
{
    Result<string,  IReadOnlyList<Failure>> FromEnvironment(string configKey);
}