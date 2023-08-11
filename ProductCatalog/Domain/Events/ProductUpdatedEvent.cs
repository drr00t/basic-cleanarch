// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.Events;

namespace ProductCatalog.Domain.Events;

// https://medium.com/the-sixt-india-blog/primitive-obsession-code-smell-that-hurt-people-the-most-5cbdd70496e9

public class ProductUpdatedEvent : DomainEvent
{
    public ProductUpdatedEvent(ProductId id, ProductName name, ProductDescription description
        , ProductWeight weight, ProductPrice price, DateTimeOffset when)
        : base(when)
    {
        Id = id.Value;
        Name = name.Value;
        Description = description.Value;
        Weight = weight.Value;
        Price = price.Value;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public double Weight { get; }
    
    public double Price { get; }

    public static ProductUpdatedEvent For(Product product)
    {
        return new ProductUpdatedEvent(
            product.Identity, 
            product.Name,
            product.Description, 
            product.Weight,
            product.Price,
            DateTimeOffset.UtcNow);
    }
}