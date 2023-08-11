// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using DFlow.BusinessObjects;
using DFlow.Validation;

namespace Stock.Domain;

public class ProductDescription : ValueOf<string, ProductDescription>
{
    public static ProductDescription Empty
    {
        get
        {
            return From(string.Empty);
        }
    }

    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value) ||
            string.IsNullOrWhiteSpace(Value))
        {
            ValidationStatus.Append(Failure
                .For("ProductName", "A descrição do produto é de preenchimento obrigatório."));
        }
    }
}