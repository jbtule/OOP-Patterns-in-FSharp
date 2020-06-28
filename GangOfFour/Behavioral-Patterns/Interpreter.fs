(***************************************************************** 
 * Interpreter (page 243)
 *   
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
namespace GangOfFour.Interpreter

module RegExInterpretor =
    open System
    open System.Text
    open System.IO
    ///Domain Code
    type MatchStream = 
        { Data:string; Position:int} with 
        member this.NextAvailable(size) =
            this.Data.[this.Position..this.Position + size - 1], {this with Position = this.Position + size}

    type State (?init:string) =
        let streams = ResizeArray<MatchStream>()
        do
            init |> Option.iter (fun s-> {Data=s;Position=0} |> streams.Add)
        member _.IsEmpty with get () = 
            streams |> Seq.isEmpty
        member _.Success with get () =
            streams |> Seq.where (fun x->x.Data.Length = x.Position) |> (not << Seq.isEmpty)
        member _.Add(s:MatchStream) = 
            s |> streams.Add
        member _.Iter (action:MatchStream -> unit) =
            streams |> Seq.iter action
        member this.AddAll(inputState:State) =
            inputState.Iter this.Add
        member _.Copy() =
            let finalState = State()
            streams |> Seq.iter finalState.Add
            finalState

    /// Interpreter (example ported from SmallTalk)
    [<AbstractClass>]
    type RegularExpression () = 
        abstract Match:State -> State

    and SequenceExpression(exp1:RegularExpression, exp2:RegularExpression) =
        inherit RegularExpression()
        override _.Match(inputState) =
            inputState |> exp1.Match |> exp2.Match

    and AlternationExpression(alt1:RegularExpression, alt2:RegularExpression) =
        inherit RegularExpression() 
        override _.Match(inputState) =
            let finalState = inputState |> alt1.Match
            inputState |> alt2.Match |> finalState.AddAll
            finalState

    and RepetitionExpression(repeat:RegularExpression) =
        inherit RegularExpression()
        override _.Match(inputState) =
            let mutable state = inputState
            let finalState = inputState.Copy()
            while not <| state.IsEmpty do
                state <- state |> repeat.Match
                state |> finalState.AddAll
            finalState

    and LiteralExpression(components:string) =
        inherit RegularExpression()
        override _.Match(inputState) =
            let finalState = State()
            inputState.Iter(
                fun s -> 
                    let next, result = s.NextAvailable(components.Length)
                    if next = components then
                        finalState.Add(result)
                )
            finalState

    ///Helpers for Syntax and Operators
    type String with
        member this.Expr with get() : RegularExpression=
            upcast LiteralExpression(this)
        member this.Repeat with get() : RegularExpression =
            upcast RepetitionExpression(this.Expr)

    type RegularExpression with
        member this.Expr with get() : RegularExpression=
            this
        member this.Repeat with get() : RegularExpression =
            upcast RepetitionExpression(this.Expr)
        static member (.|) (a : RegularExpression, b: RegularExpression) : RegularExpression =
            upcast AlternationExpression(a,b)
        static member (.&) (a : RegularExpression, b: RegularExpression) : RegularExpression =
            upcast SequenceExpression(a,b)

    ///Example 
    module Example1 =

        let main _ =
            let regex = ("dog ".Expr .| "cat ".Expr).Repeat .& "weather".Expr
            let m = (State "dog cat dog cat weather") |> regex.Match
            if(m.Success) then
                printfn "Matched!"
            ()

module BooleanInterpreter =
