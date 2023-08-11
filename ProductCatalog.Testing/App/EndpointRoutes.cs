// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.AspNetCore.Mvc;
using Stock.Business;
using Stock.Capabilities.Querying.Views;

namespace Stock.RestAPI.ApiEndpoints;

public static class EndpointRoutes
{
    public static void StateChangeApis(WebApplication app)
    {
        app.MapPost("/api/v1/products", async ([FromBody] ProductCreate command
            , [FromServices]ICommandHandler<ProductCreate, Guid> handler) =>
        {
            var result = await handler.Execute(command);

            if (result.IsSucceded == false)
            {
                return Results.BadRequest(result.Failed);
            }

            return Results.Ok(result);
        });

        app.MapPut("/products/{productId:guid}", async ([FromRoute]Guid productId, 
            [FromBody] ProductUpdateDetail command,
            [FromServices] ICommandHandler<ProductUpdate, Guid> handler) =>
        {
            var result = await handler.Execute(new ProductUpdate(
                productId, 
                command.Name,
                command.Description, 
                command.Weight,
                command.Price));
        
            if (result.IsSucceded == false)
            {
                return Results.BadRequest(result.Failed);
            }
        
            return Results.Ok(result.Succeded);
        });
        
        app.MapGet("/api/v1/products", async (
            [FromServices] IQueryHandler<ProductList, IReadOnlyList<ProductView>> handler) =>
        {
            var result = await handler.Execute(new ProductList("", "", 1, 10));
            if (result.IsSucceded == false)
            {
                return Results.BadRequest(result.Failed);
                
            }

            return Results.Ok(result.Succeded);
        });
    }
}