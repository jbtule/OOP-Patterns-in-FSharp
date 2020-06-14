module GangOfFour.Bridge

let private notImpl msg = raise <| System.NotImplementedException(msg)

type NotFullyImplementedForExampleAttribute = AbstractClassAttribute

type Point = {X:float; Y:float}

[<NotFullyImplementedForExample>]
type WindowSystemFactory () =
    static member Instance () : WindowSystemFactory =
        notImpl "Singleton to pull correct factory"
    abstract MakeWindowImp : unit -> WindowImp
and [<AbstractClass>] View () =
    abstract DrawOn: Window -> unit
and [<NotFullyImplementedForExample>]  Window () =
    let _imp = lazy WindowSystemFactory.Instance().MakeWindowImp()
    member internal _.GetWindowImp() = _imp.Force()

    abstract DrawContents: unit -> unit

    abstract Open: unit -> unit
    abstract Close: unit -> unit
    abstract Iconfify: unit -> unit
    abstract Deiconfify: unit -> unit

    abstract SetOrigin: Point -> unit
    abstract SetExtent: Point -> unit
    abstract Raise: unit -> unit
    abstract Lower: unit -> unit

    abstract DrawLine: Point * Point -> unit
    abstract DrawRect: Point * Point -> unit
    default this.DrawRect (p1, p2) =
        let imp = this.GetWindowImp()
        imp.DeviceRect(p1.X, p1.Y, p2.X, p2.Y)
    abstract DrawPolygon: Point array * int -> unit
    abstract DrawText: string * Point -> unit


and [<AbstractClass>] WindowImp () =
    abstract ImpTop: unit -> unit
    abstract ImpBottom: unit -> unit
    abstract ImpSetOrigin: float * float -> unit
    abstract ImpSetExtent: float * float  -> unit

    abstract DeviceRect: float * float  * float * float  -> unit
    abstract DeviceText: string * float * float  -> unit
    abstract DeviceBitmap: byte array * float * float -> unit
    // lots more functions for drawing on windows...

[<NotFullyImplementedForExample>]
type ApplicationWindow (contents:View) =
    inherit Window ()
    override this.DrawContents() =  contents.DrawOn(this)

[<NotFullyImplementedForExample>]
type IconWindow(bitmap:byte[]) =
    inherit Window ()
    override this.DrawContents() = 
        let imp = this.GetWindowImp()
        imp.DeviceBitmap(bitmap, 0.0, 0.0)

module XWindowLib =
    [<AllowNullLiteral>]
    type Display = class end
    [<AllowNullLiteral>]
    type Drawable = class end
    [<AllowNullLiteral>]
    type GC = class end
    let XDrawRectangle(d:Display, id: Drawable, g:GC, x: int, y:int, w:int, h:int) =
        notImpl "Library Implementation"

[<AutoOpen>]
module XWindowBrdige =
    open XWindowLib
    [<NotFullyImplementedForExample>]
    type XWindowImp(dpy: Display, winId:Drawable, gc:GC) =
        inherit WindowImp()
        new () = XWindowImp(null,null, null)
        override _.DeviceRect(x0:float, y0:float, x1:float, y1:float) =
            let x = (x0,x1) ||> min |> round |> int
            let y = (y0,y1) ||> min |> round |> int
            let w = x0 - x1 |> abs |> round |> int
            let h = y0 - y1 |> abs |> round |> int 
            XDrawRectangle(dpy, winId, gc, x, y, w, h)

module PMWindowLib =
    [<AllowNullLiteral>]
    type HPS = class end
    type PPOINTL = 
        struct
            val mutable x: float
            val mutable y: float
        end
    type STATUS =  GPI_SUCESS | GPI_ERROR
    let GpiBeginPath(_:HPS, _:int64) : bool = notImpl "Library Implementation"
    let GpiSetCurrentPosition(_:HPS, _: PPOINTL) : bool = notImpl "Library Implementation"
    let GpiPolyLine(_:HPS, _:int64, _: PPOINTL array) : STATUS = notImpl "Library Implementation"
    let GpiEndPath(_:HPS) : bool = notImpl "Library Implementation"
    let GpiStrokePath(_:HPS, _:int64, _:int64) = notImpl "Library Implementation"
[<AutoOpen>]
module PMWindowBridge =
    open PMWindowLib
    [<NotFullyImplementedForExample>]
    type PMWindowImp(hps: HPS) =
        inherit WindowImp()
        new () = PMWindowImp(null)
        override _.DeviceRect(x0:float, y0:float, x1:float, y1:float) =
            let left = min x0 x1
            let right = max x0 x1
            let bottom = min y0 y1
            let top = max y0 y1

            let point = Array.zeroCreate<PPOINTL> 4
            let rect = 
                [
                    left, top
                    right,top
                    right, bottom
                    left, bottom
                ] 
            for i, (x, y) in List.indexed rect do
                point.[i].x <- x
                point.[i].y <- y
            
            if GpiBeginPath(hps, 1L) = false
                || GpiSetCurrentPosition(hps, point.[3]) = false
                || GpiPolyLine(hps, 4L, point) = GPI_ERROR
                || GpiEndPath(hps) = false 
            then
                notImpl "Report Error"
            else
                GpiStrokePath(hps, 1L, 0L)