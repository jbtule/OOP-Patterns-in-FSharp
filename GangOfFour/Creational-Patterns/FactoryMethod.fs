namespace GangOfFour.FactoryMethod

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

type Maze () =
    let rooms = ResizeArray()
    member _.AddRoom(r) = 
        rooms.Add(r)


type MazeGame () =
    //Factory Methods
    abstract MakeMaze: unit -> Maze
    default _.MakeMaze () = Maze()

    abstract MakeWall: unit -> Wall
    default _.MakeWall() = Wall()

    abstract MakeRoom: int -> Room
    default _.MakeRoom(n) = Room(n)
    
    abstract MakeDoor: Room * Room -> Door
    default _.MakeDoor(r1, r2) = Door(r1,r2)

    ///Normal Game setup
    member this.CreateMaze() =
        let maze = this.MakeMaze()
        let r1 = this.MakeRoom(1)
        let r2 = this.MakeRoom(2)
        let door = this.MakeDoor(r1,r2)
        
        maze.AddRoom(r1)
        maze.AddRoom(r2)

        r1.SetSide(North, this.MakeWall())
        r1.SetSide(East, door)
        r1.SetSide(South, this.MakeWall())
        r1.SetSide(West, this.MakeWall())

        r2.SetSide(North, this.MakeWall())
        r2.SetSide(East, this.MakeWall())
        r2.SetSide(South, this.MakeWall())     
        r2.SetSide(West, door)
        maze

///Enchanted Maze Alternative Game implemenation
type Spell = Alohomora
type EnchantedRoom (n, s) = inherit Room (n)
type DoorNeedingSpell(r1, r2) = inherit Door(r1, r2)
type EnchantedMazeGame () =
    inherit MazeGame ()
    let castSpell () = Alohomora
    override _.MakeRoom(n) =
        upcast EnchantedRoom (n, castSpell())
    override _.MakeDoor(r1,r2) = 
        upcast DoorNeedingSpell(r1,r2)

///Bomb Maze Alternative Game implemenation
type BombedWall () = inherit Wall()
type RoomWithABomb(n) = inherit Room(n)
type BombMazeGame () =
    inherit MazeGame ()
    override _.MakeWall() = upcast BombedWall()
    override _.MakeRoom(n) = upcast RoomWithABomb(n)