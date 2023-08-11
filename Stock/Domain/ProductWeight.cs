// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.BusinessObjects;
using DFlow.Validation;

namespace Stock.Domain;

public class ProductWeight : ValueOf<float, ProductWeight>
{
    public static ProductWeight Empty
    {
        get
        {
            return From(-1.0f);
        }
    }

    protected override void Validate()
    {
        if (Value <= 0)
        {
            ValidationStatus.Append(Failure
                .For("ProductWeight", $"O Peso {Value} informado não é valido."));
        }
    }
}