(***************************************************************** 
 * Proxy (page 195)
 *  aka Surrogate
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Proxy

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)
type NotFullyImplementedForExampleAttribute = AbstractClassAttribute

//Domain Code
open System.IO

type Point = {X:int; Y:int}

[<AbstractClass>]
type Graphic () =
    abstract Draw:Point -> unit
    abstract HandleMouse:Event<obj> -> unit
    abstract Extent:Point with get
    abstract Load: Stream -> unit
    abstract Save: Stream -> unit

[<Sealed>]
type Image (fileName:string) =
    inherit Graphic()
    override _.Draw (_)= notImpl "Sample Code"
    override _.HandleMouse (_)= notImpl "Sample Code"
    override _.Extent with get () = notImpl "Sample Code"
    override _.Load(_) = notImpl "Sample Code"
    override _.Save(_) = notImpl "Sample Code"


//Virtual Proxy

type ImageProxy (fileName) =
    inherit Graphic()
    let mutable _fileName = fileName;
    let mutable _cachedExtent:Point option = None
    let _image = lazy Image(_fileName)
    let _extent = lazy _image.Value.Extent
    member _.Image with get () = _image.Force()
    override _.Extent with get () = _cachedExtent |> Option.defaultWith _extent.Force
    override this.Draw (at)= this.Image.Draw(at)
    override this.HandleMouse (event)= this.Image.HandleMouse(event)
    override this.Save(dest) = 
        use writer = new BinaryWriter(dest)
        writer.Write(this.Extent.X)
        writer.Write(this.Extent.Y)
        writer.Write(_fileName)
    override this.Load(from) = 
        use reader = new BinaryReader(from)
        let x=reader.ReadInt32()
        let y=reader.ReadInt32()
        _cachedExtent <- Some {X=x;Y=y}
        _fileName <- reader.ReadString()

//Dynamic Proxy
open FSharp.Interop.Dynamic
open FSharp.Reflection

type LegalProxy(target:obj, legalMemberNames:string Set) =
    inherit System.Dynamic.DynamicObject()
    override _.TryInvokeMember(binder, args, (result:obj byref)) =
        if legalMemberNames.Contains(binder.Name) then
            let tupleType = typeof<obj> |> Array.create binder.CallInfo.ArgumentCount |> FSharpType.MakeTupleType
            let fsharpArg = FSharpValue.MakeTuple(args, tupleType)
            result <- target |> Dyn.invokeMember binder.Name fsharpArg
            true
        else
            false