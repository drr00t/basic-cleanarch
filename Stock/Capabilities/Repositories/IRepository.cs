// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


namespace Stock.Capabilities.Repositories;

public interface IRepository<TEntity, in TIdentity>:IQueryEntity<TEntity, TIdentity>,
    IPersistEntity<TEntity>, IRemoveEntity<TEntity>
{

}