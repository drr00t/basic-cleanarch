// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.BusinessObjects;
using DFlow.Validation;

namespace Stock.Domain;

public class ProductQuantity : ValueOf<int, ProductQuantity>
{
    public static ProductQuantity Empty
    {
        get
        {
            return From(0);
        }
    }

    protected override void Validate()
    {
        if (Value < 0)
        {
            ValidationStatus.Append(Failure
                .For("ProductQuantity", $"A quantidade {Value} precisa ser igual ou mais que zero."));
        }
    }
}