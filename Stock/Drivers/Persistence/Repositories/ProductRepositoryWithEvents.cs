// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using System.Collections.Immutable;
using System.Linq.Expressions;
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
    private readonly int _initialPageNumber = 1;
    private readonly int _recordPageSizeLimit = 20;
    private readonly IMessageProducer<AggregateState> _messageProducer;

    public ProductRepositoryWithEvents(StockDbContext dbContext, 
        IMessageProducer<AggregateState> messageProducer)
    {
        _dbContext = dbContext;
        _messageProducer = messageProducer;
    }
    public async Task Add(Product entity)
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
                throw new DbUpdateConcurrencyException("This version is not the most updated for this object.");
            }

            _dbContext.Entry(oldState).CurrentValues.SetValues(entry);
        }

        await PublishChanges(entity, cancel.Token);
    }

    public async Task Remove(Product entity)
    {
        var cancel = new CancellationTokenSource();

        var oldState = await this._dbContext.Set<ProductState>()
            .AsNoTracking()
            .Where(e => e.Id.Equals(entity.Identity.Value))
            .FirstOrDefaultAsync(cancel.Token);

        if (oldState == null)
        {
            throw new ArgumentException(
                $"O produto {entity.Name} com identificação {entity.Identity} não foi encontrado.");
        }

        var entry = entity.ToProductState();
        _dbContext.Set<ProductState>().Remove(entry);

        await PublishChanges(entity, cancel.Token);
    }

    public async Task<IReadOnlyList<Product>> FindAsync(Expression<Func<ProductState, bool>> predicate
        , CancellationToken cancellationToken)
    {
        return await FindAsync(predicate, this._initialPageNumber, this._recordPageSizeLimit, cancellationToken);
    }

    public async Task<Product> GetById(ProductId id, CancellationToken cancellation)
    {
        var result = await FindAsync(p => p.Id.Equals(id.Value), cancellation);

        return result.First();
    }

    public async Task<IReadOnlyList<Product>> FindAsync(Expression<Func<ProductState, bool>> predicate,
        int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        try
        {
            return await this._dbContext.Set<ProductState>()
                .Where(predicate).AsNoTracking()
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .Select(t => t.ToProduct())
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            return ImmutableList<Product>.Empty;
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