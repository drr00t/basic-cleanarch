// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using FluentResults;

namespace Stock.Capabilities.Repositories;

public interface IRemoveEntity<in TEntity>
{
    Task<Result> Remove(TEntity entity);
}