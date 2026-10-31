namespace WebApplication1
open System
open Microsoft.Extensions.Hosting

type HelloService(env:IHostEnvironment)=
    member _.hello (prefix:string)=
        $"{prefix} app:{env.ContentRootPath} {DateTime.Now.ToString()}"

type ProductService()=
    member _.list()= task{ return DB.items }
    member _.get id= task { return DB.items|>Seq.filter(fun i->i.Id=id)|>Seq.tryExactlyOne }