// Copyright (C) 2020  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.BusinessObjects;
using DFlow.Validation;

namespace Stock.Domain;

public class ProductName : ValueOf<string, ProductName>
{
    public static ProductName Empty
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
            ValidationStatus.Append(ProductNameInvalidFailure.Create());
        }
    }

    public class ProductNameInvalidFailure : Failure
    {
        public ProductNameInvalidFailure(string propertyName, string message) 
            : this(propertyName, message, String.Empty)
        {
        }
        public ProductNameInvalidFailure(string propertyName, string message, string value) 
            : base(propertyName, message, value)
        {
        }

        public static Failure Create()
        {
            return new ProductNameInvalidFailure("ProductName", 
                "O nome do produto é de preenchimento obrigatório.");
        }
    }
}