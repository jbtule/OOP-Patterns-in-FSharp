module GangOfFour.Composite

open System.Collections.Generic

///Domain Types

[<Measure>] type watts
[<Measure>] type dollars

module Database =
    let LookupPower name : int<watts> =
        raise <| System.NotImplementedException()
    let LookupPrice name : decimal<dollars> =
        raise <| System.NotImplementedException()
    let LookupDiscountPrice name : decimal<dollars> =
        raise <| System.NotImplementedException()

///Abstract base Class

[<AbstractClass>]
type Equipment (name) =
    member val Name = name with get
    abstract Power : int<watts>
    default _.Power with get () = Database.LookupPower name
    abstract NetPrice: decimal<dollars>
    default _.NetPrice with get () = Database.LookupPrice name
    abstract DiscountPrice: decimal<dollars>
    default _.DiscountPrice with get () = Database.LookupDiscountPrice name

///Leaf Types

type FloppyDisk (name) = inherit Equipment(name)
type Card (name) = inherit Equipment(name)

///Abstract Composite

[<AbstractClass>]
type CompositeEqupment(name) =
    inherit Equipment(name)
    let _equipment = ResizeArray<Equipment>()
    override _.Power with get () = _equipment |> Seq.sumBy (fun x->x.Power)
    override _.NetPrice with get () = _equipment |> Seq.sumBy (fun x->x.NetPrice)
    override _.DiscountPrice with get () = _equipment |> Seq.sumBy (fun x->x.DiscountPrice)
    member _.Add(item) = _equipment.Add(item)
    interface IEnumerable<Equipment> with
        member _.GetEnumerator(): IEnumerator<Equipment> = 
            upcast _equipment.GetEnumerator()
        member _.GetEnumerator(): System.Collections.IEnumerator = 
            upcast _equipment.GetEnumerator()


///Composite Types       
type Chassis (name) = inherit CompositeEqupment(name)
type Cabinet (name) = inherit CompositeEqupment(name)
type Bus (name) = inherit CompositeEqupment(name)

module Example =
    let main _ =
        let cabinet = Cabinet("PC Cabinet")
        let chassis = Chassis("PC CHassis")

        cabinet.Add(chassis)

        let bus = Bus("MCA Bus")
        bus.Add(Card "16Mbs Token Ring")
        
        chassis.Add(bus)
        chassis.Add(FloppyDisk "3.5inch Floppy")

        chassis.NetPrice |> printf "The net price is %M" 
