(***************************************************************** 
 * Iterator (page 257)
 *   aka Cursor
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Iterator

open System.Collections.Generic


///Iterator .net Sytle
let [<Literal>] DEFAULT_LIST_CAPACITY = 10

type MyList<'T> (?optSize:int) =
    let size = defaultArg optSize DEFAULT_LIST_CAPACITY
    let store = ResizeArray<'T>(size)
    member _.Count with get () = store.Count
    member _.Item with get index = store.[index]
    interface seq<'T> with
        member _.GetEnumerator () =
            // This object expression implements the IEnumerator<'T> interface
            // without making a new named type
            let mutable current = 0;
            { 
                new IEnumerator<'T> with
                   member this.Current with get (): 'T = 
                        if store.Count > current then
                            store.[current]
                        else
                            failwith "Enumerator Out Of Bounds"
                    member this.Current with get(): obj = 
                        this.Current
                    member _.Dispose(): unit = ()
                    member _.MoveNext(): bool = 
                        current <- current + 1
                        store.Count > current
                    member this.Reset(): unit = 
                        current <- 0     
            }
        member this.GetEnumerator(): System.Collections.IEnumerator = 
            upcast (this :> IEnumerable<'T>).GetEnumerator()

///Example

type Employee (name) = 
    member _.Print () =
        printfn "%s" name

[<AbstractClass>]
type Traverser<'T>(items:seq<'T>) =
    abstract ProcessItem: 'T -> bool
    member this.Traverse (): bool = 
        let mutable result = None
        let iterator = items.GetEnumerator()
        while result |> Option.defaultValue true 
                && (iterator.MoveNext()) 
            do
                result <- Some <| this.ProcessItem(iterator.Current)
        result |> Option.defaultValue false

type PrintNEmployees(employees:seq<Employee> , n:int ) =
    inherit Traverser<Employee> (employees)
    let mutable count = 0
    override _.ProcessItem(e) =
        count <- count + 1
        e.Print()
        count < n

[<AbstractClass>]
type FilteringTraverser<'T>(items:seq<'T>) =
    abstract ProcessItem: 'T -> bool
    abstract TestItem: 'T -> bool
    member this.Traverse (): bool = 
        let mutable result = None
        let iterator = items.GetEnumerator()
        while result |> Option.defaultValue true 
                && (iterator.MoveNext()) 
            do
                if this.TestItem(iterator.Current) then
                    result <- Some <| this.ProcessItem(iterator.Current)
        result |> Option.defaultValue false