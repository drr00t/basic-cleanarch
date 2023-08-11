// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Stock.Business;

public record ProductUpdateDetail(string Name, string Description, float Weight, float Price, int Quantity);
public record ProductUpdate(Guid Id, string Name, string Description, float Weight, float Price, int Quantity);