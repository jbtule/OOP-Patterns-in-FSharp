(***************************************************************** 
 * Decorator (page 175)
 *  aka Wrapper
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Decorator

//Domain Types
[<AbstractClass;AllowNullLiteral>]
type VisualComponent () =
    abstract Draw: unit-> unit
    abstract Resize: unit -> unit

type Window () =
    member val Contents:VisualComponent = null  with get, set

[<AbstractClass>]
type TextView () =
    inherit VisualComponent()

//Decorator Pattern Types
type Decorator(comp:VisualComponent) =
    inherit VisualComponent()
    override _.Draw() =
        comp.Draw()
    override _.Resize() =
        comp.Resize()

type BorderDecorator(comp:VisualComponent, borderWidth:int) =
    inherit Decorator(comp)
    member private _.DrawBorder() = raise <| System.NotImplementedException()
    override this.Draw () =
        base.Draw()
        this.DrawBorder()

type ScrollDecorator (comp:VisualComponent) =
    inherit Decorator(comp)

//Example Code
module Example =
    let window: Window = failwith "Ref Window"
    let textView: TextView = failwith "Ref Text View"
    let main _ =
        window.Contents <- 
            BorderDecorator(ScrollDecorator(textView), 1)
            