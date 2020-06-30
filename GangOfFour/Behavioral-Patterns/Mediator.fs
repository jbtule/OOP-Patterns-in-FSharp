(***************************************************************** 
 * Mediator (page 273)
 *   
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module rec GangOfFour.Mediator

open FSharp.Interop.NullOptAble

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)

//Example Mediator

[<AbstractClass>]
type DialogDirector () =
    abstract ShowDialog: unit -> unit
    abstract WidgetChanged: Widget -> unit
    abstract CreateWidgets: unit -> unit

type MouseEvent = class end

[<AbstractClass>]
type Widget (dialog: DialogDirector) =
    abstract Changed: unit -> unit
    default this.Changed() = dialog.WidgetChanged(this)
    abstract HandleMouse: MouseEvent -> unit
    //...

type ListBox (dialog) =
    inherit Widget(dialog)
    abstract GetSelection: unit -> string
    default _.GetSelection() = notImpl "Get Selection"
    abstract SetList: List<char> -> unit
    default _.SetList(listItems) = notImpl "SetList"
    override _.HandleMouse(event) = notImpl "Handle Mouse"
    //...

type EntryField (dialog) =
    inherit Widget(dialog)
    abstract SetText:string -> unit
    default _.SetText(text) = notImpl "Set Text"
    abstract GetText:unit -> string
    default _.GetText() = notImpl "Get Text"
    override _.HandleMouse(event) = notImpl "Handle Mouse"
    // ...

type Button (dialog) =
    inherit Widget(dialog)
    abstract SetText:string -> unit
    default _.SetText(text) = notImpl "Set Text"
    // ...
    override this.HandleMouse(event) =
        // ...
        this.Changed()

type FontDialogDirector () =
    inherit DialogDirector()
    let mutable _ok = None
    let mutable _cancel = None
    let mutable _fontList = None
    let mutable _fontName = None
    override _.ShowDialog () = notImpl "Show Dialog"
    override this.CreateWidgets() =
        _ok <- Some <| Button(this)
        _cancel <- Some <| Button(this)
        _fontList <- Some <| ListBox(this)
        _fontName <- Some <| EntryField(this)
        // ... Fill in list box
        // ... Assemble widgets in dialog
    override this.WidgetChanged(changedWidget) =
        guard {
            let! ok = _ok 
            let! fontList = _fontList
            let! cancel = _cancel 
            let! fontName = _fontName
            match changedWidget with
            | :? ListBox as fl when fl = fontList  ->
                fontList.GetSelection() |> fontName.SetText
            | :? Button as ok' when ok' = ok ->
                //apply font change and dismiss dialog
                notImpl "ok"
            | :? Button as cancel' when cancel' = cancel ->
                //dismiss dialog
                notImpl "cancel"
            | _ -> ()
        }
