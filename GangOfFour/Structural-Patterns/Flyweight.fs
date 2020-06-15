(***************************************************************** 
 * Flyweight (page 195)
 *  
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Flyweight

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)
type NotFullyImplementedForExampleAttribute = AbstractClassAttribute

//Flyway Sample Code
type Window () = class end
type Font () = class end

[<NotFullyImplementedForExample>]
type GlyphContext () =
    abstract Next: step:int -> unit
    abstract Insert: quantity:int -> unit
    abstract GetFont: unit -> Font
    abstract SetFont: Font * span:int -> unit
[<NotFullyImplementedForExample>]
type Glyph () =
    abstract Draw: Window * GlyphContext -> unit

    abstract SetFont: Font * GlyphContext -> unit
    abstract GetFont: GlyphContext -> Font

    abstract First: GlyphContext -> unit
    abstract Next: GlyphContext -> unit
    abstract IsDone: GlyphContext -> bool
    abstract Current: GlyphContext -> Glyph

    abstract Insert: Glyph * GlyphContext -> unit
    abstract Remove: GlyphContext -> unit

[<NotFullyImplementedForExample>]
type Character (charCode:char) =
    inherit Glyph()

type Row () = class end
type Column () = class end

type GlyphFactory () =
    let _characters = Array.create<Character option> 256 None

    member _.CreateCharacter(c:char) =
        match _characters.[int c] with
        | None ->
            let n:Character = notImpl "c |> Character"  
            _characters.[int c] <- n |> Some
            n 
        | Some x -> x

    member _.CreateRow() =
        Row ()
    member _.CreateColumn() =
        Column ()