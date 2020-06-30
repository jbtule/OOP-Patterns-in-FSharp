(***************************************************************** 
 * TemplateMethod (page 273)
 *   
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.TemplateMethod

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)

///Sample Code
[<AbstractClass>]
type View () =
    member _.SetFocus () = notImpl "View Sets Focus"
    member _.ResetFocus () = notImpl "View Resets Focus"
    abstract DoDisplay : unit -> unit
    ///Template Method
    member this.Display () =
        this.SetFocus()
        this.DoDisplay()
        this.ResetFocus()
and MyView () = 
    inherit View ()
    override this.DoDisplay () = notImpl "Render Display COntent here"