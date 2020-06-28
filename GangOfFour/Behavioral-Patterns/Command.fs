(***************************************************************** 
 * Command (page 223)
 *   aka Action, Transaction
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Command

open System
open System.Collections.Generic

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)

///Domain Code
type Application () = 
    member _.Add(doc:Document) : unit= notImpl "Add  document to application"
and Document(name:string) = 
    member _.Open() : unit = notImpl "Open Document"
    member _.Paste() : unit = notImpl "Paste into Document"

///Command Code
[<AbstractClass>]
type Command () =
    abstract Execute: unit -> unit

type OpenCommand(app:Application) =
    inherit Command()
    let askUser () : string = notImpl "Prompts User for the name of the Document"
    override _.Execute() =
        let name = askUser()
        if not <| String.IsNullOrEmpty(name) then
            let doc = Document(name)
            app.Add(doc)
            doc.Open();

type PasteCommand (doc:Document) =
    inherit Command()
    override _.Execute() =
        doc.Paste()

type SimpleCommand<'T> (receiver:'T, action:'T -> unit) =
    inherit Command()
    override _.Execute () =
        receiver |> action

type MacroCommand() =
    inherit Command()
    member val Commands 
        = upcast ResizeArray<Command>() : ICollection<Command> with get
    override this.Execute () =
        this.Commands
        |> Seq.iter (fun c->c.Execute())