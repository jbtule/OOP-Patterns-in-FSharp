namespace rec GangOfFour.Adapter

type Point = {X:float; Y:float}
type BoundingBox = {BottomLeft: Point; TopRight:Point}


[<AbstractClass>]
type Shape () =
    abstract BoundingBox : BoundingBox with get
    abstract CreateManipulator : unit -> Manipulator

[<AbstractClass>]
type TextView () =
    abstract GetOrigin: unit -> float * float
    abstract GetExtent: unit -> float * float
    abstract IsEmpty: unit -> bool

type Manipulator () = class end
type TextManipulator (ts: TextShape) =
    inherit Manipulator ()

type TextShape(textView:TextView) =
    inherit Shape()
    override _.BoundingBox with get () =
        let left, bottom = textView.GetOrigin()
        let width, height = textView.GetExtent()
        {
            BottomLeft= {Y=bottom; X=left}
            TopRight= {Y=bottom+height; X=left+width}
        }
    override this.CreateManipulator() =
        upcast TextManipulator(this)
    member _.IsEmpty() =
        textView.IsEmpty()