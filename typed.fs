namespace WebApplication1
open Microsoft.AspNetCore.Http

module Typed =
    /// Converts a concrete result type (like Ok or NotFound) into its Results<...> union struct wrapper
    let inline from (value: 'T) : 'Results =
        (^Results : (static member op_Implicit : 'T -> 'Results) (value))

    let inline Ok v=from(TypedResults.Ok v)
    let inline JustOk()=from(TypedResults.Ok())
    let inline Problem details=from(TypedResults.Problem details)
    let inline NotFound()=from(TypedResults.NotFound())
    let inline NoContent()=from(TypedResults.NoContent())
    let inline Forbid()=from(TypedResults.Forbid())
    let inline Accepted (url:string)=from(TypedResults.Accepted url)
    let inline AcceptedUri (uri:System.Uri)=from(TypedResults.Accepted uri)
    let inline Created()=from(TypedResults.Created())
    let inline CreatedAt(url:string)=from(TypedResults.Created url)
    // let inline CreatedAtRoute (route:string) (values:System.Collections.Generic.IDictionary<string,string>)=from(TypedResults.CreatedAtRoute(route,Microsoft.AspNetCore.Routing.RouteValueDictionary(values)))
    let inline Redirect url=from(TypedResults.Redirect url)
    let inline RedirectToRoute route=from(TypedResults.RedirectToRoute route)
    let inline BadRequest()=from(TypedResults.BadRequest())
    let inline Bytes (b:byte array)=from(TypedResults.Bytes b)
    let inline Unauthorized ()=from(TypedResults.Unauthorized())
