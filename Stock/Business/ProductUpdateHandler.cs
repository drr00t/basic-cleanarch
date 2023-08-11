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

public sealed class ProductUpdateHandler : ICommandHandler<ProductUpdate, Guid>
{
    private readonly IDbSession<IProductRepository> _sessionDb;

    public ProductUpdateHandler(IDbSession<IProductRepository> sessionDb)
    {
        this._sessionDb = sessionDb;
    }

    public Task<Result<Guid, IReadOnlyList<Failure>>> Execute(ProductUpdate command)
    {
        return Execute(command, CancellationToken.None);
    }

    public async Task<Result<Guid, IReadOnlyList<Failure>>> Execute(ProductUpdate command
        , CancellationToken cancellationToken)
    {
        var product = await this._sessionDb.Repository.GetById(ProductId.From(command.Id),cancellationToken);
        
        var updatedPproduct = Product.ReCreate(product);
        
        
        updatedPproduct.Update( ProductName.From(command.Name),
            ProductDescription.From(command.Description),
            ProductWeight.From(command.Weight)
            , ProductPrice.From(command.Price)
            , ProductQuantity.From(command.Quantity));

        if (updatedPproduct.IsValid)
        {
            return Succeded<Guid>.SucceedFor(updatedPproduct.Identity.Value);    
        }

        await this._sessionDb.Repository.Add(updatedPproduct);
        await this._sessionDb.SaveChangesAsync(cancellationToken);
        
        return Failed<Guid>.FailedFor(updatedPproduct.Failures);
        
    }
}