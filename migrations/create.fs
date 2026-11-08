namespace Migrations

open Dapper
open FluentMigrator
open FluentMigrator.Runner

[<Migration(2026110901L,"create table of products")>]
type CreateProducts() =
    inherit Migration()

    override _.Up() =
        base.Execute.Sql """
            CREATE TABLE IF NOT EXISTS Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            )
        """

    override _.Down() =
        base.Delete.Table("Products") |> ignore
