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
    interface IEnumerable<'T> with
        member _.GetEnumerator () : IEnumerator<'T> =
            upcast new MyEnumerator<'T>(store)
        member this.GetEnumerator(): System.Collections.IEnumerator = 
            upcast (this :> IEnumerable<'T>).GetEnumerator()

and MyEnumerator<'T> (list: ResizeArray<'T>) =
    let mutable current = 0;
    interface IEnumerator<'T> with
        member this.Current with get (): 'T = 
            if list.Count > current then
                list.[current]
            else
                failwith "Enumerator Out Of Bounds"
        member this.Current with get(): obj = 
            upcast (this :> IEnumerator<'T>).Current
        member _.Dispose(): unit = ()
        member _.MoveNext(): bool = 
            current <- current + 1
            list.Count > current
        member this.Reset(): unit = 
            current <- 0           

///Example
[<AbstractClass>]
type ListTraverser<'T>(list:MyList<'T>) =
    abstract ProcessItem: 'T -> bool
    member this.Traverse (): bool = 
        let mutable result = None
        let iterator = (list :> IEnumerable<'T>).GetEnumerator()
        while 
            result |> Option.defaultValue true 
            && (iterator.MoveNext()) 
            do
            result <- Some <| this.ProcessItem(iterator.Current)
        result |> Option.defaultValue false