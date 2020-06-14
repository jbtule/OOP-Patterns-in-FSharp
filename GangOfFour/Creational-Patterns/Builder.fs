namespace GangOfFour.Builder

open System.Collections.Generic

type Direction = North | East | South | West

[<AbstractClass>]
type Edge () = class end
type Wall () = inherit Edge ()
type Door (r1, r2) = inherit Edge ()
and Room (n:int) =
    let sides = new Dictionary<Direction, Edge>()
    member _.SetSide(d, e:#Edge) =
        sides.[d] <- e
    member _.No () = n
    member _.GetSide(d) =
        let exists,s =sides.TryGetValue(d)
        if exists then Some s else None

type Maze () =
    let rooms = Dictionary<int,Room>()
    member _.AddRoom(r:Room) = 
        rooms.Add(r.No(), r)
    member _.RoomNo(n) =
        let exists,r =rooms.TryGetValue(n)
        if exists then Some r else None

[<AbstractClass>]
type MazeBuilder () =
    abstract BuildMaze: unit -> unit
    abstract BuildRoom: int -> unit
    abstract BuildDoor: int * int -> unit
    abstract GetMaze: unit -> Maze

type StandardMazeBuilder () =
    inherit MazeBuilder()
    let mutable _currentMaze = None
    let getMaze () = 
        match _currentMaze with
        | Some m -> m 
        | None -> failwith "BuildMaze() was never called." 
    // The Sample code didn't implement the "commonWall" utility function.
    // This is the dumbest implementation I could think of that was reasonable.
    let commonWall (r1:Room) (r2:Room) =
        let combos =
            [
                North, South
                East, West
                South, North
                West, East
            ]
        seq { 
                for d1, d2 in combos do
                    match r1.GetSide(d1), r2.GetSide(d2) with
                    | None, None -> yield d1, d2
                    | _, _ -> ()
            }
            |> Seq.tryHead
            |> Option.defaultWith (failwith "No Matching Sides Available")
    override _.BuildMaze() =
        _currentMaze <- Maze() |> Some
    override _.GetMaze() = getMaze ()
    override _.BuildRoom(n) =
        let r = getMaze().RoomNo(n)
        match r with
        | None -> 
            let room = Room(n)
            getMaze().AddRoom(room)
            room.SetSide(North, Wall())
            room.SetSide(East, Wall())
            room.SetSide(South, Wall())
            room.SetSide(West, Wall())
        | _ -> ()
    override _.BuildDoor(n1, n2) =
        let r1 = getMaze().RoomNo(n1)
        let r2 = getMaze().RoomNo(n2)
        match r1, r2 with
        | Some r1', Some r2' ->
            let door = Door(r1', r2')
            let d1,d2 = commonWall r1' r2'
            r1'.SetSide(d1, door)
            r2'.SetSide(d2, door)
        | f1, f2 -> failwithf "Build Door requires both Rooms %i:%A, %i:%A." n1 f1 n2 f2 

type CountingMazeBuilder() =
    inherit MazeBuilder()
    let mutable _rooms,_doors = 0,0
    override _.BuildMaze() =
        _rooms<-0;_doors <- 0
    override _.BuildRoom(_) =
        _rooms <- _rooms + 1
    override _.BuildDoor(_,_) =
        _doors <- _doors + 1
    override _.GetMaze () = raise <| System.NotImplementedException() 
    member _.GetCounts() = struct (_rooms, _doors)