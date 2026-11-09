namespace WebApplication1
#nowarn "20"
open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http.HttpResults // Contains Ok<T>, NotFound, etc.


module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        builder.Services.AddAuthentication()

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        builder.Services.AddScoped<HelloService>()
        builder.Services.AddScoped<ProductService>()
        builder.Services.AddOpenApi() |> ignore

        DB.migrate builder.Services

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
        return Typed.Ok list }))
        |>ignore

        app.MapGet("/items/{id}",
            Func<int,ProductService, Task<Results<Ok<DB.Product>, NotFound>>>(fun id products ->
            task{
            let! item=products.get id
            return match item with
                    | Some item -> Typed.Ok item
                    | None      -> Typed.NotFound() }))
        |> ignore

        app.MapPut("/items",
            Func<DB.ProductInputDTO,ProductService, Task<Results<Ok<DB.Product>, _>>>(fun input products ->
            task{
            let! newProduct=products.add input
            return Typed.CreatedAt $"/items/{newProduct.Id}" }))
        |> ignore

        app.MapPost("/items",
            Func<DB.Product,ProductService, Task<Results<Ok,ProblemHttpResult>>>(fun input products ->
            task{
            let! result=products.update input
            return match result with
                    | 1 -> Typed.JustOk()
                    | other -> Typed.Problem(ProblemDetails(Detail=$"unexpected number of updated rows: {other}"))
                }))
        |> ignore

        app.Run()

        exitCode
