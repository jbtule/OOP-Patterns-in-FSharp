(***************************************************************** 
 * Visitor (page 331)
 *   
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Vistor

module EquipmentVistor = ()


module RegexVistor =

    /// Domain Code
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

    /// Visitor (example ported from SmallTalk)
    [<AbstractClass>]
    type RegularExpression () = 
        abstract Accept:REMatchingVistor -> State

    and SequenceExpression(exp1:RegularExpression, exp2:RegularExpression) =
        inherit RegularExpression()
        member _.Exp1 = exp1
        member _.Exp2 = exp2
        override this.Accept(visitor) =
            visitor.VisitSequence(this)

    and AlternationExpression(alt1:RegularExpression, alt2:RegularExpression) =
        inherit RegularExpression() 
        member _.Alt1 = alt1
        member _.Alt2 = alt2
        override this.Accept(visitor) =
            visitor.VisitAlternation(this)

    and RepetitionExpression(repeat:RegularExpression) =
        inherit RegularExpression()
        member _.Repeat = repeat
        override this.Accept(visitor) =
            visitor.VisitRepeat(this)

    and LiteralExpression(components:string) =
        inherit RegularExpression()
        member _.Components = components
        override this.Accept(visitor) =
            visitor.VisitLiteral(this)

    and REMatchingVistor () = 
        let mutable inputState = State()
        member this.VisitSequence(seqExp:SequenceExpression) =
            inputState <- seqExp.Exp1.Accept(this)
            seqExp.Exp2.Accept(this)
        member this.VisitRepeat(repExp:RepetitionExpression) =
            let finalState = inputState.Copy()
            while not <| inputState.IsEmpty do
                inputState <- repExp.Repeat.Accept(this)
                inputState |> finalState.AddAll
            finalState
        member this.VisitAlternation(altExp:AlternationExpression) =
            let originalState = inputState
            let finalState = altExp.Alt1.Accept(this)
            inputState <- originalState
            altExp.Alt2.Accept(this) |> finalState.AddAll
            finalState
        member _.VisitLiteral(literalExp:LiteralExpression) =
            let finalState = State()
            inputState.Iter(
                fun s -> 
                    let next, result = s.NextAvailable(literalExp.Components.Length)
                    if next = literalExp.Components then
                        finalState.Add(result)
                )
            finalState
