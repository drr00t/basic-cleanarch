// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.BusinessObjects;
using Stock.Domain.Events;

namespace Stock.Domain.Entities;

public sealed class Product : EntityBase<ProductId>
{
    private Product(ProductId identity, ProductName name, ProductDescription description, ProductWeight weight,
        ProductPrice price, ProductQuantity quantity, VersionId version)
        : base(identity, version)
    {
        AppendValidationResult(identity.ValidationStatus.Failures);
        AppendValidationResult(description.ValidationStatus.Failures);
        AppendValidationResult(name.ValidationStatus.Failures);
        AppendValidationResult(weight.ValidationStatus.Failures);
        AppendValidationResult(price.ValidationStatus.Failures);
        AppendValidationResult(quantity.ValidationStatus.Failures);

        Description = description;
        Name = name;
        Weight = weight;
        Price = price;
        Quantity = quantity;
        
        if (IsValid && Version.IsNew)
        {
            RaisedEvent(ProductCreated.For(this));            
        }
    }

    public ProductName Name { get; private set; }
    public ProductWeight Weight { get;  private set;}
    
    public ProductQuantity Quantity { get;  private set;}
    public ProductDescription Description { get; private set; }

    public ProductPrice Price { get; private set; }

    public void UpdateDescription(ProductDescription description)
    {
        if (!description.ValidationStatus.IsValid)
        {
            AppendValidationResult(description.ValidationStatus.Failures);
            return;
        }

        Description = description;
        RaisedEvent(ProductUpdated.For(this));
    }
    
    public void UpdatePrice(ProductPrice price)
    {
        if (!price.ValidationStatus.IsValid)
        {
            AppendValidationResult(price.ValidationStatus.Failures);
            return;
        }

        Price = price;
        RaisedEvent(ProductUpdated.For(this));
    }
    
    public void Update(ProductName name, ProductDescription description, ProductWeight weight, ProductPrice price, ProductQuantity quantity)
    {
        if (!name.ValidationStatus.IsValid)
        {
            AppendValidationResult(name.ValidationStatus.Failures);
            return;
        }

        if (!description.ValidationStatus.IsValid)
        {
            AppendValidationResult(description.ValidationStatus.Failures);
            return;
        }
        
        if (!price.ValidationStatus.IsValid)
        {
            AppendValidationResult(price.ValidationStatus.Failures);
            return;
        }

        if (!weight.ValidationStatus.IsValid)
        {
            AppendValidationResult(weight.ValidationStatus.Failures);
            return;
        }

        if (!quantity.ValidationStatus.IsValid)
        {
            AppendValidationResult(quantity.ValidationStatus.Failures);
            return;
        }
        
        Name = name;
        Description = description;
        Weight = weight;
        Price = price;
        Quantity = quantity;
        RaisedEvent(ProductUpdated.For(this));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Identity;
        yield return Name;
        yield return Weight;
    }

    public static Product From(ProductId id, ProductName name, ProductDescription description
        , ProductWeight weight, ProductPrice price, ProductQuantity quantity, VersionId version)
    {
        return new Product(id, name, description, weight, price, quantity, version);
    }

    public static Product Create(ProductName name, ProductDescription description
        , ProductWeight weight, ProductPrice price, ProductQuantity quantity)
    {
        return From(ProductId.NewId(), name, description, weight, price, quantity,VersionId.New());
    }
    
    public static Product ReCreate(Product product)
    {
        return From(product.Identity,product.Name, product.Description, 
            product.Weight, product.Price, product.Quantity, VersionId.Next(product.Version));
    }
}