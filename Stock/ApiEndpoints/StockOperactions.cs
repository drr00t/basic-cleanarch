// Copyright (C) 2022  Road to Agility
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock.Business;
using Stock.Capabilities.Operations;

namespace Stock.ApiEndpoints;

public static class StockOperactions
{
    public static void StateChangeApis(WebApplication app)
    {
        app.MapPost("/products", async ([FromBody] ProductCreate command
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
                command.Price,
                command.Quantity));
        
            if (result.IsSucceded == false)
            {
                return Results.BadRequest(result.Failed);
            }
        
            return Results.Ok(result.Succeded);
        });
    }
}