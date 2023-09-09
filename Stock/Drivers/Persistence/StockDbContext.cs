// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Microsoft.EntityFrameworkCore;
using NodaTime;
using Stock.Capabilities.Persistence.States;
using Stock.Capabilities.Supporting;
using Stock.Drivers.Persistence.Mappings;
using Stock.Drivers.Persistence.States;
using Stock.Persistence.Mappings;

namespace Stock.Drivers.Persistence;

public sealed class StockDbContext : DbContext
{
    private const string StockModelDatabase = "STOCK_MODEL_DATABASE";
    private readonly string _connectionString;
    
    public StockDbContext(IConfig config)
    {
        var result = config.FromEnvironment(StockModelDatabase);

        if (result.IsFailed)
        {
            throw new ArgumentException(StockModelDatabase);
        }

        _connectionString = result.Value;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString,o => o.UseNodaTime());
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ProductStateMapping().Configure(modelBuilder.Entity<ProductState>());
        new AggregateStateMapping().Configure(modelBuilder.Entity<AggregateState>());
    }
    
    public override int SaveChanges()
    {
        UpdateSoftDeleteLogic();
        return base.SaveChanges();
    }

    private void UpdateSoftDeleteLogic()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.CurrentValues["IsDeleted"] = true;
            }
            else
            {
                if (entry.State == EntityState.Added)
                {
                    entry.CurrentValues["CreatedAt"] = DateTimeOffset.Now;
                    entry.CurrentValues["UpdatedAt"] = DateTimeOffset.Now;
                }
                else
                {
                    entry.CurrentValues["UpdatedAt"] = DateTimeOffset.Now;
                }
                
                entry.CurrentValues["IsDeleted"] = false;
            }
        }
    }
}