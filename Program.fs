namespace WebApplication1
#nowarn "20"
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.HttpResults // Contains Ok<T>, NotFound, etc.
open Swashbuckle.AspNetCore.SwaggerUI
open Swashbuckle.AspNetCore

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddAuthentication();

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        builder.Services.AddScoped<HelloService>()
        builder.Services.AddScoped<ProductService>()
        builder.Services.AddOpenApi() |> ignore   

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.MapOpenApi() |> ignore                   // serves /openapi/v1.json
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        
        app.MapGet("/mini",Func<string>(fun ()->"srtrtrt"))|>ignore
        app.MapGet("/boo/{par}",Func<string,HelloService,string>(fun par hello ->hello.hello par))|>ignore
        app.MapGet("/products",Func<ProductService,Task<Results<Ok<DB.Product list>,_>>>(fun products ->
        task{
            let! list=products.list()
            return from(TypedResults.Ok(list))
            }))
        |>ignore

        
        app.MapGet("/items/{id}",
            Func<int,ProductService, Task<Results<Ok<DB.Product>, NotFound>>>(fun id products ->
                task{
                    let! item=products.get id 
                    match item with
                    | Some item -> return from(TypedResults.Ok item)
                    | None      -> return from(TypedResults.NotFound())
                }))
        |> ignore

        
        app.MapPut("/items",
            Func<DB.ProductInputDTO,ProductService, Task<Results<Ok<DB.Product>, _>>>(fun input products ->
                task{
                    let! newProduct=products.add input
                    return from(TypedResults.Ok(newProduct))
                }))
        |> ignore


        app.Run()

        exitCode