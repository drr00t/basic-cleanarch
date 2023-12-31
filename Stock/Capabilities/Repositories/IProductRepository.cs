// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Stock.Domain;
using Stock.Domain.Entities;

namespace Stock.Capabilities.Repositories;

public interface IProductRepository: IRepository<Product, ProductId>
{
}