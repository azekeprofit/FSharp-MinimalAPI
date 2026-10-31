module DB

type ProductInputDTO={ Name:string }
type Product={ Id:int; Name:string }

let makeProduct id (inputProduct:ProductInputDTO)=
    { Id=id; Name=inputProduct.Name}

let items=ResizeArray [
        { Id=0; Name="rope" }
        { Id=1; Name="lamp" }
        { Id=2; Name="oil" }
        { Id=3; Name="bottle" }
        { Id=4; Name="heart" }
        { Id=5; Name="triforce" }
        ]