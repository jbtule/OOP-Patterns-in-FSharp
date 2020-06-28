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
module GangOfFour.Interpreter
open System
open System.Text
open System.IO

///Domain Code
type MatchStream = 
    { Data:string; Position:int} with 
    member this.NextAvailable(size) =
        this.Data.[this.Position..size], {this with Position = this.Position + size}

type State () =
    let streams = ResizeArray<MatchStream>()
    member _.IsEmpty with get () = 
        streams |> Seq.isEmpty
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
        while state.IsEmpty do
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

module Example =
    type String with
        member this.AsRegExpr with get() : RegularExpression =
            upcast LiteralExpression(this)
        member this.Repeat with get() = RepetitionExpression(this.AsRegExpr)

    type RegularExpression with
        member this.AsRegExpr with get() = this
        member this.Repeat with get() = RepetitionExpression(this.AsRegExpr)

    let inline (&&&) (exp1:^a when ^a : (member AsRegExpr: RegularExpression))
                        (exp2:^b when ^b : (member AsRegExpr: RegularExpression))  =
        SequenceExpression(exp1.AsRegExpr, exp2.AsRegExpr)

    let inline (|||) (alt1:^a when ^a : (member AsRegExpr: RegularExpression)) 
                        (alt2:^b when ^b : (member AsRegExpr: RegularExpression))  =
        AlternationExpression(alt1.AsRegExpr , alt2.AsRegExpr)
    
    let main _ =
        let regex = ("dog" ||| "cat").Repeat &&& "weather"
        ()