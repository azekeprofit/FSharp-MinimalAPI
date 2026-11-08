namespace WebApplication1
open System
open Microsoft.Extensions.Hosting

type HelloService(env:IHostEnvironment)=
    member _.hello (prefix:string)=
        $"{prefix} app:{env.ContentRootPath} {DateTime.Now.ToString()}"

type ProductService=DB.ProductService
