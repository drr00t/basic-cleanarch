// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using DFlow.Persistence;
using DFlow.Validation;
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

    public Task<Result<Guid, IReadOnlyList<Failure>>> Execute(ProductCreate command)
    {
        return Execute(command, CancellationToken.None);
    }

    public async Task<Result<Guid, IReadOnlyList<Failure>>> 
        Execute(ProductCreate command, CancellationToken cancellationToken)
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
            
            return Succeded<Guid>.SucceedFor(product.Identity.Value);
        }

        return Failed<Guid>.FailedFor(product.Failures);
    }
}