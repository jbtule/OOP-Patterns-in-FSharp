namespace rec GangOfFour.Prototype

open System.Collections.Generic
type private Make = System.Activator

type Direction = North | East | South | West

[<AbstractClass>]
type Edge () = class end
type Wall () =
    inherit Edge ()
    abstract CopyWall: unit -> Wall
    override this.CopyWall() =
        downcast Make.CreateInstance(this.GetType())

type Door (r1:Room, r2:Room) = 
    inherit Edge ()
    internal new () = Door(Room(),Room())
    abstract CopyDoor: Room * Room -> Door
    override this.CopyDoor(nr1, nr2) =
        downcast Make.CreateInstance(this.GetType(),nr1,nr2)

type Room (n:int) =
    let sides = new Dictionary<Direction, Edge>()
    internal new () = Room(0)
    member _.SetSide(d, e:#Edge) =
        sides.[d] <- e
    abstract CopyRoom: int -> Room
    default this.CopyRoom(n) =
        downcast Make.CreateInstance(this.GetType(),n)

type Maze () =
    let rooms = ResizeArray()
    member _.AddRoom(r) = 
        rooms.Add(r)
    abstract CopyMaze: unit -> Maze
    default this.CopyMaze() =
        downcast Make.CreateInstance(this.GetType())

type MazeFactory () =
    abstract MakeMaze: unit -> Maze
    default _.MakeMaze () = Maze()

    abstract MakeWall: unit -> Wall
    default _.MakeWall() = Wall()

    abstract MakeRoom: int -> Room
    default _.MakeRoom(n) = Room(n)
    
    abstract MakeDoor: Room * Room -> Door
    default _.MakeDoor(r1, r2) = Door(r1,r2)

type MazePrototypeFactory (maze:Maze, wall:Wall, room:Room, door:Door) =
    inherit MazeFactory()
    override _.MakeWall() = wall.CopyWall()
    override _.MakeDoor(r1,r2) = door.CopyDoor(r1,r2)
    override _.MakeRoom(n) = room.CopyRoom(n)
    override _.MakeMaze() = maze.CopyMaze()


//Bomb Maze
type BombedWall () = inherit Wall()
type RoomWithABomb(n) = 
    inherit Room(n)
    new () = RoomWithABomb(0)

module MazeFactories =
    let simpleMazeFactor = MazePrototypeFactory(Maze(), Wall(), Room(), Door())
    let bombedMazeFactor = MazePrototypeFactory(Maze(), BombedWall(), RoomWithABomb(), Door())