module DB
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open FluentMigrator
open Dapper
open Microsoft.Data.Sqlite
open FluentMigrator.Runner

let connectionString() =
    match System.Environment.GetEnvironmentVariable("DATABASE") with
    | null
    | "" -> failwith "DATABASE connection string is not set"
    | value -> value

let migrate (services:IServiceCollection) =
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(fun runner ->
                runner
                    .AddSQLite()
                    .WithGlobalConnectionString(connectionString())
                    .ScanIn(typeof<Migrations.CreateProducts>.Assembly).For.Migrations() |> ignore)
            .AddLogging(fun logging -> logging.AddFluentMigratorConsole() |> ignore)
        |> ignore
        use serviceProvider = services.BuildServiceProvider()
        use scope = serviceProvider.CreateScope()
        let migrationRunner = ServiceProviderServiceExtensions.GetRequiredService<IMigrationRunner> scope.ServiceProvider
        migrationRunner.MigrateUp()

let openConnection() =
    use connection = new SqliteConnection(connectionString())
    connection.Open()
    connection

type ProductInputDTO={ Name:string }
type Product={ Id:int64; Name:string }

let makeProduct id (inputProduct:ProductInputDTO)=
    { Id=id; Name=inputProduct.Name}



type ProductService()=
    member _.list()=
        task{
            use conn=openConnection()
            return! conn.QueryAsync<Product> "Select * from Products" }

    member _.get id=
            task {
            use conn=openConnection()
            let! result=conn.QueryFirstOrDefaultAsync<Product>("Select * from Products where id=@id",{| id=id |})
            return Option.ofObj result }
    member _.add (input:ProductInputDTO)=
            task {
                use conn=openConnection()
                let! newId=conn.ExecuteAsync("INSERT INTO Products (Name) VALUES (@Name) returning id", input)
                return makeProduct newId input }
