namespace WebApplication1

[<AutoOpen>]
module ResultsExtensions =
    /// Converts a concrete result type (like Ok or NotFound) into its Results<...> union struct wrapper
    let inline from (value: 'T) : 'Results = 
        (^Results : (static member op_Implicit : 'T -> 'Results) (value))
