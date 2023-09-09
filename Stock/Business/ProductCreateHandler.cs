// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.Persistence;
using DFlow.Validation;
using FluentResults;
using Stock.Capabilities;
using Stock.Capabilities.Operations;
using Stock.Capabilities.Repositories;
using Stock.Domain;
using Stock.Domain.Entities;

namespace Stock.Business;

public sealed class ProductCreateHandler : ICommandHandler<ProductCreate, Guid>
{
    private readonly IDbSession<IProductRepository> _sessionDb;

    public ProductCreateHandler(IDbSession<IProductRepository> sessionDb)
    {
        this._sessionDb = sessionDb;
    }

    public async Task<Result<Guid>> Execute(ProductCreate command, CancellationToken cancellationToken=default)
    {
        var product = Product.Create(ProductName.From(command.Name),
                                ProductDescription.From(command.Description),
                                ProductWeight.From(command.Weight),
                                ProductPrice.From(command.Price),
                                ProductQuantity.From(command.Quantity)
                                );
        
        if (product.IsValid)
        {
            await this._sessionDb.Repository.Add(product);
            await this._sessionDb.SaveChangesAsync(cancellationToken);
            
            return Result.Ok(product.Identity.Value);
        }

        return Result.Fail(product.Failures.Select( flr => new Error(flr.Message)));
    }
}