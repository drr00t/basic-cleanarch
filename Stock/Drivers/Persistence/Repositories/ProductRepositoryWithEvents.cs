// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using System.Collections.Immutable;
using System.Linq.Expressions;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Stock.Capabilities.Messaging;
using Stock.Capabilities.Persistence.States;
using Stock.Capabilities.Repositories;
using Stock.Domain;
using Stock.Domain.Entities;
using Stock.Drivers.Persistence.ExtensionMethods;
using Stock.Drivers.Persistence.States;

namespace Stock.Drivers.Persistence.Repositories;

public class ProductRepositoryWithEvents : IProductRepository
{
    private readonly StockDbContext _dbContext;
    private readonly IMessageProducer<AggregateState> _messageProducer;

    public ProductRepositoryWithEvents(StockDbContext dbContext, 
        IMessageProducer<AggregateState> messageProducer)
    {
        _dbContext = dbContext;
        _messageProducer = messageProducer;
    }
    public async Task<Result> Add(Product entity)
    {
        var entry = entity.ToProductState();

        var cancel = new CancellationTokenSource();

        var oldState = await this._dbContext.Set<ProductState>()
            .AsNoTracking()
            .Where(e => e.Id.Equals(entity.Identity.Value))
            .FirstOrDefaultAsync(cancel.Token);

        if (oldState == null)
        {
            this._dbContext.Set<ProductState>().Add(entry);
        }
        else
        {
            var currentId = oldState.RowVersion + 1;
            if (currentId > entity.Version.Value)
            {
                return Result.Fail("This version is not the most updated for this object.");
            }

            _dbContext.Entry(oldState).CurrentValues.SetValues(entry);
        }

        await PublishChanges(entity, cancel.Token);
        
        return Result.Ok();
    }

    public async Task<Result> Remove(Product entity)
    {
        var cancel = new CancellationTokenSource();

        var oldState = await this._dbContext.Set<ProductState>()
            .AsNoTracking()
            .Where(e => e.Id.Equals(entity.Identity.Value))
            .FirstOrDefaultAsync(cancel.Token);

        if (oldState == null)
        {
            return Result.Fail($"O produto {entity.Name} com identificação {entity.Identity} não foi encontrado.");
        }

        var entry = entity.ToProductState();
        _dbContext.Set<ProductState>().Remove(entry);

        await PublishChanges(entity, cancel.Token);
        
        return Result.Ok();
    }

    public async Task<Result<Product>> GetBy(ProductId id, CancellationToken cancellation=default)
    {
        try
        {
            var result = await this._dbContext.Set<ProductState>()
                .Where(p => p.Id.Equals(id.Value)).AsNoTracking()
                .Select(t => t.ToProduct())
                .FirstAsync(cancellation);

            return Result.Ok(result);
        }
        catch(InvalidOperationException ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    private async Task PublishChanges(Product entity, CancellationToken cancel)
    {
        foreach(var change in entity.ToOutBox())
        {
            await _messageProducer.Produce(change, cancel);
        }
    }
}